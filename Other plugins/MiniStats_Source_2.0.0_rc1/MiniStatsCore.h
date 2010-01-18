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

#ifndef _INCLUDE_SAMPLEPLUGIN_H
#define _INCLUDE_SAMPLEPLUGIN_H

#include <ISmmPlugin.h>
#include <sourcehook/sourcehook.h>
#include <igameevents.h>
#include "ienginesound.h"
#include <iplayerinfo.h>
#include <sh_vector.h>
#include "convar.h"
#include "cvars.h"
#include "ModInfo.h"
#include "const.h"
#include "StrUtils.h"
#include "BATMenu.h"
#include "EngineUtils.h"
#include "WeaponList.h"
#include "PreformanceSampler.h"

class HLStatsIngame : 
	public ISmmPlugin, 
	public IMetamodListener, 
	public IGameEventListener2, 
	public StrUtil,
	public IConCommandBaseAccessor
{
public:
	HLStatsIngame();
	~HLStatsIngame();

	bool Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late);
	bool Unload(char *error, size_t maxlen);
	void AllPluginsLoaded();
	bool Pause(char *error, size_t maxlen)
	{
		return true;
	}
	bool Unpause(char *error, size_t maxlen)
	{
		return true;
	}

#if ORANGEBOX_ENGINE == 1
	void ClientCommand(edict_t *pEntity, const CCommand &args);
	void ClientSay( const CCommand &command );
	
	int GetApiVersion() { return METAMOD_PLAPI_VERSION; }
#else
	void ClientCommand(edict_t *pEntity);
	void ClientSay();

	int GetApiVersion() { return PLAPI_VERSION; }
#endif

	const char *GetAuthor()
	{
		return "EKS";
	}
	const char *GetName()
	{
		return "MiniStats"; // HLX Ingame
	}
	const char *GetDescription()
	{
		return "Generates the extra stats needed for hlstatsx to do stats and helps showing them ingame, and can show killer hp/weapon and end of roundstats";
	}
	const char *GetURL()
	{
		return "http://www.TheXSoft.com";
	}
	const char *GetLicense()
	{
		return "zlib/libpng";
	}
	const char *GetVersion()
	{
		return HLX_VERSION;
	}
	const char *GetDate()
	{
		return __DATE__;
	}
	const char *GetLogTag()
	{
		return "MS";
	}

	//Called on ServerActivate.  Same definition as server plugins
	void ServerActivate(edict_t *pEdictList, int edictCount, int clientMax);
	bool LevelInit(const char *pMapName, char const *pMapEntities, char const *pOldLevel, char const *pLandmarkName, bool loadGame, bool background);
	void GameFrame(bool simulating);
	//Client disconnects - same as server plugins
	void ClientDisconnect(edict_t *pEntity);
	//Client is put in server - same as server plugins
	void ClientPutInServer(edict_t *pEntity, char const *playername);
	//Sets the client index - same as server plugins
	void SetCommandClient(int index);
	// Where the plugin catches events.
	void FireGameEvent(IGameEvent *event);

	virtual bool RegisterConCommandBase(ConCommandBase *pVar);

	IVEngineServer *GetEngine() { return m_Engine; }
	IPlayerInfoManager *PlayerInfo() { return m_InfoMngr; }
	IServerPluginHelpers *GetHelpers() {return m_Helpers;}
	IPlayerInfoManager *GetPlayerInfo() { return m_InfoMngr; }

	WeaponList *GetWeaponList() { return m_WeaponList; }
	EngineUtils *GetEngineUtils() { return m_EngineUtils; }

	HLSVars GetHLSVar() { return g_HLSVars; }
	void Client_Authorized(int id);	

private:
	IGameEventManager2 *m_GameEventManager;	
	SourceHook::CVector<int> m_hooks;
	IVEngineServer *m_Engine;
	IServerGameDLL *m_ServerDll;
	IServerGameClients *m_ServerClients;
	IPlayerInfoManager *m_InfoMngr;
	IServerPluginHelpers *m_Helpers;
	IEngineSound *m_Sound;
	ICvar *m_ICvar;
	SourceHook::CallClass<IVEngineServer> *m_Engine_CC;
	ConCommand *pSayCmd;
	ConCommand *pSayTeamCmd;
	WeaponList *m_WeaponList;
	EngineUtils *m_EngineUtils;

	void LoadPluginSettings(int clientMax);
	ConCommand *HookConsoleCmd(const char *CmdName);
	
	int g_IndexOfLastUserCmd;

	int g_AlivePlayers;
	int g_AliveBots;
	int g_BotCount;
	int g_PlayerCount;

	int g_LastRoundShow;
	float g_FlTime;
	
	bool g_HasMenuOpen[MAXPLAYERS+1];
	bool g_FirstLoad;
	bool g_FoundInterface;
	bool g_BlockSayChat;
};

extern HLStatsIngame g_MSCore;
extern HLSVars g_HLSVars;
extern stModSettings g_ModSettings;
extern stPlayerInfo g_UserInfo[MAXPLAYERS+1];
extern stPlayerStats g_UserStats[MAXPLAYERS+1];
extern stMorePlayerStats g_UserMoreStats[MAXPLAYERS+1];

PLUGIN_GLOBALVARS();

#if ORANGEBOX_ENGINE == 0
#define GetGlobals g_SMAPI->pGlobals

class CCommand
{
public:
	const char *ArgS()
	{
		return g_MSCore.GetEngine()->Cmd_Args();
	}
	int ArgC()
	{
		return g_MSCore.GetEngine()->Cmd_Argc();
	}

	const char *Arg(int index)
	{
		return g_MSCore.GetEngine()->Cmd_Argv(index);
	}
};
extern ICvar *g_pCVar;
#else
#define GetGlobals g_SMAPI->GetCGlobals
#endif
extern CCommand g_LastCCommand;
#endif //_INCLUDE_SAMPLEPLUGIN_H
