/* ======== SimpleStats ========
* Copyright (C) 2004-2008 Erling K. Sæterdal
* No warranties of any kind
*
* License: zlib/libpng
*
* Author(s): Erling K. Sæterdal ( EKS )
* Credits:
* Helping on misc errors/functions: BAILOPAN,sslice,devicenull,PMOnoTo,cybermind ( most who idle in #sourcemod on GameSurge really )
* ============================ */

#ifndef _INCLUDE_CONST_H
#define _INCLUDE_CONST_H

#include "edict.h"
#include "shareddefs.h"

#define HLX_VERSION	"2.0.0 RC1"
#define HLX_DEBUG 0

#define ORANGEBOX_ENGINE 1
#define HLX_REMOVEOLDSTEAMIDS 30	// The amount of days before steamids are removed from the list

#define RECONNECT_REMEMEBERCOUNT 5
#define TEAM_NOT_CONNECTED -1

#define MAXPLAYERS ABSOLUTE_PLAYER_LIMIT +1

#define StartStringTag "\x04[MS]\x01"

#define PluginCore g_MSCore
#define PluginSpesficLogPrint GetEngineUtils()->LogPrint

// ---------- GameFrame()
#define TASK_CHECKTIME 1.0			// This is how often the plugin checks if any tasks should be executed
#define MAX_WEAPONNAMELEN 20

#define MENUSYSTEM_NOTWORKING -1

// From Shareddefs.h
#define	HITGROUP_GENERIC	0
#define	HITGROUP_HEAD		1
#define	HITGROUP_CHEST		2
#define	HITGROUP_STOMACH	3
#define HITGROUP_LEFTARM	4	
#define HITGROUP_RIGHTARM	5
#define HITGROUP_LEFTLEG	6
#define HITGROUP_RIGHTLEG	7
#define HITGROUP_GEAR		10			// alerts NPC, but doesn't do damage or bleed (1/100th damage)

#ifdef WIN32
#define STEAMID_FILE "%s\\addons\\MiniStats\\OldSteamids.ini"
#else
#define STEAMID_FILE "%s/addons/MiniStats/OldSteamids.ini"
#endif

#if ORANGEBOX_ENGINE == 1
#define GetGlobals g_SMAPI->GetCGlobals
#define MAX_TEAMSINMENU 5
#else
#define GetGlobals g_SMAPI->pGlobals
#define FnChangeCallback_t FnChangeCallback
#define MAX_TEAMSINMENU 4
#endif

typedef struct 
{
	char Steamid[MAX_NETWORKID_LENGTH+1];
	int Userid;
	bool IsAlive;	// If the user is alive
	bool IsBot;		// is false if a player is a hltv or a bot
	edict_t *PlayerEdict;
	float LastHitTime;	// The last time his weapon hit someone
	bool ShowStats; // If the player wants to see stats related messages ingame
}stPlayerInfo;
typedef struct 
{
	int WeaponIndex;	// The weapon index of the current weapon.Better then strings at least
	bool WeaponUsed;

	int Damage;
	int Hits;
	int Miss;
	int Shots;
	int Kills;
	int HKills; // Headshot kills
	int TKills; // Team kills
	int Death; // No clue

	int HitBoxHead;
	int HitBoxChest;
	int HitBoxStomach;
	int HitBoxLeftArm;
	int HitBoxRightArm;
	int HitBoxLeftLeg;
	int HitBoxRightLeg;
}stWeaponStats;
typedef struct 
{
	int ArrayIndex;
	stWeaponStats *WeaponStats;
}stWeaponStatsReturn;
typedef struct 
{
	stWeaponStats *WeaponStats[MAX_WEAPONS+1];
	bool HasDoneSomething;
}stPlayerStats;
typedef struct 
{
	int DmgGiven; // Damage given to player
	int DmgTaken; // Damage taken from player
	bool Killed;  // If you killed this player
	bool Killer; // If this was the player that killed you.
}stMoreVictimStats;
typedef struct 
{
	bool ShowStats; // If the player wants to see his stats at round end & death
	stMoreVictimStats Players[MAXPLAYERS+1];
	char StatsString[512]; // This is where we save the string.
	bool StatsStringMade;  // If we have generated the stats string
	bool HasShownStats;	// If the stats already has been shown
	bool StatsToShow;	// If there are any stats to show the player
	bool PlayerWasInList; // If the player was in the player list that logs what users have selected before
}stMorePlayerStats;
typedef struct 
{
	char SteamID[MAX_NETWORKID_LENGTH+1];
	int FirstNum;
	int SecNum;
	int ThirdNum;
	uint SteamIDNum; // This is a number that combines all the FirstNum,SecNum and ThirdNum so a fast check can be done with only 1 if
	int DaysSizeLastUse; // This is used so we can check how long it was since the user last connected, and remove his steamid if needed
}stSteamIDInfo;
#endif //_INCLUDE_CONST_H