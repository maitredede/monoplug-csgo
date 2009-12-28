#include "monoplug.h"

SH_DECL_HOOK6(IServerGameDLL, LevelInit, SH_NOATTRIB, 0, bool, char const *, char const *, char const *, char const *, bool, bool);
SH_DECL_HOOK3_void(IServerGameDLL, ServerActivate, SH_NOATTRIB, 0, edict_t *, int, int);
SH_DECL_HOOK1_void(IServerGameDLL, GameFrame, SH_NOATTRIB, 0, bool);
SH_DECL_HOOK0_void(IServerGameDLL, LevelShutdown, SH_NOATTRIB, 0);

CMonoPlug g_MonoPlugPlugin;

IServerGameDLL *server = NULL;
IServerGameClients *gameclients = NULL;
IVEngineServer *engine = NULL;
IServerPluginHelpers *helpers = NULL;
IGameEventManager2 *gameevents = NULL;
IServerPluginCallbacks *vsp_callbacks = NULL;
IPlayerInfoManager *playerinfomanager = NULL;

ICvar *icvar = NULL;
IFileSystem* filesystem = NULL;
CGlobalVars *gpGlobals = NULL;

MonoDomain* g_Domain = NULL;
MonoAssembly* g_Assembly = NULL;
MonoImage *g_Image = NULL;
MonoClass *g_Class = NULL;

MonoMethod* g_Init = NULL;
MonoMethod* g_Shutdown = NULL;
MonoMethod* g_HandleMessage = NULL;

MonoMethod* g_EVT_LevelInit = NULL;
MonoMethod* g_EVT_LevelShutdown = NULL;
MonoMethod* g_EVT_ConVarStringValueChanged = NULL;


/** 
 * Something like this is needed to register cvars/CON_COMMANDs.
 */
class BaseAccessor : public IConCommandBaseAccessor
{
public:
	bool RegisterConCommandBase(ConCommandBase *pCommandBase)
	{
		/* Always call META_REGCVAR instead of going through the engine. */
		return META_REGCVAR(pCommandBase);
	}
} s_BaseAccessor;

PLUGIN_EXPOSE(CMonoPlug, g_MonoPlugPlugin);

static void ConVarStringChangeCallback(IConVar *var, const char *pOldValue, float flOldValue)
{
	ConVar* v = (ConVar*)var;
	int i = g_MonoPlugPlugin.m_convarStringPtr->Find(v);
	if(i>=0)
	{
		uint64Container* value = g_MonoPlugPlugin.m_convarStringId->Element(i);

		gpointer args[1];
		uint64 val = value->Value();
		args[0] = &val;
		MONO_CALL_ARGS(g_MonoPlugPlugin.m_ClsMain, g_EVT_ConVarStringValueChanged, args);
	}
};


static bool Mono_RegisterConCommand(MonoString* name, MonoString* description, MonoDelegate* code, int flags)
{
	MonoConCommand* com = new MonoConCommand(mono_string_to_utf8(name), mono_string_to_utf8(description), code, flags);
	g_pCVar->RegisterConCommand(com);
	//{
		g_MonoPlugPlugin.m_conCommands->AddToTail(com);
		return true;
	//}
	//else
	//{
	//	delete com;
	//	return false;
	//}
};

static bool Mono_UnregisterConCommand(MonoString* name)
{
	const char* s_name = mono_string_to_utf8(name);

	for(int i = 0; i < 	g_MonoPlugPlugin.m_conCommands->Count(); i++)
	{
		MonoConCommand* item = 	g_MonoPlugPlugin.m_conCommands->Element(i);

		if(Q_strcmp(item->GetName(), s_name) == 0)
		{
			g_pCVar->UnregisterConCommand(item);
			g_MonoPlugPlugin.m_conCommands->Remove(i);
			delete item;
			return true;
		}
	}

	META_CONPRINTF("Mono_UnregisterConCommand : Command NOT found\n");
	return false;
};

static uint64 Mono_RegisterConVarString(MonoString* name, MonoString* description, int flags, MonoString* defaultValue)
{
	uint64 nativeID = ++g_MonoPlugPlugin.m_convarStringIdValue;
	ConVar* var = new ConVar(
		mono_string_to_utf8(name),
		mono_string_to_utf8(defaultValue),
		flags,
		mono_string_to_utf8(description)
		, (FnChangeCallback_t)ConVarStringChangeCallback
		);
	g_pCVar->RegisterConCommand(var);
	g_MonoPlugPlugin.m_convarStringId->AddToTail(new uint64Container(nativeID));
	g_MonoPlugPlugin.m_convarStringPtr->AddToTail(var);
	return nativeID;
};

static void Mono_UnregisterConVarString(uint64 nativeID)
{
	for(int i=0;i<g_MonoPlugPlugin.m_convarStringId->Count(); i++)
	{
		uint64Container* cont = g_MonoPlugPlugin.m_convarStringId->Element(i);
		if(cont->Value() == nativeID)
		{
			ConVar* var = g_MonoPlugPlugin.m_convarStringPtr->Element(i);
			g_pCVar->UnregisterConCommand(var);
			g_MonoPlugPlugin.m_convarStringId->Remove(i);
			g_MonoPlugPlugin.m_convarStringPtr->Remove(i);
			delete var;
			break;
		}
	}

};

static MonoString* Mono_GetConVarStringValue(uint64 nativeID)
{
#ifdef _DEBUG
	META_CONPRINTF("Entering Mono_GetConVarStringValue : %l\n", nativeID);
#endif
	for(int i=0;i<g_MonoPlugPlugin.m_convarStringId->Count(); i++)
	{
		uint64Container* cont = g_MonoPlugPlugin.m_convarStringId->Element(i);
		if(cont->Value() == nativeID)
		{
			ConVar* var = g_MonoPlugPlugin.m_convarStringPtr->Element(i);

			return MONO_STRING(g_Domain, var->GetString());
		}
	}
	//return MONO_STRING(g_Domain, "TEST");
	return NULL;
};

static void Mono_SetConVarStringValue(uint64 nativeID, MonoString* value)
{
#ifdef _DEBUG
	META_CONPRINT("Entering Mono_SetConVarStringValue\n");
#endif

	for(int i=0;i<g_MonoPlugPlugin.m_convarStringId->Count(); i++)
	{
		uint64Container* cont = g_MonoPlugPlugin.m_convarStringId->Element(i);
		if(cont->Value() == nativeID)
		{
			ConVar* var = g_MonoPlugPlugin.m_convarStringPtr->Element(i);
			var->SetValue(mono_string_to_utf8(value));
			break;
		}
	}
};


bool CMonoPlug::Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late)
{
	PLUGIN_SAVEVARS();

	GET_V_IFACE_CURRENT(GetEngineFactory, engine, IVEngineServer, INTERFACEVERSION_VENGINESERVER);
	GET_V_IFACE_CURRENT(GetEngineFactory, gameevents, IGameEventManager2, INTERFACEVERSION_GAMEEVENTSMANAGER2);
	GET_V_IFACE_CURRENT(GetEngineFactory, helpers, IServerPluginHelpers, INTERFACEVERSION_ISERVERPLUGINHELPERS);
	GET_V_IFACE_CURRENT(GetEngineFactory, icvar, ICvar, CVAR_INTERFACE_VERSION);
	GET_V_IFACE_ANY(GetServerFactory, server, IServerGameDLL, INTERFACEVERSION_SERVERGAMEDLL);
	GET_V_IFACE_ANY(GetServerFactory, gameclients, IServerGameClients, INTERFACEVERSION_SERVERGAMECLIENTS);
	GET_V_IFACE_ANY(GetServerFactory, playerinfomanager, IPlayerInfoManager, INTERFACEVERSION_PLAYERINFOMANAGER);

	GET_V_IFACE_CURRENT(GetEngineFactory, filesystem, IFileSystem, FILESYSTEM_INTERFACE_VERSION);

	gpGlobals = ismm->GetCGlobals();

	/* Load the VSP listener.  This is usually needed for IServerPluginHelpers. */
	if ((vsp_callbacks = ismm->GetVSPInfo(NULL)) == NULL)
	{
		ismm->AddListener(this, this);
		ismm->EnableVSPListener();
	}

	//Get game dir
	char* dir = new char[MAX_PATH];
	engine->GetGameDir(dir, MAX_PATH);
	char dllPath[MAX_PATH];
	Q_snprintf( dllPath, sizeof(dllPath), MONOPLUG_DLLFILE, dir); 
	delete dir;

	META_CONPRINTF("MONOPLUG : managed dll file is : %s\n", dllPath);
	if(!filesystem->FileExists(dllPath))
	{
		Q_snprintf(error, maxlen, "Can't find managed DLL : %s", dllPath);
		META_CONPRINTF("%s\n", error);
		return false;
	}

	//One time init
	if(!g_Domain)
	{
//#ifdef _WIN32
//		HINSTANCE hDLL = NULL;
//		hDLL = LoadLibrary("mono.dll");
//		if(hDLL)
//		{
//			META_CONPRINT("Mono.dll loaded\n");
//		}
//		else
//		{
//			META_CONPRINT("Mono.dll NOT loaded\n");
//		}
//#endif

#ifndef _WIN32
		//crash on windows
		mono_config_parse(NULL);
#endif

		g_Domain = mono_jit_init(dllPath);
		if(!g_Domain)
		{
			Q_snprintf(error, maxlen, "Can't init Mono JIT");
			META_CONPRINTF("%s\n", error);
			return false;
		}
		g_Assembly = mono_domain_assembly_open(g_Domain, dllPath);
		if(!g_Assembly)
		{
			Q_snprintf(error, maxlen, "Can't open assembly : %s", dllPath);
			META_CONPRINTF("%s\n", error);
			return false;
		}
		g_Image = mono_assembly_get_image(g_Assembly);
		if(!g_Image)
		{
			Q_snprintf(error, maxlen, "Can't get assembly image");
			META_CONPRINTF("%s\n", error);
			return false;
		}
		g_Class = mono_class_from_name(g_Image, MONOPLUG_NAMESPACE, MONOPLUG_CLASSNAME);
		if(!g_Class)
		{
			Q_snprintf(error, maxlen, "Can't get main type");
			META_CONPRINTF("%s\n", error);
			return false;
		}

#ifdef _DEBUG
	//TODO : enumerate ClsMain methods signature to debug purposes
	META_CONPRINTF("M:DEBUG\n");
	void* iter = NULL;
	MonoMethod* m = NULL;
	while ((m = mono_class_get_methods (g_Class, &iter)))
	{
		META_CONPRINTF("%s\n", mono_method_full_name(m, true));
	}
#else
	META_CONPRINTF("M:RELEASE\n");
#endif


		//Add internal calls from managed to native
		mono_add_internal_call (MONOPLUG_CALLBACK_MSG, (gconstpointer)Mono_Msg);
		mono_add_internal_call (MONOPLUG_CALLBACK_REGISTERCONCOMMAND, (gconstpointer)Mono_RegisterConCommand);
		mono_add_internal_call (MONOPLUG_CALLBACK_UNREGISTERCONCOMMAND, (gconstpointer)Mono_UnregisterConCommand);
		mono_add_internal_call (MONOPLUG_CALLBACK_REGISTERCONVARSTRING, (gconstpointer)Mono_RegisterConVarString);
		mono_add_internal_call (MONOPLUG_CALLBACK_UNREGISTERCONVARSTRING, (gconstpointer)Mono_UnregisterConVarString);

		mono_add_internal_call (MONOPLUG_CALLBACK_CONVARSTRING_GETVALUE, (gconstpointer)Mono_GetConVarStringValue);
		mono_add_internal_call (MONOPLUG_CALLBACK_CONVARSTRING_SETVALUE, (gconstpointer)Mono_SetConVarStringValue);

		//Get callbacks from native to managed
		ATTACH(MONOPLUG_NATMAN_INIT, g_Init, g_Image);
		ATTACH(MONOPLUG_NATMAN_SHUTDOWN, g_Shutdown, g_Image);
		ATTACH(MONOPLUG_NATMAN_HANDLEMESSAGE, g_HandleMessage, g_Image);
		
		//Events
		ATTACH(MONOPLUG_CLSMAIN_EVT_LEVELINIT, g_EVT_LevelInit, g_Image);
		ATTACH(MONOPLUG_CLSMAIN_EVT_LEVELSHUTDOWN, g_EVT_LevelShutdown, g_Image);
		ATTACH(MONOPLUG_NATMAN_CONVARSTRING_VALUECHANGED, g_EVT_ConVarStringValueChanged, g_Image);
	}
	
	//ConCommandBase init code
	this->m_conCommands = new CUtlVector<MonoConCommand*>();

	//this->m_conVarString = new CUtlVector<MonoConVarString*>();
	this->m_convarStringId = new CUtlVector<uint64Container*>();
	this->m_convarStringPtr = new CUtlVector<ConVar*>();
	this->m_convarStringIdValue = 0;
	
	
	//Create object instance
	this->m_ClsMain = mono_object_new(g_Domain, g_Class);
	mono_runtime_object_init(this->m_ClsMain);
	
	MONO_CALL(this->m_ClsMain, g_Init);

	//Final hooks	
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, LevelInit, server, this, &CMonoPlug::Hook_LevelInit, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, server, this, &CMonoPlug::Hook_ServerActivate, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, server, this, &CMonoPlug::Hook_GameFrame, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, LevelShutdown, server, this, &CMonoPlug::Hook_LevelShutdown, false);

	ENGINE_CALL(LogPrint)("All hooks started!\n");

#if SOURCE_ENGINE >= SE_ORANGEBOX
	g_pCVar = icvar;
	ConVar_Register(0, &s_BaseAccessor);
#else
	ConCommandBaseMgr::OneTimeInit(&s_BaseAccessor);
#endif
	return true;
}

bool CMonoPlug::Unload(char *error, size_t maxlen)
{
	//Mono is leaked, so trying to unload plugin lead to segfault
	Q_snprintf(error, maxlen, "Unload denied to avoid segfault caused by Mono.");
	return false;
	
	//SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, server, this, &CMonoPlug::Hook_ServerActivate, true);
	//SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, GameFrame, server, this, &CMonoPlug::Hook_GameFrame, true);

	//SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, LevelInit, server, this, &CMonoPlug::Hook_LevelInit, true);
	//SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, LevelShutdown, server, this, &CMonoPlug::Hook_LevelShutdown, false);

	//Clean mono
	//MONO_CALL(this->m_ClsMain, g_Shutdown);

	//DO NOT CALL : leaked -> generate a segfault and Mono suggest only OneTime init
	//mono_jit_cleanup(this->m_ClrDomain);
	
	//delete this->m_conCommands;
	
	//return true;
}

void CMonoPlug::OnVSPListening(IServerPluginCallbacks *iface)
{
	vsp_callbacks = iface;
}

void CMonoPlug::Hook_ServerActivate(edict_t *pEdictList, int edictCount, int clientMax)
{
	META_LOG(g_PLAPI, "ServerActivate() called: edictCount = %d, clientMax = %d", edictCount, clientMax);
}

void CMonoPlug::Hook_GameFrame(bool simulating)
{
	/**
	 * simulating:
	 * ***********
	 * true  | game is ticking
	 * false | game is not ticking
	 */
	 //MONO_CALL(this->m_ClsMain, g_HandleMessage);
}

bool CMonoPlug::Hook_LevelInit(const char *pMapName,
								char const *pMapEntities,
								char const *pOldLevel,
								char const *pLandmarkName,
								bool loadGame,
								bool background)
{

	void* args[6];
	args[0] = MONO_STRING(g_Domain, pMapName);
	args[1] = MONO_STRING(g_Domain, pMapEntities);
	args[2] = MONO_STRING(g_Domain, pOldLevel);
	args[3] = MONO_STRING(g_Domain, pLandmarkName);
	args[4] = &loadGame;
	args[5] = &background;
	
	MONO_CALL_ARGS(this->m_ClsMain, g_EVT_LevelInit, args);

	return true;
}

void CMonoPlug::Hook_LevelShutdown()
{
	META_LOG(g_PLAPI, "Hook_LevelShutdown()");

	MONO_CALL(this->m_ClsMain, g_EVT_LevelShutdown);
}

const char *CMonoPlug::GetLicense()
{
	return "Licence";
}

const char *CMonoPlug::GetVersion()
{
	return "1.0.0.0";
}

const char *CMonoPlug::GetDate()
{
	return __DATE__;
}

const char *CMonoPlug::GetLogTag()
{
	return "MONOPLUG";
}

const char *CMonoPlug::GetAuthor()
{
	return "MaitreDede";
}

const char *CMonoPlug::GetDescription()
{
	return "Managed plugin wrapper";
}

const char *CMonoPlug::GetName()
{
	return "MonoPlugin";
}

const char *CMonoPlug::GetURL()
{
	return "http://www.sourcemm.net/";
}


MonoConCommand::MonoConCommand(char* name, char* description, MonoDelegate* code, int flags)
: ConCommand(name, (FnCommandCallback_t)NULL, description, flags, (FnCommandCompletionCallback)NULL)
{
	this->m_code = code;
};

void MonoConCommand::Dispatch(const CCommand &command)
{
	void* args[1];
	args[0] = MONO_STRING(g_Domain, command.ArgS());
	MONO_DELEGATE_CALL(this->m_code, args);
};