#include <stdio.h>
#include "monoplug.h"

SH_DECL_HOOK3_void(IServerGameDLL, ServerActivate, SH_NOATTRIB, 0, edict_t *, int, int);
SH_DECL_HOOK1_void(IServerGameDLL, GameFrame, SH_NOATTRIB, 0, bool);

SH_DECL_HOOK6(IServerGameDLL, LevelInit, SH_NOATTRIB, 0, bool, char const *, char const *, char const *, char const *, bool, bool);
SH_DECL_HOOK0_void(IServerGameDLL, LevelShutdown, SH_NOATTRIB, 0);

CMonoPlug g_MonoPlugPlugin;

IServerGameDLL *server = NULL;
IVEngineServer *engine = NULL;
IFileSystem *filesystem = NULL;
ICvar *icvar = NULL;
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

PLUGIN_EXPOSE(CMonoPlug, g_MonoPlugPlugin);

//---------------------------------------------------------------------------------
// Purpose: constructor/destructor
//---------------------------------------------------------------------------------
//CMonoPlug::CMonoPlug()
//{
//	this->m_ClrDomain = NULL;
//	this->m_Assembly = NULL;
//	this->m_Image = NULL;
//	this->m_Class = NULL;
//	this->m_ClsMain = NULL;
//
//
//	this->m_HandleMessage = NULL;
//}

bool CMonoPlug::Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late)
{
	//META_LOG(g_PLAPI, "Init Plugin.");
	
	PLUGIN_SAVEVARS();

	GET_V_IFACE_ANY(GetServerFactory, server, IServerGameDLL, INTERFACEVERSION_SERVERGAMEDLL);
	GET_V_IFACE_ANY(GetEngineFactory, filesystem, IFileSystem, FILESYSTEM_INTERFACE_VERSION);
	GET_V_IFACE_CURRENT(GetEngineFactory, engine, IVEngineServer, INTERFACEVERSION_VENGINESERVER);

	gpGlobals = ismm->GetCGlobals();

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
		mono_config_parse(NULL);
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
		MonoClass* cls_delegate = mono_class_from_name(g_Image, MONOPLUG_NAMESPACE, "ConCommandDelegate");
		if(cls_delegate)
		{
			META_CONPRINT("Found delegate class.\n");
		}
		else
		{
			META_CONPRINT("Found delegate class.\n");
		}

		//Add internal calls from managed to native
		mono_add_internal_call (MONOPLUG_CALLBACK_MSG, (void*)Mono_Msg);
		mono_add_internal_call (MONOPLUG_CALLBACK_REGISTERCONCOMMAND, (void*)Mono_RegisterConCommand);
		mono_add_internal_call (MONOPLUG_CALLBACK_UNREGISTERCONCOMMAND, (void*)Mono_UnregisterConCommand);

		//Get callbacks from native to managed
		ATTACH(MONOPLUG_NATMAN_INIT, g_Init, g_Image);
		ATTACH(MONOPLUG_NATMAN_SHUTDOWN, g_Shutdown, g_Image);
		ATTACH(MONOPLUG_NATMAN_HANDLEMESSAGE, g_HandleMessage, g_Image);
		
		//Events
		ATTACH(MONOPLUG_CLSMAIN_EVT_LEVELINIT, g_EVT_LevelInit, g_Image);
		ATTACH(MONOPLUG_CLSMAIN_EVT_LEVELSHUTDOWN, g_EVT_LevelShutdown, g_Image);
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

	this->m_conCommands = new CUtlVector<MonoConCommand*>();
	
	//Create object instance
	this->m_ClsMain = mono_object_new(g_Domain, g_Class);
	mono_runtime_object_init(this->m_ClsMain);
	
	MONO_CALL(this->m_ClsMain, g_Init);
	
	//Final hooks	
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, server, this, &CMonoPlug::Hook_ServerActivate, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, server, this, &CMonoPlug::Hook_GameFrame, true);

	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, LevelInit, server, this, &CMonoPlug::Hook_LevelInit, true);
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
	 MONO_CALL(this->m_ClsMain, g_HandleMessage);
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

MonoConCommand::MonoConCommand(char* name, char* description, CCode* code)
: ConCommand(name, (FnCommandCallback_t)NULL, description)
{
	META_CONPRINT("Entering : MonoConCommand::MonoConCommand\n");
	this->m_code = code;
}

void MonoConCommand::Dispatch(const CCommand &command)
{
	META_CONPRINT("Entering : MonoConCommand::mono_callback\n");

	void* args[1];
	args[0] = MONO_STRING(g_Domain, command.ArgS());
	META_CONPRINTF("Calling delegate of command %s\n", this->GetName());
	MONO_DELEGATE_CALL(this->m_code, args);
	META_CONPRINTF("Called delegate of command %s\n", this->GetName());
	//MonoClass* dlgClass = mono_class_from_name(g_Image, MONOPLUG_NAMESPACE, "ConCommandDelegate");
	//if(dlgClass)
	//{

		//mono_runtime_delegate_invoke(this->code, args, 
		//MonoMethodDesc* desc = mono_method_desc_new("MonoPlug.ConCommandDelegate:Invoke(string)", true);
		//if(desc)
		//{
		//	MonoMethod* dlgInvoke = mono_method_desc_search_in_image(desc, g_Image);
		//	if(dlgInvoke)
		//	{
		//	}
		//	else
		//	{
		//		META_CONPRINTF("Can't get ConCommandDelegate:Invoke(string) method handle\n");
		//	}
		//	mono_method_desc_free(desc);
		//}
		//else
		//{
		//	META_CONPRINTF("Can't get ConCommandDelegate:Invoke(string) method description\n");
		//}
	//}
	//else
	//{
	//	META_CONPRINT("Can't get ConCommandDelegate class\n");
	//}
}
