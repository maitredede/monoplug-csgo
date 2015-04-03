#include <stdio.h>
#include "Plugin.h"
#include <iplayerinfo.h>
#include "UserTracker.h"

bool DotNetPlugPlugin::Hook_LevelInit(const char *pMapName, const char *pMapEntities, const char *pOldLevel, const char *pLandmarkName, bool loadGame, bool background)
{
	gUserTracker.LevelInit();

	this->hostname = icvar->FindVar("hostname");
	this->tv_name = icvar->FindVar("tv_name");

	g_Managed.RaiseLevelInit(pMapName, pMapEntities, pOldLevel, pLandmarkName, loadGame, background);

	RETURN_META_VALUE_NEWPARAMS(MRES_IGNORED, true, &IServerGameDLL::LevelInit, (pMapName, pMapEntities, pOldLevel, pLandmarkName, loadGame, background));
}

void DotNetPlugPlugin::Hook_ServerActivate(edict_t *pEdictList, int edictCount, int clientMax)
{
	//META_LOG(g_PLAPI, "ServerActivate() called: edictCount = %d, clientMax = %d", edictCount, clientMax);
	this->max_players = clientMax;

	g_Managed.RaiseServerActivate(clientMax);
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

void DotNetPlugPlugin::Hook_LevelShutdown()
{
	g_Managed.RaiseLevelShutdown();
}

void DotNetPlugPlugin::Hook_ClientActive(edict_t *pEntity, bool bLoadGame)
{
	gUserTracker.ClientActive(pEntity);

	//player_t player;
	//player.entity = pEntity;
	//if (!FindPlayerByEntity(&player)) return;

	//const char *pname = player.name;
	//if (pname && pname[0] == 0)
	//	pname = NULL;

	//if (!pname && !player.is_bot) {
	//	char	kick_cmd[512];
	//	PrintToClientConsole(player.entity, "Empty name violation\n");
	//	gpManiPlayerKick->AddPlayer(player.index, 0.5f, "Empty name violation");
	//	snprintf(kick_cmd, sizeof(kick_cmd), "kickid %i Empty name violation\n", player.user_id);
	//	LogCommand(NULL, "Kick (Empty name violation) [%s] %s\n", player.steam_id, kick_cmd);
	//	return;
	//}

	//if (!player.is_bot)
	//{
	//	user_name[player.index - 1].in_use = true;
	//	Q_strcpy(user_name[player.index - 1].name, engine->GetClientConVarValue(player.index, "name"));

	//	// Player settings
	//	PlayerJoinedInitSettings(&player);

	//	ProcessPlayActionSound(&player, MANI_ACTION_SOUND_JOINSERVER);
	//}
	g_Managed.RaiseClientActive();
}

void DotNetPlugPlugin::Hook_ClientDisconnect(edict_t *pEntity)
{
	player_t player;

	player.entity = pEntity;

	if (!FindPlayerByEntity(&player))
	{
		//gpManiReservedSlot->ClientDisconnect(NULL); // must be human and still connecting!
		return;
	}

	g_Managed.RaiseClientDisconnect();
	gUserTracker.ClientDisconnect(&player);
}

void DotNetPlugPlugin::Hook_ClientPutInServer(edict_t *pEntity, char const *playername)
{
	g_Managed.RaiseClientPutInServer();
}

void DotNetPlugPlugin::Hook_SetCommandClient(int index)
{
	//g_Managed.RaiseClientPutInServer();
}

void DotNetPlugPlugin::Hook_ClientSettingsChanged(edict_t *pEdict)
{
	if (playerinfomanager)
	{
		int	player_index = IndexOfEdict(pEdict);
		//if (user_name[player_index - 1].in_use)
		//{
		//	const char * name = engine->GetClientConVarValue(player_index, "name");
		//	if (strcmp(user_name[player_index - 1].name, name) != 0)
		//	{
		//		player_t player;
		//		player.index = player_index;
		//		if (FindPlayerByIndex(&player))
		//		{
		//			if (!player.is_bot)
		//			{
		//				// Handle name change
		//				PlayerJoinedInitSettings(&player);
		//				ProcessChangeName(&player, name, user_name[player_index - 1].name);
		//				Q_strcpy(user_name[player_index - 1].name, name);
		//			}
		//		}
		//	}
		//}
	}
	g_Managed.RaiseClientSettingsChanged();
}

bool DotNetPlugPlugin::Hook_ClientConnect(edict_t *pEntity, const char *pszName, const char *pszAddress, char *reject, int maxrejectlen)
{
	g_Managed.RaiseClientConnect();

	RETURN_META_VALUE_NEWPARAMS(MRES_IGNORED, true, &IServerGameClients::ClientConnect, (pEntity, pszName, pszAddress, reject, maxrejectlen));
}

void DotNetPlugPlugin::Hook_ClientCommand(edict_t *pEntity, const CCommand &args)
{
	g_Managed.RaiseClientCommand();
}

