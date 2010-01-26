#include "CMonoPlugin.h"
#include "convar.h"

//Hooks declaration
SH_DECL_HOOK3_void(IServerGameDLL, ServerActivate, SH_NOATTRIB, 0, edict_t *, int, int);

void Test_Change(IConVar *var, const char *pOldValue, float flOldValue)
{
	META_CONPRINTF("Var has changed : %s\n", var->GetName());
}

//ConVar clr_test("clr_test", "42", FCVAR_CHEAT, "Help test", Test_Change);

ICvar* MonoPlugin::CMonoPlugin::GetICVar()
{
#if SOURCE_ENGINE >= SE_ORANGEBOX
	return (ICvar *)((g_SMAPI->GetEngineFactory())(CVAR_INTERFACE_VERSION, NULL));
#else
	return (ICvar *)((g_SMAPI->GetEngineFactory())(VENGINE_CVAR_INTERFACE_VERSION, NULL));
#endif
}

bool MonoPlugin::CMonoPlugin::Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late)
{
	PLUGIN_SAVEVARS();

	//Get engine interfaces
	GET_V_IFACE_ANY(GetServerFactory, g_iserver, IServerGameDLL, INTERFACEVERSION_SERVERGAMEDLL);
	GET_V_IFACE_ANY(GetEngineFactory, g_filesystem, IFileSystem, FILESYSTEM_INTERFACE_VERSION);
	GET_V_IFACE_ANY(GetEngineFactory, g_engine, IVEngineServer, INTERFACEVERSION_VENGINESERVER);
	GET_V_IFACE_ANY(GetEngineFactory, icvar, ICvar, CVAR_INTERFACE_VERSION);
	GET_V_IFACE_CURRENT(GetEngineFactory, g_Helpers, IServerPluginHelpers, INTERFACEVERSION_ISERVERPLUGINHELPERS);

	gpGlobals = ismm->GetCGlobals();

	/* Load the VSP listener.  This is usually needed for IServerPluginHelpers. */
	if ((vsp_callbacks = ismm->GetVSPInfo(NULL)) == NULL)
	{
		ismm->AddListener(this, this);
		ismm->EnableVSPListener();
	}

	this->AddHooks();

#if SOURCE_ENGINE >= SE_ORANGEBOX
	g_pCVar = this->GetICVar();
	ConVar_Register(0, &s_BaseAccessor);
#else
	ConCommandBaseMgr::OneTimeInit(&s_BaseAccessor);
#endif

	//Get game dir
	char dllPath[MAX_PATH];
	{
		char* dir = new char[MAX_PATH];
		g_engine->GetGameDir(dir, MAX_PATH);
		g_SMAPI->Format(dllPath, sizeof(dllPath), MONOPLUG_DLLFILE, dir); 
		delete dir;
		META_LOG(g_PLAPI, "MonoPlugin Managed dll file is : %s", dllPath);
		if(!g_filesystem->FileExists(dllPath))
		{
			g_SMAPI->Format(error, maxlen, "Can't find managed DLL : %s", dllPath);
			return false;
		}
	}

	if(!this->InitMono(dllPath, error, maxlen))
	{
		return false;
	}

	this->m_test = new ConVar("clr_test", "42", FCVAR_CHEAT, "Help test", Test_Change);
	if(!META_REGCVAR(this->m_test))
	{
		META_LOG(g_PLAPI, "Can't register test var");
	}

	if(!this->StartMono(error, maxlen))
	{
		return false;
	}

	//All done
	return true;
}

bool MonoPlugin::CMonoPlugin::Unload(char *error, size_t maxlen)
{
	g_SMAPI->Format(error, maxlen, "Plugin unloading denied : Mono unloading is impossible, thus leading to crashes...");
	return false;

	//SH_REMOVE_HOOK_STATICFUNC(IServerGameDLL, ServerActivate, g_iserver, Hook_ServerActivate, true);
	//return true;
}

void MonoPlugin::CMonoPlugin::AllPluginsLoaded()
{
	/* This is where we'd do stuff that relies on the mod or other plugins
	 * being initialized (for example, cvars added and events registered).
	 */
}
