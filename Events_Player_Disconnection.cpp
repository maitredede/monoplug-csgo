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
		META_LOG(g_PLAPI, "********** %s **********\n", "Hook_Raise_ClientDisconnect");
#endif
		if(!pEntity || pEntity->IsFree())
		{
			return;
		}

		int index = g_engine->IndexOfEdict(pEntity);
		int userid = g_engine->GetPlayerUserId(pEntity);

		g_MonoPlugin.m_players[index].Player->SetConnected(false, false);

		//TODO : FIXME

		//void* args[1];
		//args[0] = this->m_players[index].MPlayer;
		//CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_Raise_ClientDisconnect, args);

		//if(this->m_players[index].MPlayer)
		//{
		//	delete this->m_players[index].MPlayer;
		//	this->m_players[index].MPlayer = NULL;
		//}

		RETURN_META(MRES_IGNORED);
	}

	MP_EVENT_CODE(player_disconnect, g_MonoPlugin.m_event_player_disconnect, g_MonoPlugin.m_ClsMain_event_player_disconnect)
	{
#ifdef _DEBUG
		META_LOG(g_PLAPI, "********** %s **********\n", "MP_EVENT_CODE player_disconnect");
#endif

		int userid = evt->GetInt("userid");
		edict_t* pEntity = EdictOfUserId(userid);
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
		//int index = g_engine->IndexOfEdict(pEntity);
		//int userid = g_engine->GetPlayerUserId(pEntity);

		if(found)
		{
			g_MonoPlugin.m_players[index].Player->SetConnected(false, false);
		}
		//TODO : FIXME

		//int index = g_engine->IndexOfEdict(pEntity);

		//MonoObject* player = g_MonoPlugin.m_players[index].MPlayer;

		//void* args[5];
		//args[0] = &userid;
		//args[1] = CMonoHelpers::GetString(g_Domain, evt->GetString("reason"));
		//args[2] = CMonoHelpers::GetString(g_Domain, evt->GetString("name"));
		//args[3] = CMonoHelpers::GetString(g_Domain, evt->GetString("networkid"));
		//args[4] = player;
		//CMonoHelpers::CallMethod(g_MonoPlugin.m_main, this->m_method, args);
	}
}
