#include <stdio.h>
#include "Plugin.h"

SH_DECL_HOOK3_void(IServerGameDLL, ServerActivate, SH_NOATTRIB, 0, edict_t *, int, int);
SH_DECL_HOOK1_void(IServerGameDLL, GameFrame, SH_NOATTRIB, 0, bool);

DotNetPlugPlugin g_DotNetPlugPlugin;
IServerGameDLL *server = NULL;
IServerGameClients *gameclients = NULL;
IVEngineServer *engine = NULL;
IServerPluginHelpers *helpers = NULL;
IGameEventManager2 *gameevents = NULL;
IServerPluginCallbacks *vsp_callbacks = NULL;
IPlayerInfoManager *playerinfomanager = NULL;
ICvar *icvar = NULL;
CGlobalVars *gpGlobals = NULL;

PLUGIN_EXPOSE(DotNetPlugPlugin, g_DotNetPlugPlugin);
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

	META_LOG(g_PLAPI, "GetBaseDir() %s\n", mm_path);
	META_CONPRINTF("GetBaseDir() %s\n", mm_path);

	//Init DotNet
	if (!g_Managed.Init(mm_path)){
		//strcpy_s(error, maxlen, "Error with DotNet init");
		snprintf(error, maxlen, "Error with DotNet init\n");
		return false;
	}


	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, server, this, &DotNetPlugPlugin::Hook_ServerActivate, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, server, this, &DotNetPlugPlugin::Hook_GameFrame, true);

	return true;
}

bool DotNetPlugPlugin::Unload(char *error, size_t maxlen)
{
	SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, GameFrame, server, this, &DotNetPlugPlugin::Hook_GameFrame, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, server, this, &DotNetPlugPlugin::Hook_ServerActivate, true);

	g_Managed.Unload();

	return true;
}

void DotNetPlugPlugin::Hook_ServerActivate(edict_t *pEdictList, int edictCount, int clientMax)
{
	META_LOG(g_PLAPI, "ServerActivate() called: edictCount = %d, clientMax = %d", edictCount, clientMax);
}

void DotNetPlugPlugin::Hook_GameFrame(bool simulating)
{
	/**
	* simulating:
	* ***********
	* true  | game is ticking
	* false | game is not ticking
	*/
	if (simulating)
		g_Managed.Tick();
}

void DotNetPlugPlugin::AllPluginsLoaded()
{
	/* This is where we'd do stuff that relies on the mod or other plugins
	* being initialized (for example, cvars added and events registered).
	*/
	g_Managed.AllPluginsLoaded();
}

bool DotNetPlugPlugin::Pause(char *error, size_t maxlen)
{
	return true;
}

bool DotNetPlugPlugin::Unpause(char *error, size_t maxlen)
{
	return true;
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
	return "Managed plugin";
}

const char *DotNetPlugPlugin::GetName()
{
	return "DotNetPlugPlugin";
}

const char *DotNetPlugPlugin::GetURL()
{
	return "http://www.sourcemm.net/";
}
