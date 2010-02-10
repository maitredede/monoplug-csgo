#include "CMonoPlugin.h"
#include "pluginterfaces.h"

namespace MonoPlugin
{
	SH_DECL_HOOK1_void(IServerGameDLL, GameFrame, SH_NOATTRIB, 0, bool);
	SH_DECL_HOOK3_void(IServerGameDLL, ServerActivate, SH_NOATTRIB, 0, edict_t *, int, int);
	SH_DECL_HOOK0_void(IServerGameDLL, LevelShutdown, SH_NOATTRIB, 0);
	SH_DECL_HOOK1_void(IServerGameClients, ClientDisconnect, SH_NOATTRIB, 0, edict_t *);
	SH_DECL_HOOK2_void(IServerGameClients, ClientPutInServer, SH_NOATTRIB, 0, edict_t *, const char *);
	SH_DECL_HOOK1_void(ConCommand, Dispatch, SH_NOATTRIB, 0, const CCommand &);

	SH_DECL_HOOK2_void(IServerGameClients, NetworkIDValidated, SH_NOATTRIB, 0, const char*, const char*);

	SH_DECL_HOOK5(IServerGameClients, ClientConnect, SH_NOATTRIB, 0, bool, edict_t *, const char*, const char *, char *, int);

	SH_DECL_HOOK1_void(IServerGameClients, SetCommandClient, SH_NOATTRIB, 0, int);

	void CMonoPlugin::AddHooks()
	{
		//Interthread calls
		SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_GameFrame, true);
		//Say hooks
		m_pSayCmd = icvar->FindCommand("say");
		m_pSayTeamCmd = icvar->FindCommand("say_team");
		if (m_pSayCmd)
		{
			SH_ADD_HOOK_MEMFUNC(ConCommand, Dispatch, m_pSayCmd, this, &CMonoPlugin::Hook_PlayerSay, false);
		}
		if (m_pSayTeamCmd)
		{
			SH_ADD_HOOK_MEMFUNC(ConCommand, Dispatch, m_pSayTeamCmd, this, &CMonoPlugin::Hook_PlayerSay, false);
		}
		//Client commands
		SH_ADD_HOOK_MEMFUNC(IServerGameClients, SetCommandClient, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_SetCommandClient, false);
		//Steamid
		SH_ADD_HOOK_MEMFUNC(IServerGameClients, NetworkIDValidated, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_NetworkIDValidated, true);
	}

	void CMonoPlugin::RemoveHooks()
	{
		//Interthread calls
		SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, GameFrame, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_GameFrame, true);
		//Say hooks
		if (m_pSayCmd)
		{
			SH_REMOVE_HOOK_MEMFUNC(ConCommand, Dispatch, m_pSayCmd, this, &CMonoPlugin::Hook_PlayerSay, false);
		}
		if (m_pSayTeamCmd)
		{
			SH_REMOVE_HOOK_MEMFUNC(ConCommand, Dispatch, m_pSayTeamCmd, this, &CMonoPlugin::Hook_PlayerSay, false);
		}
		//Client commands
		SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, SetCommandClient, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_SetCommandClient, false);
		//Steamid
		SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, NetworkIDValidated, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_NetworkIDValidated, true);
	}

	void CMonoPlugin::Hook_Attach_ServerActivate()
	{
		SH_ADD_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ServerActivate, true);
	}

	void CMonoPlugin::Hook_Detach_ServerActivate()
	{
		SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ServerActivate, true);
	}

	void CMonoPlugin::Hook_Attach_ClientDisconnect()
	{
		SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ClientDisconnect, true);		//Hook ClientPutInServer to our function
	}

	void CMonoPlugin::Hook_Detach_ClientDisconnect()
	{
		SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ClientDisconnect, true);		//Hook ClientPutInServer to our function
	}

	void CMonoPlugin::Hook_Attach_ClientPutInServer()
	{
		SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ClientPutInServer, true);	//Hook SetCommandClient to our function
	}

	void CMonoPlugin::Hook_Detach_ClientPutInServer()
	{
		SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ClientPutInServer, true);	//Hook SetCommandClient to our function
	}

	void CMonoPlugin::Hook_Attach_ClientConnect()
	{
		SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientConnect, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ClientConnect, false);	//Hook ClientCommand to our function
	}

	void CMonoPlugin::Hook_Detach_ClientConnect()
	{
		SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientConnect, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ClientConnect, false);	//Hook ClientCommand to our function
	}

	void CMonoPlugin::Hook_Attach_LevelShutdown()
	{
		SH_ADD_HOOK_MEMFUNC(IServerGameDLL, LevelShutdown, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_LevelShutdown, false);
	}

	void CMonoPlugin::Hook_Detach_LevelShutdown()
	{
		SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, LevelShutdown, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_LevelShutdown, false);
	}


	void CMonoPlugin::Hook_Raise_ServerActivate(edict_t *pEdictList, int edictCount, int clientMax)
	{
		this->m_clientMax = clientMax;
		this->m_EdictCount = edictCount;
		void* args[1];
		args[0] = &clientMax;
		CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_Raise_ServerActivate, args);
	}

	void CMonoPlugin::Hook_PlayerSay(const CCommand &command)
	{
		MonoString* line = CMonoHelpers::GetString(g_Domain, command.ArgS());
		MonoArray* arr = CMonoHelpers::GetStringArray(g_Domain, command.ArgC(), command.ArgV());

		int index = this->m_clientCommand;

		MonoObject* player = NULL;
		if(index > 0)
		{
			player = g_MonoPlugin.m_players[index].Player->GetPlayer();
		}

		void* args[3];
		args[0] = player;
		args[1] = line;
		args[2] = arr;
		MonoObject* ret = CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_Raise_PlayerSay, args);
		if(ret)
		{
			bool bRet = *(bool*)mono_object_unbox(ret);
			if(bRet)
			{
				RETURN_META(MRES_SUPERCEDE);
			}
		}
		RETURN_META(MRES_IGNORED);
	}

	void CMonoPlugin::Hook_SetCommandClient(int index)
	{
		this->m_clientCommand = index + 1;
	}
}
