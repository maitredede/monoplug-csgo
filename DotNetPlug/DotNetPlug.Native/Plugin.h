#ifndef _DOTNETPLUG_PLUGIN_H_
#define _DOTNETPLUG_PLUGIN_H_
#ifdef _WIN32
#pragma once
#endif

#include <ISmmPlugin.h>
#include "Managed.h"
#include <igameevents.h>
#include <iplayerinfo.h>
#include <sh_vector.h>
#include "engine_wrappers.h"
#include "Helpers.h"
#include "AdminInterface.h"

class DotNetPlugPlugin : public ISmmPlugin, public IMetamodListener
{
public:
	bool Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late);
	bool Unload(char *error, size_t maxlen);
	bool Pause(char *error, size_t maxlen);
	bool Unpause(char *error, size_t maxlen);
	void AllPluginsLoaded();
public:
	const char *GetAuthor();
	const char *GetName();
	const char *GetDescription();
	const char *GetURL();
	const char *GetLicense();
	const char *GetVersion();
	const char *GetDate();
	const char *GetLogTag();
public:
	void OnVSPListening(IServerPluginCallbacks *iface);
public:
	int max_players;
	int edictCount;
	ConVar* tv_name;
	ConVar* hostname;
	ConVar* m_varVersion;

public: //Hooks
	bool Hook_LevelInit(const char *pMapName, const char *pMapEntities, const char *pOldLevel, const char *pLandmarkName, bool loadGame, bool background);
	void Hook_ServerActivate(edict_t *pEdictList, int edictCount, int clientMax);
	void Hook_GameFrame(bool simulating);
	void Hook_LevelShutdown();

	void Hook_ClientActive(edict_t *pEntity, bool bLoadGame);
	void Hook_ClientDisconnect(edict_t *pEntity);
	void Hook_ClientPutInServer(edict_t *pEntity, char const *playername);
	void Hook_SetCommandClient(int index);
	void Hook_ClientSettingsChanged(edict_t *pEdict);
	bool Hook_ClientConnect(edict_t *pEntity, const char *pszName, const char *pszAddress, char *reject, int maxrejectlen);
	void Hook_ClientCommand(edict_t *pEntity, const CCommand &args);
	PLUGIN_RESULT Hook_NetworkIDValidated(const char *pszUserName, const char *pszNetworkID);
};

extern DotNetPlugPlugin g_DotNetPlugPlugin;
extern ICvar *icvar;
extern IServerGameDLL *server;
extern IServerGameClients *gameclients;
extern IVEngineServer *engine;
extern IServerPluginHelpers *helpers;
extern IGameEventManager2 *gameevents;
extern IServerPluginCallbacks *vsp_callbacks;
extern IPlayerInfoManager *playerinfomanager;
extern CGlobalVars *gpGlobals;

PLUGIN_GLOBALVARS();

#endif //_DOTNETPLUG_PLUGIN_H_
