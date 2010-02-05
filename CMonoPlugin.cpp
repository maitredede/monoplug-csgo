#include "CMonoPlugin.h"
#include "convar.h"
#include "pluginterfaces.h"

bool MonoPlugin::CMonoPlugin::Less_uint64(const uint64 & k1, const uint64 & k2)
{
	return k1 < k2;
};

bool MonoPlugin::CMonoPlugin::Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late)
{
	//Init vars
	PLUGIN_SAVEVARS();
	this->m_console = NULL;

	//Get engine interfaces
	GET_V_IFACE_ANY(GetServerFactory, g_iserver, IServerGameDLL, INTERFACEVERSION_SERVERGAMEDLL);
	GET_V_IFACE_ANY(GetEngineFactory, g_filesystem, IFileSystem, FILESYSTEM_INTERFACE_VERSION);
	GET_V_IFACE_ANY(GetEngineFactory, g_engine, IVEngineServer, INTERFACEVERSION_VENGINESERVER);
	GET_V_IFACE_ANY(GetEngineFactory, icvar, ICvar, CVAR_INTERFACE_VERSION);
	GET_V_IFACE_CURRENT(GetEngineFactory, g_helpers, IServerPluginHelpers, INTERFACEVERSION_ISERVERPLUGINHELPERS);
	GET_V_IFACE_ANY(GetServerFactory, g_ServerClients, IServerGameClients, INTERFACEVERSION_SERVERGAMECLIENTS);
	GET_V_IFACE_ANY(GetServerFactory, g_PlayerInfoManager, IPlayerInfoManager, INTERFACEVERSION_PLAYERINFOMANAGER);
	GET_V_IFACE_CURRENT(GetEngineFactory, g_GameEventManager, IGameEventManager2, INTERFACEVERSION_GAMEEVENTSMANAGER2);

	gpGlobals = ismm->GetCGlobals();

	/* Load the VSP listener.  This is usually needed for IServerPluginHelpers. */
	if ((g_vsp_callbacks = ismm->GetVSPInfo(NULL)) == NULL)
	{
		ismm->AddListener(this, this);
		ismm->EnableVSPListener();
	}

	//Get game dir
	char dllPath[MAX_PATH];
	char dllDir[MAX_PATH];
	{
		char* dir = new char[MAX_PATH];
		g_engine->GetGameDir(dir, MAX_PATH);
		g_SMAPI->Format(dllDir, sizeof(dllDir), MONOPLUG_DLLPATH, dir); 
		g_SMAPI->Format(dllPath, sizeof(dllPath), MONOPLUG_DLLFILE, dllDir); 
		delete dir;
		META_LOG(g_PLAPI, "MonoPlugin Managed dll file is : %s", dllPath);
		if(!g_filesystem->FileExists(dllPath))
		{
			g_SMAPI->Format(error, maxlen, "Can't find managed DLL : %s", dllPath);
			return false;
		}
	}

	if(!this->InitMono(dllPath, dllDir, error, maxlen))
	{
		return false;
	}

	//if(!this->AddEventListeners(error, maxlen))
	//{
	//	return false;
	//}


	this->m_nextConbaseId = 1;
	this->m_conbase = new CUtlMap<uint64, ConCommandBase*>();
	this->m_conbase->SetLessFunc(CMonoPlugin::Less_uint64);

	this->AddHooks();
#if SOURCE_ENGINE >= SE_ORANGEBOX
	g_pCVar = icvar;
	ConVar_Register(0, &s_BaseAccessor);
#else
	ConCommandBaseMgr::OneTimeInit(&s_BaseAccessor);
#endif

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
	CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_AllPluginsLoaded, NULL);
}
