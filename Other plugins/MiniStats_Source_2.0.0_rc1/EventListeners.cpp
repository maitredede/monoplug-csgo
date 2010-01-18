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

#include <ISmmPlugin.h>
#include <sourcehook/sourcehook.h>
#include <igameevents.h>
#include "ienginesound.h"
#include <iplayerinfo.h>
#include "convar.h"
#include "cvars.h"
#include "Stats.h"
#include "const.h"
#include "MiniStatsCore.h"
#include "EventListeners.h"

extern Stats *m_Stats;

void EventListenPlayerHurt::FireGameEvent(IGameEvent *event)
{
	if (!event || !event->GetName())
		return;

	PreSamplerStart("EventListenPlayerHurt::FireGameEvent")
	const char *EventName = event->GetName();

	int victim = g_MSCore.GetEngineUtils()->FindPlayer(event->GetInt("userid"));
	int attacker = g_MSCore.GetEngineUtils()->FindPlayer(event->GetInt("attacker"));
	if(attacker <= 0) // Hurt by some entity perhaps its the world 
		return;

	int HitGroup = event->GetInt("hitgroup");
	int WeaponIndex = g_MSCore.GetWeaponList()->GetWeaponIndex(event->GetString("weapon"));
	int Dmg;

	if(g_ModSettings.GameMod == MOD_DOD)
	{
		Dmg = event->GetInt("damage");
		int HealthLeft = event->GetInt("health");
		if(HealthLeft <= 0 && HitGroup == HITGROUP_HEAD)
		{
			//L 08/14/2008 - 01:03:36: "EKS<17><STEAM_0:1:20732><TERRORIST>" triggered "headshot"

			char FakeHeadShotLog[192];
			_snprintf(FakeHeadShotLog,191,"\"%s<%d><%s><%s>\" triggered \"headshot\"\n",PluginCore.GetEngineUtils()->GetPlayerName(attacker),g_UserInfo[attacker].Userid,g_UserInfo[attacker].Steamid,PluginCore.GetEngineUtils()->GetTeamName(attacker));
			PluginCore.GetEngineUtils()->LogPrint(FakeHeadShotLog);

			m_Stats->AddPlayerHeadShot(attacker,WeaponIndex);
		}
	}
	else
		Dmg = event->GetInt("dmg_health");

	if(WeaponIndex <= 0) // We dident find the correct weapon index. Screw this im going home
		return;

	m_Stats->AddPlayerAttack(attacker,Dmg,HitGroup,WeaponIndex);

	g_UserMoreStats[attacker].Players[victim].DmgGiven += Dmg;
	g_UserMoreStats[victim].Players[attacker].DmgTaken += Dmg;
	PreSamplerEnd("EventListenPlayerHurt::FireGameEvent")
}

void EventListenPlayerWeaponFire::FireGameEvent(IGameEvent *event)
{
	if (!event || !event->GetName())
		return;

	PreSamplerStart("EventListenPlayerWeaponFire::FireGameEvent")
	const char *EventName = event->GetName();
	int WeaponIndex;
	int id;

	if(g_ModSettings.GameMod == MOD_DOD)
	{
		id = g_MSCore.GetEngineUtils()->FindPlayer(event->GetInt("attacker"));
		const char *pWeaponName = g_MSCore.GetEngineUtils()->GetPlayerWeaponNamePretty(id);

		WeaponIndex = g_MSCore.GetWeaponList()->GetWeaponIndex(pWeaponName);
	}
	else
	{
		WeaponIndex = g_MSCore.GetWeaponList()->GetWeaponIndex(event->GetString("weapon"));
		id = g_MSCore.GetEngineUtils()->FindPlayer(event->GetInt("userid"));
	}

	if(WeaponIndex <= 0 || id == -1)
	{
		PreSamplerEnd("EventListenPlayerWeaponFire::FireGameEvent")
		return;
	}

	m_Stats->AddPlayerShot(id,WeaponIndex);
	PreSamplerEnd("EventListenPlayerWeaponFire::FireGameEvent")
}


void EventListenPlayerDeath::FireGameEvent(IGameEvent *event)
{
	if (!event || !event->GetName())
		return;

	PreSamplerStart("EventListenPlayerDeath::FireGameEvent")
	const char *EventName = event->GetName();

	int attacker = g_MSCore.GetEngineUtils()->FindPlayer(event->GetInt("attacker"));
	int victim = g_MSCore.GetEngineUtils()->FindPlayer(event->GetInt("userid"));
	int WeaponIndex = g_MSCore.GetWeaponList()->GetWeaponIndex(event->GetString("weapon"));
	bool Headshot = event->GetBool("headshot");

	if(WeaponIndex <= 0 || attacker <= 0 || victim < 0) 
	{
		PreSamplerEnd("EventListenPlayerDeath::FireGameEvent")
		return;
	}

	bool TeamKill = false;

	if(g_MSCore.GetEngineUtils()->GetUserTeam(victim) == g_MSCore.GetEngineUtils()->GetUserTeam(attacker))
		TeamKill = true;

	m_Stats->AddPlayerAttackKill(attacker,Headshot,WeaponIndex,TeamKill);

	g_UserMoreStats[attacker].Players[victim].Killed = true;
	g_UserMoreStats[victim].Players[attacker].Killer = true;

	if(Stats::MoreStatsMode != 0 && !g_UserInfo[victim].IsBot && g_UserMoreStats[victim].ShowStats)
	{
		m_Stats->ShowMoreStats(victim);
	}
	// We use both in logging and in hp left so we get it here.
	const char *pWeaponName = g_MSCore.GetEngineUtils()->GetPlayerWeaponNamePretty(attacker);
	if(!pWeaponName)
	{
		PreSamplerEnd("EventListenPlayerDeath::FireGameEvent");
		return;
	}

	if(g_HLSVars.ShowHPLeft() && !g_UserInfo[victim].IsBot && victim != attacker)
	{
		IPlayerInfo *pInfo = g_MSCore.PlayerInfo()->GetPlayerInfo(g_UserInfo[attacker].PlayerEdict);
		
		if(!pInfo)
		{
			PreSamplerEnd("EventListenPlayerDeath::FireGameEvent");
			return;
		}
		
		g_MSCore.GetEngineUtils()->MessagePlayer(victim,"%s %s has %d HP left after killing you with a %s",StartStringTag,g_MSCore.GetEngineUtils()->GetPlayerName(attacker),pInfo->GetHealth(),pWeaponName);
	}

	if(!g_ModSettings.KnownRoundRestart) // If the mod does not have a known round end we push weapon stats into the logs now
	{
		m_Stats->LogPlayerStats(victim);
	}
	/*
	//L 07/30/2008 - 19:06:27: "A7X<21><STEAM_0:0:5619311><TERRORIST>" killed "Guy1<22><STEAM_0:0:18134642><CT>" with "glock" (headshot) (attacker_position "431 797 -160") (victim_position "232 -189 -103")
	char StartOfLog[512];
	int len = _snprintf(StartOfLog,511,"\"%s<%d><%s><%s>\" killed ",g_MSCore.GetEngineUtils()->GetPlayerName(attacker),g_UserInfo[attacker].Userid,g_UserInfo[attacker].Steamid,g_MSCore.GetTeamName(attacker));
	len += _snprintf(&StartOfLog[len],511-len,"\"%s<%d><%s><%s>\" with  ",g_MSCore.GetEngineUtils()->GetPlayerName(victim),g_UserInfo[victim].Userid,g_UserInfo[victim].Steamid,g_MSCore.GetTeamName(victim));

	Vector AttackerPos = g_MSCore.GetPlayerOrigin(attacker);
	Vector VictimPos = g_MSCore.GetPlayerOrigin(victim);

	if(Headshot)
		_snprintf(&StartOfLog[len],511-len,"\"%s\" (headshot) (attacker_position \"%d %d %d\") (victim_position \"%d %d %d\")",pWeaponName,(int)AttackerPos.x,(int)AttackerPos.y,(int)AttackerPos.z,(int)VictimPos.x,(int)VictimPos.y,(int)VictimPos.z);
	else
		_snprintf(&StartOfLog[len],511-len,"\"%s\" (attacker_position \"%d %d %d\") (victim_position \"%d %d %d\")",pWeaponName,(int)AttackerPos.x,(int)AttackerPos.y,(int)AttackerPos.z,(int)VictimPos.x,(int)VictimPos.y,(int)VictimPos.z);

	g_MSCore.GetEngineUtils()->LogPrint(StartOfLog);
	*/

	PreSamplerEnd("EventListenPlayerDeath::FireGameEvent")
}