#include "monoBase.h"
#include "Events.h"
#include "CMonoHelpers.h"
#include "CMonoPlugin.h"
#include "CMonoPlayer.h"
#include "tools.h"

namespace MonoPlugin
{
	MP_EVENT_CODE(server_spawn, g_MonoPlugin.m_event_server_spawn, g_MonoPlugin.m_ClsMain_event_server_spawn)
	{
		META_LOG(g_PLAPI, "********** %s **********\n", "MP_EVENT_CODE server_spawn");
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
		META_LOG(g_PLAPI, "********** %s **********\n", "MP_EVENT_CODE server_shutdown");
		void* args[1];
		args[0] = CMonoHelpers::GetString(g_Domain, evt->GetString("reason"));
		CMonoHelpers::CallMethod(g_MonoPlugin.m_main, this->m_method, args);
	}

}

