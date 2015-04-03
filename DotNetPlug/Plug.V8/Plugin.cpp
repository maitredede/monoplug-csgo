#include <stdio.h>
#include <edict.h>
#include <iplayerinfo.h>
#include "Plugin.h"
#include "Helpers.h"

#include <include/v8.h>
#include <include/libplatform/libplatform.h>

using namespace v8;

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

IServerGameDLL *server = NULL;
IServerGameClients *gameclients = NULL;
IVEngineServer *engine = NULL;
IServerPluginHelpers *helpers = NULL;
IGameEventManager2 *gameevents = NULL;
IServerPluginCallbacks *vsp_callbacks = NULL;
IPlayerInfoManager *playerinfomanager = NULL;
ICvar *icvar = NULL;
CGlobalVars *gpGlobals = NULL;

V8Plugin g_V8Plugin;

PLUGIN_EXPOSE(V8Plugin, g_V8Plugin);

bool V8Plugin::Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late)
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

		SH_ADD_HOOK_MEMFUNC(IServerGameDLL, LevelInit, server, this, &V8Plugin::Hook_LevelInit, false);
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, server, this, &V8Plugin::Hook_ServerActivate, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, server, this, &V8Plugin::Hook_GameFrame, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, LevelShutdown, server, this, &V8Plugin::Hook_LevelShutdown, false);

	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientActive, gameclients, this, &V8Plugin::Hook_ClientActive, false);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, gameclients, this, &V8Plugin::Hook_ClientDisconnect, false);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, gameclients, this, &V8Plugin::Hook_ClientPutInServer, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, SetCommandClient, gameclients, this, &V8Plugin::Hook_SetCommandClient, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientSettingsChanged, gameclients, this, &V8Plugin::Hook_ClientSettingsChanged, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientConnect, gameclients, this, &V8Plugin::Hook_ClientConnect, false);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientCommand, gameclients, this, &V8Plugin::Hook_ClientCommand, false);

	this->tv_name = icvar->FindVar("tv_name");

	//Init v8
	V8::InitializeICU();
	this->m_v8_platform = platform::CreateDefaultPlatform();
	V8::InitializePlatform(this->m_v8_platform);
	V8::Initialize();
	
	return true;
}

bool V8Plugin::Unload(char *error, size_t maxlen)
{
	V8::Dispose();
	V8::ShutdownPlatform();
	delete this->m_v8_platform;

	SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, LevelInit, server, this, &V8Plugin::Hook_LevelInit, false);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, server, this, &V8Plugin::Hook_ServerActivate, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, GameFrame, server, this, &V8Plugin::Hook_GameFrame, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, LevelShutdown, server, this, &V8Plugin::Hook_LevelShutdown, false);

	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientActive, gameclients, this, &V8Plugin::Hook_ClientActive, false);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, gameclients, this, &V8Plugin::Hook_ClientDisconnect, false);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, gameclients, this, &V8Plugin::Hook_ClientPutInServer, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, SetCommandClient, gameclients, this, &V8Plugin::Hook_SetCommandClient, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientSettingsChanged, gameclients, this, &V8Plugin::Hook_ClientSettingsChanged, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientConnect, gameclients, this, &V8Plugin::Hook_ClientConnect, false);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientCommand, gameclients, this, &V8Plugin::Hook_ClientCommand, false);

	return true;
}

void V8Plugin::AllPluginsLoaded()
{
	/* This is where we'd do stuff that relies on the mod or other plugins
	* being initialized (for example, cvars added and events registered).
	*/
}

bool V8Plugin::Pause(char *error, size_t maxlen)
{
	return false;
}

bool V8Plugin::Unpause(char *error, size_t maxlen)
{
	return false;
}

const char *V8Plugin::GetLicense()
{
	return "No licence yet";
}

const char *V8Plugin::GetVersion()
{
	return "0.1.0.0";
}

const char *V8Plugin::GetDate()
{
	return __DATE__;
}

const char *V8Plugin::GetLogTag()
{
	return "V8";
}

const char *V8Plugin::GetAuthor()
{
	return "MaitreDede";
}

const char *V8Plugin::GetDescription()
{
	return "Javascript V8 plugin engine";
}

const char *V8Plugin::GetName()
{
	return "PlugV8";
}

const char *V8Plugin::GetURL()
{
	return "http://www.sourcemm.net/";
}