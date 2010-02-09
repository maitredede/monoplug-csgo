#include "monoBase.h"
#include "Events.h"
#include "CMonoHelpers.h"
#include "CMonoPlugin.h"
#include "CMonoPlayer.h"
#include "tools.h"

namespace MonoPlugin
{
	////////////////////////////////
	// Connection #1
	////////////////////////////////
	MP_EVENT_CODE(player_connect, g_MonoPlugin.m_event_player_connect, g_MonoPlugin.m_ClsMain_event_player_connect)
	{
#ifdef _DEBUG
		META_LOG(g_PLAPI, "********** %s **********\n", "MP_EVENT_CODE player_connect");
#endif
		int index = evt->GetInt("index") + 1;
		const char* name = evt->GetString("name");
		int userid = evt->GetInt("userid");
		const char* networkid = evt->GetString("networkid");
		const char* address = evt->GetString("address");
	
		bool isBot;
		if(strcmp(networkid,"BOT") == 0 || strcmp(networkid,"HLTV") == 0)
			isBot = true;
		else 
			isBot = false;

		g_MonoPlugin.m_players[index].Player = new CMonoPlayer(userid, isBot);
		//g_MonoPlugin.m_players[index].PlayerEdict = EdictOfUserId(userid);

		g_MonoPlugin.m_players[index].Player->SetConnected(true, false);
		g_MonoPlugin.m_players[index].Player->SetData(name, address);
		g_MonoPlugin.m_players[index].Player->SetSteam(isBot, networkid);

		void* args[1];
		args[0] = g_MonoPlugin.m_players[index].Player->GetPlayer();
		CMonoHelpers::CallMethod(g_MonoPlugin.m_main, this->m_method, args);
	}

	////////////////////////////////
	// Connection #2 (ignored by bots)
	////////////////////////////////
	bool CMonoPlugin::Hook_Raise_ClientConnect(edict_t *pEntity, const char *pszName, const char *pszAddress, char *reject, int maxrejectlen)
	{
#ifdef _DEBUG
		META_LOG(g_PLAPI, "********** %s **********\n", "Hook_Raise_ClientConnect");
#endif
		int index = g_engine->IndexOfEdict(pEntity);
		//int userid = g_engine->GetPlayerUserId(pEntity);


		g_MonoPlugin.m_players[index].PlayerEdict = pEntity;
		g_MonoPlugin.m_players[index].Player->SetData(pszName, pszAddress);

		void* args[3];
		args[0] = g_MonoPlugin.m_players[index].Player->GetPlayer();

		CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_Raise_ClientConnect, args);

		RETURN_META_VALUE(MRES_IGNORED, true);
	}

	////////////////////////////////
	// Connection #3
	////////////////////////////////
	void CMonoPlugin::Hook_Raise_ClientPutInServer(edict_t *pEntity, const char* playername)
	{
#ifdef _DEBUG
		META_LOG(g_PLAPI, "********** %s **********\n", "Hook_Raise_ClientPutInServer");
#endif
		if(!pEntity || pEntity->IsFree())
		{
			return;
		}

		int index = g_engine->IndexOfEdict(pEntity);
		//int userid = g_engine->GetPlayerUserId(pEntity);

		g_MonoPlugin.m_players[index].Player->SetConnected(false, true);
		g_MonoPlugin.m_players[index].Player->SetData(playername, NULL);

		//this->RefreshPlayerInfo(index, playername, NULL, pEntity);

		void* args[1];
		args[0] = g_MonoPlugin.m_players[index].Player->GetPlayer();
		CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_Raise_ClientPutInServer, args);

		RETURN_META(MRES_IGNORED);
	}

	void CMonoPlugin::Hook_NetworkIDValidated(const char *pszUserName, const char *pszNetworkID)
	{
#ifdef _DEBUG
		META_LOG(g_PLAPI, "********** %s **********\n", "Hook_NetworkIDValidated");
#endif
		for(int i = 0; i < MAXPLAYERS + 2; i++)
		{
			if(this->m_players[i].Player)
			{
				if(strcmp(this->m_players[i].Player->Name(), pszUserName) == 0)
				{
					this->m_players[i].Player->SetSteam(true, pszNetworkID);
					break;
				}
			}
		}
	}
}
