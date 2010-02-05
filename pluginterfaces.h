#ifndef _GAMEINTERFACES_H_
#define _GAMEINTERFACES_H_

#include "monoBase.h"
#include <igameevents.h>
#include <iplayerinfo.h>
#include <filesystem.h>

//Game
extern IServerGameDLL *g_iserver;
extern IVEngineServer *g_engine;
extern IFileSystem *g_filesystem;
extern IServerGameClients *g_ServerClients;

//Mono
extern MonoDomain *g_Domain;

extern CGlobalVars *gpGlobals;
extern ICvar *icvar;
extern IServerPluginHelpers *g_helpers;
extern IServerPluginCallbacks *g_vsp_callbacks;
extern IPlayerInfoManager *g_PlayerInfoManager;
extern IGameEventManager2 *g_GameEventManager;

#endif //_GAMEINTERFACES_H_
