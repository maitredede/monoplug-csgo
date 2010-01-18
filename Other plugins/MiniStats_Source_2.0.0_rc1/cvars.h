/* ======== SimpleStats ========
* Copyright (C) 2004-2008 Erling K. Sæterdal
* No warranties of any kind
*
* License: zlib/libpng
*
* Author(s): Erling K. Sæterdal ( EKS )
* Credits:
*	Menu code based on code from CSDM ( http://www.tcwonline.org/~dvander/cssdm ) Created by BAILOPAN
* Helping on misc errors/functions: BAILOPAN,sslice,devicenull,PMOnoTo,cybermind ( most who idle in #sourcemod on GameSurge realy )
* ============================ */

#ifndef _INCLUDE_CVARS_H
#define _INCLUDE_CVARS_H

#include <convar.h>

class HLSVars : public IConCommandBaseAccessor
{
public:
	virtual bool RegisterConCommandBase(ConCommandBase *pVar);
	
	bool ShowHPLeft();
	const char *HLSVars::GetTeam1Name();
	const char *HLSVars::GetTeam2Name();
	int GetMoreStatsReportTime();
	int GetMoreStats();

	bool BlockSayRank();
	static void SetupCallBacks();
private:
	
};

extern HLSVars g_HLSVars;

#endif //_INCLUDE_CVARS_H
