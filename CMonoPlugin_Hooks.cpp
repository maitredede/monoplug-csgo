#include "CMonoPlugin.h"
#include "pluginterfaces.h"

namespace MonoPlugin
{
	SH_DECL_HOOK1_void(IServerGameDLL, GameFrame, SH_NOATTRIB, 0, bool);
	SH_DECL_HOOK3_void(IServerGameDLL, ServerActivate, SH_NOATTRIB, 0, edict_t *, int, int);
	SH_DECL_HOOK0_void(IServerGameDLL, LevelShutdown, SH_NOATTRIB, 0);
	SH_DECL_HOOK1_void(IServerGameClients, ClientDisconnect, SH_NOATTRIB, 0, edict_t *);
	SH_DECL_HOOK2_void(IServerGameClients, ClientPutInServer, SH_NOATTRIB, 0, edict_t *, const char *);

	void CMonoPlugin::AddHooks()
	{
		SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_GameFrame, true);
		SH_ADD_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ServerActivate, true);
	}

	void CMonoPlugin::RemoveHooks()
	{
		SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, GameFrame, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_GameFrame, true);
		SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ServerActivate, true);
	}

	void CMonoPlugin::Hook_Attach_ServerActivate()
	{
		//Special case : Nothing to do
	}

	void CMonoPlugin::Hook_Detach_ServerActivate()
	{
		//Special case : Nothing to do
	}

	void CMonoPlugin::Hook_Raise_ServerActivate(edict_t *pEdictList, int edictCount, int clientMax)
	{
		this->m_EdictCount = edictCount;
		void* args[1];
		args[0] = &clientMax;
		CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_Raise_ServerActivate, args);
	}

	void CMonoPlugin::Hook_Attach_LevelShutdown()
	{
		SH_ADD_HOOK_MEMFUNC(IServerGameDLL, LevelShutdown, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_LevelShutdown, false);
	}

	void CMonoPlugin::Hook_Detach_LevelShutdown()
	{
		SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, LevelShutdown, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_LevelShutdown, false);
	}

	void CMonoPlugin::Hook_Raise_LevelShutdown()
	{
		CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_Raise_LevelShutdown, NULL);
	}

	void CMonoPlugin::Hook_Attach_ClientDisconnect()
	{
		SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ClientDisconnect, true);		//Hook ClientPutInServer to our function
	}

	void CMonoPlugin::Hook_Detach_ClientDisconnect()
	{
		SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ClientDisconnect, true);		//Hook ClientPutInServer to our function
	}

	void CMonoPlugin::Hook_Raise_ClientDisconnect(edict_t *pEntity)
	{
		if(!pEntity || pEntity->IsFree())
		{
			return;
		}

		MonoObject* player = this->GetPlayer(pEntity);

		void* args[1];
		args[0] = player;
		CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_Raise_ClientDisconnect, args);
	}

	void CMonoPlugin::Hook_Attach_ClientPutInServer()
	{
		SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ClientPutInServer, true);	//Hook SetCommandClient to our function
	}

	void CMonoPlugin::Hook_Detach_ClientPutInServer()
	{
		SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, g_ServerClients, &g_MonoPlugin, &CMonoPlugin::Hook_Raise_ClientPutInServer, true);	//Hook SetCommandClient to our function
	}

	void CMonoPlugin::Hook_Raise_ClientPutInServer(edict_t *pEntity, const char* playername)
	{
		if(!pEntity || pEntity->IsFree())
		{
			return;
		}

		MonoObject* player = this->GetPlayer(pEntity);

		void* args[1];
		args[0] = player;
		CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_Raise_ClientPutInServer, args);
	}
}
