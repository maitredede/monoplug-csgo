#include "Common.h"
#include "CMonoPlug.h"
#include <server_class.h>

//ICvar *icvar = NULL;
CMonoPlug g_MonoPlug;
CMonoPlugListener g_Listener;
MonoDomain* g_Domain = NULL;

IServerGameDLL *g_ServerDll = NULL;
IServerGameClients *g_ServerClients = NULL;
IVEngineServer *g_Engine = NULL;
IServerPluginHelpers *helpers = NULL;
IGameEventManager2 *gameevents = NULL;
IServerPluginCallbacks *vsp_callbacks = NULL;
IPlayerInfoManager *playerinfomanager = NULL;
ICvar *icvar = NULL;
CGlobalVars *g_Globals = NULL;
IServerPluginCallbacks* g_vsp_callbacks = NULL;
IServerPluginHelpers* g_helpers = NULL;
IPlayerInfoManager* g_playerinfomanager = NULL;
IFileSystem* g_filesystem = NULL;
IGameEventManager2* g_GameEventManager = NULL;

edict_t *EdictOfUserId( int UserId )
{
	for(int i=0; i<g_MonoPlug.m_EdictCount; i++)
	{
		edict_t* pEntity = g_Engine->PEntityOfEntIndex(i);
		if(pEntity && !pEntity->IsFree())
		{
			if(FStrEq(pEntity->GetClassName(), "player"))
			{
				if(g_Engine->GetPlayerUserId(pEntity) == UserId)
				{
					return pEntity;
				}
			}
		}
	}
	return NULL;
};

// find an offset for PropertyName listed in the server class send table for ClassName
int UTIL_FindOffset(const char *ClassName, const char *PropertyName)
{
	ServerClass *sc = g_ServerDll->GetAllServerClasses();
	while (sc)
	{
		if (FStrEq(sc->GetName(), ClassName))
		{
			int NumProps = sc->m_pTable->GetNumProps();
			for (int i=0; i<NumProps; i++)
			{
				if (stricmp(sc->m_pTable->GetProp(i)->GetName(), PropertyName) == 0)
				{
					return sc->m_pTable->GetProp(i)->GetOffset();
				}
			}
			return 0;
		}
		sc = sc->m_pNext;
	}

	return 0;
};

