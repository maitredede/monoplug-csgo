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
#include "Stats.h"

int Stats::MoreStatsMode = 0;
int Stats::MoreStatsShowMode = 0;

void Stats::cbCvarMoreStats()
{
	MoreStatsMode = g_HLSVars.GetMoreStats();
}

void Stats::ShowMoreStats()
{
	for(int i=1;i<=PluginCore.GetEngineUtils()->MaxClients;i++) if(g_MSCore.GetEngineUtils()->FastIsUserConnected(i) && !g_UserInfo[i].IsBot)
	{
		if(g_UserMoreStats[i].ShowStats)
		{
			if(Stats::MoreStatsMode == 1 || Stats::MoreStatsMode == 2 && !g_UserMoreStats[i].HasShownStats)
			ShowMoreStats(i);
		}
		else
			ClearMoreStats(i);
	}
}
void Stats::ClearMoreStats()
{
	for(int i=1;i<=PluginCore.GetEngineUtils()->MaxClients;i++) if(g_MSCore.GetEngineUtils()->FastIsUserConnected(i) && !g_UserInfo[i].IsBot)
	{
		ClearMoreStats(i);
	}
}
void Stats::ShowMoreStats(int id)
{
	//g_MSCore.GetEngineUtils()->LogPrint("Debug ShowMoreStats g_UserMoreStats[id].StatsStringMade %s  '%d'",g_MSCore.GetEngineUtils()->GetPlayerName(id),g_UserMoreStats[id].StatsStringMade);
	if(g_UserMoreStats[id].StatsStringMade == false)
		GenMoreStats(id);
	
	//g_MSCore.GetEngineUtils()->LogPrint("Debug ShowMoreStats g_UserMoreStats[id].StatsToShow %s  '%d'",g_MSCore.GetEngineUtils()->GetPlayerName(id),g_UserMoreStats[id].StatsToShow);
	if(g_UserMoreStats[id].StatsToShow == false)		
		return;

	//g_MSCore.GetEngineUtils()->LogPrint("Debug ShowMoreStats g_UserMoreStats[id].StatsString: %s '%s'",g_MSCore.GetEngineUtils()->GetPlayerName(id),g_UserMoreStats[id].StatsString);
	g_MSCore.GetEngineUtils()->MessagePlayer(id,g_UserMoreStats[id].StatsString);
	g_UserMoreStats[id].HasShownStats = true;
}

void Stats::GenMoreStats(int id)
{
	const char *ColorCode4 = "\x04";
	const char *ColorCode1 = "\x01";
	const char *NewLine = "\n";
	int StrLen;

	char VictimBuffer[240];
	char AttacerBuffer[240];

	int VictimBufferLen = 0;
	int AttacerBufferLen = 0;

	int DmgGivenOut=0;
	int DmgTaken=0;

	for(int i=1;i<=PluginCore.GetEngineUtils()->MaxClients;i++)
	{
		if(g_UserMoreStats[id].Players[i].DmgGiven > 0)
		{
			DmgGivenOut += g_UserMoreStats[id].Players[i].DmgGiven;

			if(g_UserMoreStats[id].Players[i].Killed)
				VictimBufferLen += _snprintf(&(VictimBuffer[VictimBufferLen]),sizeof(VictimBuffer)-VictimBufferLen,"%s (Killed - %d)%s",g_MSCore.GetEngineUtils()->GetPlayerName(i),g_UserMoreStats[id].Players[i].DmgGiven,NewLine);
			else
				VictimBufferLen += _snprintf(&(VictimBuffer[VictimBufferLen]),sizeof(VictimBuffer)-VictimBufferLen,"%s (%d)%s",g_MSCore.GetEngineUtils()->GetPlayerName(i),g_UserMoreStats[id].Players[i].DmgGiven,NewLine);
		}
		if(g_UserMoreStats[id].Players[i].DmgTaken > 0)
		{
			DmgTaken += g_UserMoreStats[id].Players[i].DmgTaken;

			if(g_UserMoreStats[id].Players[i].Killer)
				AttacerBufferLen += _snprintf(&(AttacerBuffer[AttacerBufferLen]),sizeof(AttacerBuffer)-AttacerBufferLen,"%s (Killer - %d)%s",g_MSCore.GetEngineUtils()->GetPlayerName(i),g_UserMoreStats[id].Players[i].DmgTaken,NewLine);
			else
				AttacerBufferLen += _snprintf(&(AttacerBuffer[AttacerBufferLen]),sizeof(AttacerBuffer)-AttacerBufferLen,"%s (%d)%s",g_MSCore.GetEngineUtils()->GetPlayerName(i),g_UserMoreStats[id].Players[i].DmgTaken,NewLine);
		}
	}
	if(VictimBufferLen == 0 && AttacerBufferLen == 0 ) // No stats to show, we halt on this user
	{
		g_UserMoreStats[id].StatsToShow = false;
		g_UserMoreStats[id].StatsStringMade = false;
		return;
	}

	VictimBuffer[VictimBufferLen] = '\0';
	AttacerBuffer[AttacerBufferLen] = '\0';

	if(DmgGivenOut > 0 && DmgTaken > 0)
		StrLen = _snprintf(g_UserMoreStats[id].StatsString,sizeof(g_UserMoreStats[0].StatsString),"%sVictims %s: (%d Dmg given out)%s%s%sAttackers %s: (%d Dmg taken)%s%s",ColorCode4,ColorCode1,DmgGivenOut,NewLine,VictimBuffer,ColorCode4,ColorCode1,DmgTaken,NewLine,AttacerBuffer);
	else if(DmgGivenOut > 0)
		StrLen = _snprintf(g_UserMoreStats[id].StatsString,sizeof(g_UserMoreStats[0].StatsString),"%sVictims %s: (%d Dmg given out)%s%s",ColorCode4,ColorCode1,DmgGivenOut,NewLine,VictimBuffer);
	else
		StrLen = _snprintf(g_UserMoreStats[id].StatsString,sizeof(g_UserMoreStats[0].StatsString),"%sAttackers %s: (%d Dmg taken)%s%s",ColorCode4,ColorCode1,DmgTaken,NewLine,AttacerBuffer);

	g_UserMoreStats[id].StatsString[StrLen - 1] = '\0'; // remove the last new line
	g_UserMoreStats[id].StatsStringMade = true;
	g_UserMoreStats[id].StatsToShow = true;
}

void Stats::ClearMoreStats(int id)
{
	for(int i=1;i<=PluginCore.GetEngineUtils()->MaxClients;i++)
	{
		g_UserMoreStats[id].Players[i].DmgGiven = 0;
		g_UserMoreStats[id].Players[i].DmgTaken = 0;
		g_UserMoreStats[id].Players[i].Killed = false;
		g_UserMoreStats[id].Players[i].Killer = false;
	}

	g_UserMoreStats[id].StatsStringMade = false;
	g_UserMoreStats[id].StatsToShow = false;
	g_UserMoreStats[id].HasShownStats = false;
}

void Stats::AddPlayerShot(int id,int WeaponIndex)
{
	stWeaponStatsReturn WStatsR = GetWeaponStatsIndex(id,WeaponIndex);
	
	if(WStatsR.ArrayIndex == -1)
		return;

	WStatsR.WeaponStats->Miss++;
	WStatsR.WeaponStats->Shots++;
	g_UserStats[id].WeaponStats[WStatsR.ArrayIndex] = WStatsR.WeaponStats;
}

void Stats::AddPlayerAttackKill(int id,bool Headshot,int WeaponIndex,bool TeamKill)
{
	g_UserStats[id].HasDoneSomething = true;
	stWeaponStatsReturn WStatsR = GetWeaponStatsIndex(id,WeaponIndex);
	
	if(WStatsR.ArrayIndex == -1)
		return;

	WStatsR.WeaponStats->Kills++;
	if(Headshot)
		WStatsR.WeaponStats->HKills++;

	if(TeamKill)
		WStatsR.WeaponStats->TKills++;

	g_UserStats[id].WeaponStats[WStatsR.ArrayIndex] = WStatsR.WeaponStats;
}

void Stats::AddPlayerHeadShot(int id,int WeaponIndex)
{
	g_UserStats[id].HasDoneSomething = true;
	stWeaponStatsReturn WStatsR = GetWeaponStatsIndex(id,WeaponIndex);

	if(WStatsR.ArrayIndex == -1)
		return;

	WStatsR.WeaponStats->Kills--; // This function is only called in mods where we dont know if the kill realy was done via a headshot and have to guess. So the death event is also going to add a kill that we remove now
	WStatsR.WeaponStats->HKills++;

	g_UserStats[id].WeaponStats[WStatsR.ArrayIndex] = WStatsR.WeaponStats;
}

void Stats::AddPlayerAttack(int id, int Dmg, int HitGroup,int WeaponIndex)
{
	g_UserStats[id].HasDoneSomething = true;
	int ArrayIndex;		// This is the index we are gonna use in the WeaponStats array.

	stWeaponStatsReturn WStatsR = GetWeaponStatsIndex(id,WeaponIndex);
	stWeaponStats *WeaponStats = WStatsR.WeaponStats;
	ArrayIndex = WStatsR.ArrayIndex;

	WeaponStats->Damage += Dmg;
	WeaponStats->Hits++;
	WeaponStats->Miss--; // Since this shot clearly hit someone, we need to remove 1 miss. As we first record every shot a miss, then remove the miss when its did damage

	switch(HitGroup)
	{
	case HITGROUP_HEAD:
		WeaponStats->HitBoxHead++;
		break;
	case HITGROUP_CHEST:
		WeaponStats->HitBoxChest++;
		break;
	case HITGROUP_LEFTARM:
		WeaponStats->HitBoxLeftArm++;
		break;
	case HITGROUP_RIGHTARM:
		WeaponStats->HitBoxRightArm++;
		break;
	case HITGROUP_LEFTLEG:
		WeaponStats->HitBoxLeftLeg++;
		break;
	case HITGROUP_RIGHTLEG:
		WeaponStats->HitBoxLeftLeg++;
		break;
	}
	g_UserStats[id].WeaponStats[ArrayIndex] = WeaponStats;
}

void Stats::LogPlayerStats(int id)
{
	if(!g_UserStats[id].HasDoneSomething)
		return;

	char StartOfLog[200];
	_snprintf(StartOfLog,199,"\"%s<%d><%s><%s>\" triggered \"weaponstats",g_MSCore.GetEngineUtils()->GetPlayerName(id),g_UserInfo[id].Userid,g_UserInfo[id].Steamid,g_MSCore.GetEngineUtils()->GetTeamName(id));
	
	for(int i=1;i<MAX_WEAPONS;i++)
	{
		stWeaponStats *WeaponStats = g_UserStats[id].WeaponStats[i];
		
		if(!WeaponStats->WeaponUsed)
			break;
		

		g_MSCore.GetEngineUtils()->LogPrint("%s\" (weapon \"%s\") (shots \"%d\") (hits \"%d\") (kills \"%d\") (headshots \"%d\") (tks \"%d\") (damage \"%d\") (deaths \"%d\")",StartOfLog,g_MSCore.GetWeaponList()->GetWeaponName(WeaponStats->WeaponIndex),WeaponStats->Shots,WeaponStats->Hits,WeaponStats->Kills,WeaponStats->HKills,WeaponStats->TKills,WeaponStats->Damage,WeaponStats->Death,WeaponStats->Death);
		g_MSCore.GetEngineUtils()->LogPrint("%s2\" (weapon \"%s\") (head \"%d\") (chest \"%d\") (stomach \"%d\") (leftarm \"%d\") (rightarm \"%d\") (leftleg \"%d\") (rightleg \"%d\")",StartOfLog,g_MSCore.GetWeaponList()->GetWeaponName(WeaponStats->WeaponIndex),WeaponStats->HitBoxHead,WeaponStats->HitBoxChest,WeaponStats->HitBoxStomach,WeaponStats->HitBoxLeftArm,WeaponStats->HitBoxRightArm,WeaponStats->HitBoxLeftLeg,WeaponStats->HitBoxRightLeg);
		
		FastResetWeaponStats(g_UserStats[id].WeaponStats[i]);
	}	
	g_UserStats[id].HasDoneSomething = false;
	g_UserMoreStats[id].StatsStringMade = false; // This in effect resets the more stats
}
/*
L 06/18/2006 - 17:54:52: "Perry<3><BOT><TERRORIST>" triggered "weaponstats" (weapon "galil") (shots "4") (hits "4") (kills "2") (headshots "1") (tks "0") (damage "199") (death "0")
L 06/18/2006 - 17:54:52: "Perry<3><BOT><TERRORIST>" triggered "weaponstats2" (weapon "galil") (head "1") (chest "0") (stomach "0") (leftarm "1") (rightarm "0")(leftleg "0") (rightleg "0")
*/
/*
From Mani
L 06/18/2006 - 15:26:27: "Pinkey<226><STEAM_0:0:7295768><CT>" triggered "weaponstats" (weapon "ak47") (shots "0") (hits "0") (kills "0") (headshots "0") (tks "0") (damage "0") (deaths "1")
L 06/18/2006 - 15:26:27: "Pinkey<226><STEAM_0:0:7295768><CT>" triggered "weaponstats2" (weapon "ak47") (head "0") (chest "0") (stomach "0") (leftarm "0") (rightarm "0") (leftleg "0") (rightleg "0")

// From statsmeminimum
L 06/11/2006 - 19:49:40: "Eric<22><BOT><TERRORIST>" triggered "weaponstats" (weapon "galil") (shots "4") (hits "2") (kills "1") (headshots "1") (tks "0") (damage "108") (deaths "0")
L 06/11/2006 - 19:49:40: "Eric<22><BOT><TERRORIST>" triggered "weaponstats2" (weapon "galil") (head "1") (chest "1") (stomach "0") (leftarm "0") (rightarm "0")(leftleg "0") (rightleg "0")
*/
const char* Stats::GetHitGroupName(int HitGroup)
{
	switch(HitGroup)
	{
	case HITGROUP_HEAD:
		return "head";
	case HITGROUP_CHEST:
		return "stomach";
	case HITGROUP_LEFTARM:
		return "leftarm";
	case HITGROUP_RIGHTARM:
		return "rightarm";
	case HITGROUP_LEFTLEG:
		return "leftleg";
	case HITGROUP_RIGHTLEG:
		return "rightleg";
	}
	return "ERROR";
}

stWeaponStatsReturn Stats::GetWeaponStatsIndex(int id,int WeaponIndex)
{
	stWeaponStatsReturn WStatsR;
	WStatsR.ArrayIndex  = -1;

	if(g_UserStats[id].HasDoneSomething == false)
	{
		if(g_UserStats[id].WeaponStats[1] == NULL)
			g_UserStats[id].WeaponStats[1] = new stWeaponStats;

		g_UserStats[id].HasDoneSomething = true;
		WStatsR.WeaponStats = g_UserStats[id].WeaponStats[1];
		WStatsR.WeaponStats->WeaponUsed = true;
		WStatsR.WeaponStats->WeaponIndex = WeaponIndex;

		WStatsR.ArrayIndex = 0;
		return WStatsR;
	}
	else
	{
		for(int i=1;i<MAX_WEAPONS;i++)
		{
			if(!g_UserStats[id].WeaponStats[i]->WeaponUsed) // This index is not in use, so we use this one.
			{
				WStatsR.WeaponStats = g_UserStats[id].WeaponStats[i];
				WStatsR.WeaponStats->WeaponUsed = true;
				WStatsR.WeaponStats->WeaponIndex = WeaponIndex;
				WStatsR.ArrayIndex = i;	// We found the old use of the weapon
				return WStatsR;	
			}
			if(WeaponIndex == g_UserStats[id].WeaponStats[i]->WeaponIndex)
			{
				WStatsR.WeaponStats = g_UserStats[id].WeaponStats[i];
				WStatsR.ArrayIndex = i;	// We found the old use of the weapon
				return WStatsR;
			}
		}
	}
	return WStatsR;
}

void Stats::ResetMoreStats(int id)
{
	for(int i=1;i<=PluginCore.GetEngineUtils()->MaxClients;i++)
	{
		g_UserMoreStats[id].Players[i].DmgGiven = 0;
		g_UserMoreStats[id].Players[i].DmgTaken = 0;
	}
}

void Stats::ResetStatsArray(int id)
{
	g_UserStats[id].HasDoneSomething = false;

	for(int i=1;i<=PluginCore.GetEngineUtils()->MaxClients;i++)
	{
		g_UserMoreStats[id].Players[i].DmgGiven = 0;
		g_UserMoreStats[id].Players[i].DmgTaken = 0;
	}

	for(int i=1;i<MAX_WEAPONS;i++)
	{
		stWeaponStats *WeaponStats = g_UserStats[id].WeaponStats[i];
		if(!WeaponStats)
			WeaponStats = new stWeaponStats;

		WeaponStats->Damage = 0;
		WeaponStats->HitBoxChest = 0;
		WeaponStats->HitBoxHead = 0;
		WeaponStats->HitBoxLeftArm = 0;
		WeaponStats->HitBoxLeftLeg = 0;
		WeaponStats->HitBoxRightArm = 0;
		WeaponStats->HitBoxRightLeg = 0;
		WeaponStats->HitBoxStomach = 0;
		WeaponStats->Hits = 0;
		WeaponStats->Kills= 0;
		WeaponStats->Miss = 0 ;
		WeaponStats->WeaponIndex = 0;
		WeaponStats->Shots = 0;
		WeaponStats->HKills = 0;
		WeaponStats->TKills = 0;
		WeaponStats->Death = 0;
		WeaponStats->WeaponUsed = false;

		g_UserStats[id].WeaponStats[i] = WeaponStats;
	}
}

void Stats::FastResetWeaponStats(stWeaponStats *WeaponStats)
{
	WeaponStats->Damage = 0;
	WeaponStats->HitBoxChest = 0;
	WeaponStats->HitBoxHead = 0;
	WeaponStats->HitBoxLeftArm = 0;
	WeaponStats->HitBoxLeftLeg = 0;
	WeaponStats->HitBoxRightArm = 0;
	WeaponStats->HitBoxRightLeg = 0;
	WeaponStats->HitBoxStomach = 0;
	WeaponStats->Hits = 0;
	WeaponStats->Kills= 0;
	WeaponStats->Miss = 0 ;
	WeaponStats->WeaponIndex = 0;
	WeaponStats->Shots = 0;
	WeaponStats->HKills = 0;
	WeaponStats->TKills = 0;
	WeaponStats->Death = 0;
	WeaponStats->WeaponUsed = false;
}