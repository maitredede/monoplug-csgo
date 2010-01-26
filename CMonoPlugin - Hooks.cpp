#include "CMonoPlugin.h"

SH_DECL_HOOK1_void(IServerGameDLL, GameFrame, SH_NOATTRIB, 0, bool);

void MonoPlugin::CMonoPlugin::AddHooks()
{
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_GameFrame, true);
}

void MonoPlugin::CMonoPlugin::RemoveHooks()
{
	SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, GameFrame, g_iserver, &g_MonoPlugin, &CMonoPlugin::Hook_GameFrame, true);
}
//void MonoPlugin::Hook_ServerActivate(edict_t *pEdictList, int edictCount, int clientMax)
//{
//	META_LOG(g_PLAPI, "ServerActivate() called: edictCount = %d, clientMax = %d", edictCount, clientMax);
//}
