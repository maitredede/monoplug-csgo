#include "monoBase.h"
#include "Events.h"
#include "CMonoHelpers.h"
#include "CMonoPlugin.h"

namespace MonoPlugin
{
	MP_EVENT_CODE(server_spawn, g_MonoPlugin.m_event_server_spawn, g_MonoPlugin.m_ClsMain_event_server_spawn)
	{
		void* args[9];
		args[0] = CMonoHelpers::GetString(g_Domain, evt->GetString("hostname"));
		args[1] = CMonoHelpers::GetString(g_Domain, evt->GetString("address"));
		args[2] = CMonoHelpers::GetString(g_Domain, evt->GetString("port"));
		args[3] = CMonoHelpers::GetString(g_Domain, evt->GetString("game"));
		args[4] = CMonoHelpers::GetString(g_Domain, evt->GetString("mapname"));
		int maxplayers = evt->GetInt("maxplayers");
		args[5] = &maxplayers;
		args[6] = CMonoHelpers::GetString(g_Domain, evt->GetString("os"));
		bool dedicated = !!evt->GetInt("dedicated");
		args[7] = &dedicated;
		bool password = !!evt->GetInt("password");
		args[8] = &password;
		CMonoHelpers::CallMethod(g_MonoPlugin.m_main, this->m_method, args);
	}

	MP_EVENT_CODE(server_shutdown, g_MonoPlugin.m_event_server_shutdown, g_MonoPlugin.m_ClsMain_event_server_shutdown)
	{
		void* args[1];
		args[0] = CMonoHelpers::GetString(g_Domain, evt->GetString("reason"));
		CMonoHelpers::CallMethod(g_MonoPlugin.m_main, this->m_method, args);
	}

	MP_EVENT_CODE_PERMANENT(player_connect, g_MonoPlugin.m_event_player_connect, g_MonoPlugin.m_ClsMain_event_player_connect)
	{
		void* args[5];
		args[0] = CMonoHelpers::GetString(g_Domain, evt->GetString("name"));
		int index = evt->GetInt("index");
		args[1] = &index;
		int userid = evt->GetInt("userid");
		args[2] = &userid;
		args[3] = CMonoHelpers::GetString(g_Domain, evt->GetString("networkid"));
		args[4] = CMonoHelpers::GetString(g_Domain, evt->GetString("address"));
		//args[5] = g_MonoPlugin.GetPlayer(userid);
		CMonoHelpers::CallMethod(g_MonoPlugin.m_main, this->m_method, args);
	}

	MP_EVENT_CODE_PERMANENT(player_disconnect, g_MonoPlugin.m_event_player_disconnect, g_MonoPlugin.m_ClsMain_event_player_disconnect)
	{
		void* args[5];
		int userid = evt->GetInt("userid");
		args[0] = &userid;
		args[1] = CMonoHelpers::GetString(g_Domain, evt->GetString("reason"));
		args[2] = CMonoHelpers::GetString(g_Domain, evt->GetString("name"));
		args[3] = CMonoHelpers::GetString(g_Domain, evt->GetString("networkid"));
		args[4] = g_MonoPlugin.GetPlayer(userid);
		CMonoHelpers::CallMethod(g_MonoPlugin.m_main, this->m_method, args);
	}
}
