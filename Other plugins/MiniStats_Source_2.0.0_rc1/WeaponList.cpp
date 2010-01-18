/* ======== SimpleStats ========
* Copyright (C) 2004-2008 Erling K. Sæterdal
* No warranties of any kind
*
* License: zlib/libpng
*
* Author(s): Erling K. Sæterdal ( EKS )
* Credits:
* Helping on misc errors/functions: BAILOPAN,sslice,devicenull,PMOnoTo,cybermind ( most who idle in #sourcemod on GameSurge realy )
* ============================ */

#include "MiniStatsCore.h"
#include "time.h"
#include "cvars.h"
#include "convar.h"
#include "const.h"
#include <string.h>
#include <bitbuf.h>
#include <ctype.h>
#include "recipientfilters.h"

int WeaponList::GetWeaponIndex(const char *pWeaponName)
{
	int StrLen = strlen(pWeaponName);
	unsigned long WeaponNameHash = g_MSCore.StrHash(pWeaponName,StrLen);

	for(int i=1;i<MAX_WEAPONS;i++)	
	{
		if(i >= g_WeaponListCount) // This is a new weapon we add it
		{
			_snprintf(g_TextWeaponList[i],StrLen + 1,"%s",pWeaponName);
			g_HashWeaponList[i] = WeaponNameHash;

			g_WeaponListCount++;
			return i;
		}

		if(g_HashWeaponList[i] == WeaponNameHash && strcmp(pWeaponName,g_TextWeaponList[i]) == 0)
			return i;
	}
	return -1;
}

const char * WeaponList::GetWeaponName(int WeaponIndex)
{
	if(WeaponIndex >= g_WeaponListCount || WeaponIndex < 0)
		return "ErrorWeaponName";

	return g_TextWeaponList[WeaponIndex];
}