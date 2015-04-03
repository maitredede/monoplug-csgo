#include <stdio.h>
#include <edict.h>
#include <iplayerinfo.h>
#include "Plugin.h"
#include "Helpers.h"

#include <include/v8.h>
#include <include/libplatform/libplatform.h>

using namespace v8;

bool V8Plugin::Hook_LevelInit(const char *pMapName, const char *pMapEntities, const char *pOldLevel, const char *pLandmarkName, bool loadGame, bool background)
{
	this->hostname = icvar->FindVar("hostname");
	this->tv_name = icvar->FindVar("tv_name");

	RETURN_META_VALUE_NEWPARAMS(MRES_IGNORED, true, &IServerGameDLL::LevelInit, (pMapName, pMapEntities, pOldLevel, pLandmarkName, loadGame, background));
}

void V8Plugin::Hook_ServerActivate(edict_t *pEdictList, int edictCount, int clientMax)
{
	this->max_players = clientMax;
}

void V8Plugin::Hook_GameFrame(bool simulating)
{
	/**
	* simulating:
	* ***********
	* true  | game is ticking
	* false | game is not ticking
	*/
}

void V8Plugin::Hook_LevelShutdown()
{
}

void V8Plugin::Hook_ClientActive(edict_t *pEntity, bool bLoadGame)
{
}

void V8Plugin::Hook_ClientDisconnect(edict_t *pEntity)
{
}

void V8Plugin::Hook_ClientPutInServer(edict_t *pEntity, char const *playername)
{
}

void V8Plugin::Hook_SetCommandClient(int index)
{
}

void V8Plugin::Hook_ClientSettingsChanged(edict_t *pEdict)
{
}

bool V8Plugin::Hook_ClientConnect(edict_t *pEntity, const char *pszName, const char *pszAddress, char *reject, int maxrejectlen)
{
	RETURN_META_VALUE_NEWPARAMS(MRES_IGNORED, true, &IServerGameClients::ClientConnect, (pEntity, pszName, pszAddress, reject, maxrejectlen));
}

void V8Plugin::Hook_ClientCommand(edict_t *pEntity, const CCommand &args)
{
}
