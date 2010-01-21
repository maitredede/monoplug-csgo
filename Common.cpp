#include "Common.h"
#include "CMonoPlug.h"
#include <server_class.h>

edict_t *EdictOfUserId( int UserId )
{
	for(int i=0; i<g_MonoPlug.m_EdictCount; i++)
	{
		edict_t* pEntity = g_MonoPlug.m_Engine->PEntityOfEntIndex(i);
		if(pEntity && !pEntity->IsFree())
		{
			if(FStrEq(pEntity->GetClassName(), "player"))
			{
				if(g_MonoPlug.m_Engine->GetPlayerUserId(pEntity) == UserId)
				{
					return pEntity;
				}
			}
		}
	}
	return NULL;
	//int i;
	//edict_t *pEnt;
	//for  (i=0; i < MAX_PLAYERS; i++)
	//{
	//	pEnt = g_SamplePlugin.m_Engine->PEntityOfEntIndex(i);
	//	if (pEnt && !pEnt->IsFree())
	//	{
	//		if (FStrEq(pEnt->GetClassName(), "player"))
	//		{
	//			if (g_SamplePlugin.m_Engine->GetPlayerUserId(pEnt)==UserId)
	//			{
	//				return pEnt;
	//			}
	//		}
	//	}
	//}

	return NULL;
};

// find an offset for PropertyName listed in the server class send table for ClassName
int UTIL_FindOffset(const char *ClassName, const char *PropertyName)
{
	ServerClass *sc = g_MonoPlug.m_ServerDll->GetAllServerClasses();
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

