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

#include "MiniStatsCore.h"
#include "cvars.h"
#include "Stats.h"

HLSVars g_HLSVars;

ConVar g_VarMiniVersion("ms_version",HLX_VERSION, FCVAR_SPONLY|FCVAR_REPLICATED|FCVAR_NOTIFY, "The version of the MiniStats plugin");
ConVar g_VarHLXShowHPLeft("ms_showhpleft", "1", 0, "Show how much HP the killer has left");
ConVar g_VarHLXBlockSayRank("ms_hidesayrank", "1", 0, "If the plugin should block the rank text from the public");
ConVar g_VarHLXShowMoreStats("ms_morestats", "1", 0, "1 = Enable the morestats system (Display on death & Round start)\n2 = Enable the morestats system (Display on Death)");
ConVar g_VarHLXTellAboutMoreStats("ms_morestatsinform", "5", 0, "How often the users should be informed about the ability to say /morestats to get more Stats info");

ConVar g_VarTeam1Name("ms_team1name","team1", 0, "Contains the name of the first player team, like Terrorist");
ConVar g_VarTeam2Name("ms_team2name","team2", 0, "Contains the name of the second player team, like Counter-Terrorist");

void HLSVars::SetupCallBacks() // Yes i know we read all the cvars, when just 1 changed ( Or we can end up reading all 4 chars 4 times. But who cares, this should only happen on a mapchange and im lazy )
{
	g_VarHLXShowMoreStats.InstallChangeCallback((FnChangeCallback_t) &Stats::cbCvarMoreStats);
}

bool HLSVars::RegisterConCommandBase(ConCommandBase *pVar)
{
	//this will work on any type of concmd!
	//pVar->Create(

	return META_REGCVAR(pVar);
}
int HLSVars:: GetMoreStats()
{
	return g_VarHLXShowMoreStats.GetInt();
}

int HLSVars::GetMoreStatsReportTime()
{
	return g_VarHLXTellAboutMoreStats.GetInt();
}
const char *HLSVars::GetTeam1Name()
{
	return g_VarTeam1Name.GetString();
}
const char *HLSVars::GetTeam2Name()
{
	return g_VarTeam2Name.GetString();
}
bool HLSVars::ShowHPLeft()
{
	if(g_VarHLXShowHPLeft.GetInt() == 0)
		return false;
	
	return true;
}

bool HLSVars::BlockSayRank()
{
	return g_VarHLXBlockSayRank.GetBool();
}