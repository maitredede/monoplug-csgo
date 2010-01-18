/* ======== SimpleStats ========
* Copyright (C) 2004-2008 Erling K. Sæterdal
* No warranties of any kind
*
* License: zlib/libpng
*
* Author(s): Erling K. Sæterdal ( EKS )
* Credits:
*	Menu code based on code from CSDM ( http://www.tcwonline.org/~dvander/cssdm ) Created by BAILOPAN
*	Adminloading function from Xaphan ( http://www.sourcemod.net/forums/viewtopic.php?p=25807 ) Created by Devicenull
* Helping on misc errors/functions: BAILOPAN,sslice,devicenull,PMOnoTo,cybermind ( most who idle in #sourcemod on GameSurge realy )
* ============================ */

#ifndef _INCLUDE_STEAMIDLIST
#define _INCLUDE_STEAMIDLIST

#include "MiniStatsCore.h"

class SteamIDList : StrUtil
{
public:
	SteamIDList() {g_SteamIDListCount = 0;}
	~SteamIDList() {g_SteamIDList.clear();}

	void AddSteamID(const char *pSteamID);
	void RemoveSteamID(const char *pSteamID);
	bool IsSteamIDInList(const char *pSteamID);

	void ReadSteamIDFile();
	void WriteSteamIDFile();

private:
	stSteamIDInfo GetstSteamIDInfo(const char *pSteamID);
	void AddSteamID(const char *pSteamID,uint DaysSinceOnline);
	int GetDaysSince1970();
	void GrowSteamIDList();

	SourceHook::CVector<stSteamIDInfo *>g_SteamIDList;
	uint g_SteamIDListCount;
};
#endif