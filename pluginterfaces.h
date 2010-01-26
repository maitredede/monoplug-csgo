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

//Mono
extern MonoDomain *g_Domain;

extern CGlobalVars *gpGlobals;
extern ICvar *icvar;
extern IServerPluginCallbacks *vsp_callbacks;
extern IServerPluginHelpers *g_Helpers;

#endif //_GAMEINTERFACES_H_
