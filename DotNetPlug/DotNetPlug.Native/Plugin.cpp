#include <stdio.h>
#include "Plugin.h"
#include <iplayerinfo.h>
#include "UserTracker.h"

SH_DECL_HOOK6(IServerGameDLL, LevelInit, SH_NOATTRIB, 0, bool, char const *, char const *, char const *, char const *, bool, bool);
SH_DECL_HOOK3_void(IServerGameDLL, ServerActivate, SH_NOATTRIB, 0, edict_t *, int, int);
SH_DECL_HOOK1_void(IServerGameDLL, GameFrame, SH_NOATTRIB, 0, bool);
SH_DECL_HOOK0_void(IServerGameDLL, LevelShutdown, SH_NOATTRIB, 0);

SH_DECL_HOOK2_void(IServerGameClients, ClientActive, SH_NOATTRIB, 0, edict_t *, bool);
SH_DECL_HOOK1_void(IServerGameClients, ClientDisconnect, SH_NOATTRIB, 0, edict_t *);
SH_DECL_HOOK2_void(IServerGameClients, ClientPutInServer, SH_NOATTRIB, 0, edict_t *, char const *);
SH_DECL_HOOK1_void(IServerGameClients, SetCommandClient, SH_NOATTRIB, 0, int);
SH_DECL_HOOK1_void(IServerGameClients, ClientSettingsChanged, SH_NOATTRIB, 0, edict_t *);
SH_DECL_HOOK5(IServerGameClients, ClientConnect, SH_NOATTRIB, 0, bool, edict_t *, const char*, const char *, char *, int);
SH_DECL_HOOK2_void(IServerGameClients, ClientCommand, SH_NOATTRIB, 0, edict_t *, const CCommand &);

SH_DECL_HOOK2(IServerPluginCallbacks, NetworkIDValidated, SH_NOATTRIB, 0, PLUGIN_RESULT, const char *, const char *);

IServerGameDLL *server = NULL;
IServerGameClients *gameclients = NULL;
IVEngineServer *engine = NULL;
IServerPluginHelpers *helpers = NULL;
IGameEventManager2 *gameevents = NULL;
IServerPluginCallbacks *vsp_callbacks = NULL;
IPlayerInfoManager *playerinfomanager = NULL;
ICvar *icvar = NULL;
CGlobalVars *gpGlobals = NULL;

DotNetPlugPlugin g_DotNetPlugPlugin;
Managed g_Managed;
UserTracker gUserTracker;

PLUGIN_EXPOSE(DotNetPlugPlugin, g_DotNetPlugPlugin);

void DotNetPlugPlugin::OnVSPListening(IServerPluginCallbacks *iface)
{
	SH_ADD_HOOK_MEMFUNC(IServerPluginCallbacks, NetworkIDValidated, iface, &g_DotNetPlugPlugin, &DotNetPlugPlugin::Hook_NetworkIDValidated, false);
	vsp_callbacks = iface;
}

void DotPlugVersionCallback(IConVar *var, const char *pOldString, float flOldFloat)
{
	if (!FStrEq(pOldString, g_DotNetPlugPlugin.GetVersion()))
	{
		g_DotNetPlugPlugin.m_varVersion->SetValue(g_DotNetPlugPlugin.GetVersion());
	}
}


bool DotNetPlugPlugin::Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late)
{
	PLUGIN_SAVEVARS();

	// Get engine interfaces
	GET_V_IFACE_CURRENT(GetEngineFactory, engine, IVEngineServer, INTERFACEVERSION_VENGINESERVER);
	GET_V_IFACE_CURRENT(GetEngineFactory, gameevents, IGameEventManager2, INTERFACEVERSION_GAMEEVENTSMANAGER2);
	GET_V_IFACE_CURRENT(GetEngineFactory, helpers, IServerPluginHelpers, INTERFACEVERSION_ISERVERPLUGINHELPERS);
	GET_V_IFACE_CURRENT(GetEngineFactory, icvar, ICvar, CVAR_INTERFACE_VERSION);
	GET_V_IFACE_ANY(GetServerFactory, server, IServerGameDLL, INTERFACEVERSION_SERVERGAMEDLL);
	GET_V_IFACE_ANY(GetServerFactory, gameclients, IServerGameClients, INTERFACEVERSION_SERVERGAMECLIENTS);
	GET_V_IFACE_ANY(GetServerFactory, playerinfomanager, IPlayerInfoManager, INTERFACEVERSION_PLAYERINFOMANAGER);
	gpGlobals = g_SMAPI->GetCGlobals();

	if ((vsp_callbacks = ismm->GetVSPInfo(NULL)) == NULL)
	{
		ismm->EnableVSPListener();
		ismm->AddListener(this, this);
	}

	//Gather init data
	char mm_path[MAX_PATH];
	ZeroMemory(&mm_path, MAX_PATH);
	const char* sBaseDir = g_SMAPI->GetBaseDir();

	ConCommandBase* mm_basedirBase = icvar->FindCommandBase("mm_basedir");
	if (mm_basedirBase->IsCommand())
	{
		snprintf(error, maxlen, "mm_basedir is not a convar\n");
		return false;
	}
	ConVar* mm_basedirVar = (ConVar*)mm_basedirBase;
	const char* mm_basedir = mm_basedirVar->GetString();

	V_ComposeFileName(sBaseDir, mm_basedir, mm_path, MAX_PATH);

	//Init DotNet
	if (!g_Managed.Init(mm_path)){
		//strcpy_s(error, maxlen, "Error with DotNet init");
		snprintf(error, maxlen, "Error with DotNet init\n");
		return false;
	}

	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, LevelInit, server, this, &DotNetPlugPlugin::Hook_LevelInit, false);
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, server, this, &DotNetPlugPlugin::Hook_ServerActivate, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, server, this, &DotNetPlugPlugin::Hook_GameFrame, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, LevelShutdown, server, this, &DotNetPlugPlugin::Hook_LevelShutdown, false);

	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientActive, gameclients, this, &DotNetPlugPlugin::Hook_ClientActive, false);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, gameclients, this, &DotNetPlugPlugin::Hook_ClientDisconnect, false);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, gameclients, this, &DotNetPlugPlugin::Hook_ClientPutInServer, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, SetCommandClient, gameclients, this, &DotNetPlugPlugin::Hook_SetCommandClient, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientSettingsChanged, gameclients, this, &DotNetPlugPlugin::Hook_ClientSettingsChanged, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientConnect, gameclients, this, &DotNetPlugPlugin::Hook_ClientConnect, false);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientCommand, gameclients, this, &DotNetPlugPlugin::Hook_ClientCommand, false);

	this->tv_name = icvar->FindVar("tv_name");
	gUserTracker.Load();
	
	//Load Native interop
	g_Managed.Load();

	gameevents->AddListener(g_Managed.EVT_player_death, "player_death", true);

	this->m_varVersion = new ConVar("dotplug_version", this->GetVersion(), FCVAR_REPLICATED | FCVAR_NOTIFY, "Plugin version of DotPlug engine", &DotPlugVersionCallback);
	META_REGCVAR(this->m_varVersion);

	return true;
}

bool DotNetPlugPlugin::Unload(char *error, size_t maxlen)
{
	META_UNREGCVAR(this->m_varVersion);

	gUserTracker.Unload();
	g_Managed.Unload();

	SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, LevelInit, server, this, &DotNetPlugPlugin::Hook_LevelInit, false);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, server, this, &DotNetPlugPlugin::Hook_ServerActivate, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, GameFrame, server, this, &DotNetPlugPlugin::Hook_GameFrame, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, LevelShutdown, server, this, &DotNetPlugPlugin::Hook_LevelShutdown, false);

	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientActive, gameclients, this, &DotNetPlugPlugin::Hook_ClientActive, false);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, gameclients, this, &DotNetPlugPlugin::Hook_ClientDisconnect, false);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, gameclients, this, &DotNetPlugPlugin::Hook_ClientPutInServer, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, SetCommandClient, gameclients, this, &DotNetPlugPlugin::Hook_SetCommandClient, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientSettingsChanged, gameclients, this, &DotNetPlugPlugin::Hook_ClientSettingsChanged, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientConnect, gameclients, this, &DotNetPlugPlugin::Hook_ClientConnect, false);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientCommand, gameclients, this, &DotNetPlugPlugin::Hook_ClientCommand, false);

	return true;
}

void DotNetPlugPlugin::AllPluginsLoaded()
{
	/* This is where we'd do stuff that relies on the mod or other plugins
	* being initialized (for example, cvars added and events registered).
	*/
	//Plugin architecture ready, execute plugin server script
	engine->ServerCommand("exec dotnetplug_server.cfg\n");
	engine->ServerExecute();
}

bool DotNetPlugPlugin::Pause(char *error, size_t maxlen)
{
	return false;
}

bool DotNetPlugPlugin::Unpause(char *error, size_t maxlen)
{
	return false;
}

const char *DotNetPlugPlugin::GetLicense()
{
	return "No licence yet";
}

const char *DotNetPlugPlugin::GetVersion()
{
	return "0.1.0.0";
}

const char *DotNetPlugPlugin::GetDate()
{
	return __DATE__;
}

const char *DotNetPlugPlugin::GetLogTag()
{
	return "DOTNET";
}

const char *DotNetPlugPlugin::GetAuthor()
{
	return "MaitreDede";
}

const char *DotNetPlugPlugin::GetDescription()
{
#ifdef MANAGED_WIN32
	return "Managed plugin - Win32";
#else 
#ifdef MANAGED_MONO
	return "Managed plugin - Mono";
#else
	return "Managed plugin - Unkown plateform";
#endif
#endif
}

const char *DotNetPlugPlugin::GetName()
{
	return "DotNetPlugPlugin";
}

const char *DotNetPlugPlugin::GetURL()
{
	return "http://www.sourcemm.net/";
}