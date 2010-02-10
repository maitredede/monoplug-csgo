#include "monoBase.h"
#include "Events.h"
#include "CMonoHelpers.h"
#include "CMonoPlugin.h"
#include "CMonoPlayer.h"
#include "tools.h"

namespace MonoPlugin
{
	void CMonoPlugin::Hook_Raise_ClientDisconnect(edict_t *pEntity)
	{
#ifdef _DEBUG
		META_CONPRINTF("********** %s **********\n", "Hook_Raise_ClientDisconnect");
#endif
		if(!pEntity || pEntity->IsFree())
		{
			return;
		}

		int index = g_engine->IndexOfEdict(pEntity);

		MonoObject* player = NULL;
		if(g_MonoPlugin.m_players[index].Player)
		{
			g_MonoPlugin.m_players[index].Player->SetConnected(false, false);
			player = g_MonoPlugin.m_players[index].Player->GetPlayer();
		}

		void* args[1];
		args[0] = player;
		CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_Raise_ClientDisconnect, args);

		if(g_MonoPlugin.m_players[index].Player)
		{
			delete g_MonoPlugin.m_players[index].Player;
			g_MonoPlugin.m_players[index].Player = NULL;
			g_MonoPlugin.m_players[index].PlayerEdict = NULL;
		}

		RETURN_META(MRES_IGNORED);
	}

	MP_EVENT_CODE(player_disconnect, g_MonoPlugin.m_event_player_disconnect, g_MonoPlugin.m_ClsMain_event_player_disconnect)
	{
#ifdef _DEBUG
		META_CONPRINTF("********** %s **********\n", "MP_EVENT_CODE player_disconnect");
#endif

		int userid = evt->GetInt("userid");
		bool found = false;
		int index = -1;

		for(int i = 0; i < MAXPLAYERS + 2; i++)
		{
			if(g_MonoPlugin.m_players[i].Player)
			{
				if(g_MonoPlugin.m_players[i].Player->UserId() == userid)
				{
					index = i;
					found = true;
					break;
				}
			}
		}

		MonoObject* player = NULL;
		if(found)
		{
			g_MonoPlugin.m_players[index].Player->SetData(evt->GetString("name"), NULL);
			g_MonoPlugin.m_players[index].Player->SetConnected(false, false);
			player = g_MonoPlugin.m_players[index].Player->GetPlayer();
		}

		void* args[2];
		args[0] = player;
		args[1] = CMonoHelpers::GetString(g_Domain, evt->GetString("reason"));
		CMonoHelpers::CallMethod(g_MonoPlugin.m_main, this->m_method, args);

		if(found)
		{
			delete g_MonoPlugin.m_players[index].Player;
			g_MonoPlugin.m_players[index].Player = NULL;
			g_MonoPlugin.m_players[index].PlayerEdict = NULL;
		}
	}

	void CMonoPlugin::Hook_Raise_LevelShutdown()
	{
#ifdef _DEBUG
		META_CONPRINTF("********** %s **********\n", "Hook_Raise_LevelShutdown");
#endif
		for(int i = 0; i < MAXPLAYERS + 2; i++)
		{
			if(g_MonoPlugin.m_players[i].Player)
			{
				g_MonoPlugin.m_players[i].Player->SetConnected(false, false);
			}
		}

		CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_Raise_LevelShutdown, NULL);

		for(int i = 0; i < MAXPLAYERS + 2; i++)
		{
			if(g_MonoPlugin.m_players[i].Player)
			{
				delete g_MonoPlugin.m_players[i].Player;
				g_MonoPlugin.m_players[i].Player = NULL;
				g_MonoPlugin.m_players[i].PlayerEdict = NULL;
			}
		}
	}
}
