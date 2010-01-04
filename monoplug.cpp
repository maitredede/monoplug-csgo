#include "monoplug.h"

SH_DECL_HOOK1_void(IServerGameDLL, GameFrame, SH_NOATTRIB, 0, bool);

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

MonoMethod* g_ClsMain_Init = NULL;
MonoMethod* g_ClsMain_Shutdown = NULL;

MonoMethod* g_EVT_GameFrame = NULL;
MonoMethod* g_EVT_ConVarStringValueChanged = NULL;

MONO_EVENT_DECL_HOOK0_VOID(IServerGameDLL, LevelShutdown, SH_NOATTRIB, 0, server, false);

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

static uint64 Mono_RegisterConVarString(MonoString* name, MonoString* description, int flags, MonoString* defaultValue);
static void Mono_UnregisterConVarString(uint64 nativeID);
static bool Mono_RegisterConCommand(MonoString* name, MonoString* description, MonoDelegate* code, int flags, MonoDelegate* complete);
static bool Mono_UnregisterConCommand(MonoString* name);

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
//#ifdef _DEBUG
//	META_CONPRINT("Entering Mono_SetConVarStringValue\n");
//#endif

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
		ATTACH(MONOPLUG_NATMAN_INIT, g_ClsMain_Init, g_Image);
		ATTACH(MONOPLUG_NATMAN_SHUTDOWN, g_ClsMain_Shutdown, g_Image);
		
		//Events
		ATTACH(MONOPLUG_NATMAN_GAMEFRAME, g_EVT_GameFrame, g_Image);
		ATTACH(MONOPLUG_NATMAN_CONVARSTRING_VALUECHANGED, g_EVT_ConVarStringValueChanged, g_Image);
	}

	MONO_EVENT_INIT_HOOK0_VOID(LevelShutdown, "MonoPlug.ClsMain:Raise_LevelShutdown()", "MonoPlug.ClsMain::Attach_LevelShutdown", "MonoPlug.ClsMain::Dettach_LevelShutdown");
	
	//ConCommandBase init code
	this->m_convarStringIdValue = 0;
	this->m_conCommands = new CUtlVector<MonoConCommand*>();
	this->m_convarStringId = new CUtlVector<uint64Container*>();
	this->m_convarStringPtr = new CUtlVector<ConVar*>();
	
	//Create object instance
	this->m_ClsMain = mono_object_new(g_Domain, g_Class);
	mono_runtime_object_init(this->m_ClsMain);

	//Init object
	MONO_CALL(this->m_ClsMain, g_ClsMain_Init);
	META_CONPRINT("MP: Init OK\n");

	//Final hooks	
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, server, this, &CMonoPlug::Hook_GameFrame, true);

	META_CONPRINT("MP: Hooks OK\n");
	ENGINE_CALL(LogPrint)("All hooks started!\n");

#if SOURCE_ENGINE >= SE_ORANGEBOX
	g_pCVar = icvar;
	ConVar_Register(0, &s_BaseAccessor);
#else
	ConCommandBaseMgr::OneTimeInit(&s_BaseAccessor);
#endif

	//From SourceMod ChatTriggers
	m_pSayCmd = icvar->FindCommand("say");
	m_pSayTeamCmd = icvar->FindCommand("say_team");

	return true;
}

bool CMonoPlug::Unload(char *error, size_t maxlen)
{
	//Mono is leaked, so trying to unload plugin lead to segfault
	Q_snprintf(error, maxlen, "Unload denied to avoid segfault caused by Mono.");
	return false;

	//TODO : Clean remove hooks and hacks
}

void CMonoPlug::OnVSPListening(IServerPluginCallbacks *iface)
{
	vsp_callbacks = iface;
}

void CMonoPlug::Hook_GameFrame(bool simulating)
{
	/**
	 * simulating:
	 * ***********
	 * true  | game is ticking
	 * false | game is not ticking
	 */
	MONO_CALL(this->m_ClsMain, g_EVT_GameFrame);
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


MonoConCommand::MonoConCommand(char* name, char* description, MonoDelegate* code, int flags,MonoDelegate* complete)
: ConCommand(name, (FnCommandCallback_t)NULL, description, flags, (FnCommandCompletionCallback)NULL)
{
	this->m_code = code;
	this->m_complete = complete;
};

void MonoConCommand::Dispatch(const CCommand &command)
{
	void* args[1];
	args[0] = MONO_STRING(g_Domain, command.ArgS());
	META_CONPRINTF("Dispatch command : %s\n", command.GetCommandString());
	MONO_DELEGATE_CALL(this->m_code, args);
};

int MonoConCommand::AutoCompleteSuggest( const char *partial, CUtlVector< CUtlString > &commands )
{
	void* args[1];
	args[0] = MONO_STRING(g_Domain, partial);
	MonoObject* ret = MONO_DELEGATE_CALL(this->m_complete, args);
	if(ret)
	{
		MonoArray* arr = (MonoArray*)ret;
		for(uint i=0;i<mono_array_length(arr);i++)
		{
			MonoString* str = mono_array_get(arr, MonoString*, i);
			commands.AddToTail(CUtlString(mono_string_to_utf8(str)));
		}
		return mono_array_length(arr);
	}
	else
	{
		return 0;
	}
};

bool MonoConCommand::CanAutoComplete()
{
	if(this->m_complete)
	{
		return true;
	}
	else
	{
		return false;
	}
};

uint64 Mono_RegisterConVarString(MonoString* name, MonoString* description, int flags, MonoString* defaultValue)
{
	uint64 nativeID = ++g_MonoPlugPlugin.m_convarStringIdValue;
	ConVar* var = new ConVar(
		mono_string_to_utf8(name),
		mono_string_to_utf8(defaultValue),
		flags,
		mono_string_to_utf8(description),
		ConVarStringChangeCallback
		);
	g_MonoPlugPlugin.m_convarStringId->AddToTail(new uint64Container(nativeID));
	g_MonoPlugPlugin.m_convarStringPtr->AddToTail(var);
	return nativeID;
};

void Mono_UnregisterConVarString(uint64 nativeID)
{
	for(int i=0;i<g_MonoPlugPlugin.m_convarStringId->Count(); i++)
	{
		uint64Container* cont = g_MonoPlugPlugin.m_convarStringId->Element(i);
		if(cont->Value() == nativeID)
		{
			ConVar* var = g_MonoPlugPlugin.m_convarStringPtr->Element(i);
			META_UNREGCVAR(var);
			g_MonoPlugPlugin.m_convarStringId->Remove(i);
			g_MonoPlugPlugin.m_convarStringPtr->Remove(i);
			delete var;
			break;
		}
	}

};

bool Mono_RegisterConCommand(MonoString* name, MonoString* description, MonoDelegate* code, int flags, MonoDelegate* complete)
{
	MonoConCommand* com = new MonoConCommand(mono_string_to_utf8(name), mono_string_to_utf8(description), code, flags, complete);

	g_MonoPlugPlugin.m_conCommands->AddToTail(com);
	return true;
};

bool Mono_UnregisterConCommand(MonoString* name)
{
	const char* s_name = mono_string_to_utf8(name);

	for(int i = 0; i < 	g_MonoPlugPlugin.m_conCommands->Count(); i++)
	{
		MonoConCommand* item = 	g_MonoPlugPlugin.m_conCommands->Element(i);

		if(Q_strcmp(item->GetName(), s_name) == 0)
		{
			//g_pCVar->UnregisterConCommand(item);
			META_UNREGCVAR(item);
			g_MonoPlugPlugin.m_conCommands->Remove(i);
			delete item;
			return true;
		}
	}

	META_CONPRINTF("Mono_UnregisterConCommand : Command NOT found\n");
	return false;
};

void CMonoPlug::FireGameEvent( IGameEvent *event )
{
	if (!event)
		return;

	const char *name = event->GetName();

	META_LOG(g_PLAPI, "event '%s' received\n", name);
	META_CONPRINTF("event '%s' received\n", name);
}

MonoObject* MONO_CALL_ARGS(void* target, MonoMethod* methodHandle, void** args)
{
	MonoObject* exception = NULL;
	MonoObject* ret = mono_runtime_invoke(methodHandle, target, args, &exception);
	if(exception)
	{
		mono_print_unhandled_exception(exception);
		return NULL;
	}
	else
	{
		return ret;
	}
};

MonoObject* MONO_DELEGATE_CALL(MonoDelegate* delegateObject, void** args)
{
	//Code from : http://www.mail-archive.com/mono-list@lists.ximian.com/msg26230.html
	//MonoClass* dlgClass = mono_object_get_class((MonoObject*)delegateObject);
	//MonoMethod* dlgMethod = mono_get_delegate_invoke(dlgClass);
	//return MONO_CALL_ARGS(delegateObject, dlgMethod, args);
	MonoObject* exception = NULL;
	MonoObject* ret = mono_runtime_delegate_invoke((MonoObject*)delegateObject, args, &exception);
	if(exception)
	{
		mono_print_unhandled_exception(exception);
		return NULL;
	}
	else
	{
		return ret;
	}
}

