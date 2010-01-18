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

#ifndef _INCLUDE_STATS_CLASS
#define _INCLUDE_STATS_CLASS

#include "const.h"

class Stats
{
public:
	void AddPlayerAttack(int id,int dmg,int hitgroup,int WeaponIndex);
	void AddPlayerAttackKill(int id,bool Headshot,int WeaponIndex,bool TeamKill);
	void AddPlayerShot(int id,int WeaponIndex);		// We call this every time a weapon is fired.
	void AddPlayerHeadShot(int id,int WeaponIndex);
	void ResetStatsArray(int id);
	void LogPlayerStats(int id);
	void ShowMoreStats();  // Shows to all enabled players, and clears old stats when their shown
	void ClearMoreStats();  // Shows to all enabled players, and clears old stats when their shown
	void ShowMoreStats(int id); // Shows only the stats to a specific player

	static void cbCvarMoreStats();
	static int MoreStatsMode;
	static int MoreStatsShowMode;

private:
	void ResetMoreStats(int id);
	void GenMoreStats(int id);
	void ClearMoreStats(int id);
	void FastResetWeaponStats(stWeaponStats *WeaponStats);	
	const char* GetHitGroupName(int HitGroup);	
	stWeaponStatsReturn GetWeaponStatsIndex(int id,int WeaponIndex);
};
#endif