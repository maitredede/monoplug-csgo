#include "CMonoPlugin.h"
#include "pluginterfaces.h"

namespace MonoPlugin
{
	SH_DECL_HOOK1_void(IServerGameDLL, GameFrame, SH_NOATTRIB, 0, bool);
	SH_DECL_HOOK3_void(IServerGameDLL, ServerActivate, SH_NOATTRIB, 0, edict_t *, int, int);

	void CMonoPlugin::AddHooks()
	{
		SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_GameFrame, true);
		SH_ADD_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_ServerActivate, true);
	}

	void CMonoPlugin::RemoveHooks()
	{
		SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, GameFrame, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_GameFrame, true);
		SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_ServerActivate, true);
	}

	void CMonoPlugin::Hook_ServerActivate(edict_t *pEdictList, int edictCount, int clientMax)
	{
		this->m_EdictCount = edictCount;
	}
}
