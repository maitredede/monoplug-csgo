#include "tools.h"
#include "pluginterfaces.h"
#include "CMonoPlugin.h"

namespace MonoPlugin
{
	edict_t *EdictOfUserId( int UserId )
	{
		for(int i=0; i<g_MonoPlugin.m_EdictCount; i++)
		{
			edict_t* pEntity = g_engine->PEntityOfEntIndex(i);
			if(pEntity && !pEntity->IsFree())
			{
				if(strcmp(pEntity->GetClassName(), "player") == 0)
				{
					if(g_engine->GetPlayerUserId(pEntity) == UserId)
					{
						return pEntity;
					}
				}
			}
		}
		return NULL;
	};
}