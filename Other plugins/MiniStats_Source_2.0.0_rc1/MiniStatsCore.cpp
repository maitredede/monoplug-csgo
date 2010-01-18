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
#include "cvars.h"
#include "const.h"
#include <time.h>
#include "SteamIDList.h"
#include "EventListeners.h"
#include "recipientfilters.h"
#include "HLXMenu.h"
#include <bitbuf.h>

HLStatsIngame g_MSCore;
BATMenuMngr *m_MenuMngr;
stPlayerInfo g_UserInfo[MAXPLAYERS+1];
stPlayerStats g_UserStats[MAXPLAYERS+1];
stMorePlayerStats g_UserMoreStats[MAXPLAYERS+1];

EventListenPlayerWeaponFire *m_EventPlayerFire;
EventListenPlayerHurt *m_EventPlayerHurt;
EventListenPlayerDeath *m_EventPlayerDeath;
Stats *m_Stats;
SteamIDList *m_SteamIDList;
CCommand g_LastCCommand;

// Menu related things
int g_HLXMenu;
extern int g_OldMenuShowTimes[MAX_PLAYERS];
extern bool g_RequiresMenuReShow;

// Chat commands from hlstatsx
const char *gSayCommands[] = {"rank","/rank","stats","/stats","statsme","/statsme","top10","/top10","session","/session","session_data","/session_data","next","/next","clans","/clans","status","/status","weapons","/weapons","accuracy","/accuracy","targets","/targets","kills","/kills","hlx_hideranking","/hlx_hideranking","cheaters","/cheaters","help","/help","kpd","/kpd"};
unsigned long *gSayCommandsHash;
int gSayCommandsCount;

extern int g_LastPlayerIndex;


SH_DECL_HOOK6(IServerGameDLL, LevelInit, SH_NOATTRIB, 0, bool, char const *, char const *, char const *, char const *, bool, bool);
SH_DECL_HOOK3_void(IServerGameDLL, ServerActivate, SH_NOATTRIB, 0, edict_t *, int, int);
SH_DECL_HOOK1_void(IServerGameDLL, GameFrame, SH_NOATTRIB, 0, bool);
SH_DECL_HOOK0_void(IServerGameDLL, LevelShutdown, SH_NOATTRIB, 0);
//SH_DECL_HOOK2_void(IServerGameClients, ClientActive, SH_NOATTRIB, 0, edict_t *, bool);
SH_DECL_HOOK1_void(IServerGameClients, ClientDisconnect, SH_NOATTRIB, 0, edict_t *);
SH_DECL_HOOK2_void(IServerGameClients, ClientPutInServer, SH_NOATTRIB, 0, edict_t *, char const *);
SH_DECL_HOOK1_void(IServerGameClients, SetCommandClient, SH_NOATTRIB, 0, int);
//SH_DECL_HOOK1_void(IServerGameClients, ClientSettingsChanged, SH_NOATTRIB, 0, edict_t *);
SH_DECL_HOOK5(IServerGameClients, ClientConnect, SH_NOATTRIB, 0, bool, edict_t *, const char*, const char *, char *, int);
SH_DECL_HOOK2(IGameEventManager2, FireEvent, SH_NOATTRIB, 0, bool, IGameEvent *, bool);

#if ORANGEBOX_ENGINE == 1
SH_DECL_HOOK2_void(IServerGameClients, NetworkIDValidated, SH_NOATTRIB, 0, const char *, const char *);
SH_DECL_HOOK2_void(IServerGameClients, ClientCommand, SH_NOATTRIB, 0, edict_t *, const CCommand &);
SH_DECL_HOOK1_void(ConCommand, Dispatch, SH_NOATTRIB, 0,const CCommand &);
#else
SH_DECL_HOOK1_void(IServerGameClients, ClientCommand, SH_NOATTRIB, 0, edict_t *);
//SH_DECL_HOOK2_void(IServerGameClients, NetworkIDValidated, SH_NOATTRIB, 0, const char *, const char *);
SH_DECL_HOOK0_void(ConCommand, Dispatch, SH_NOATTRIB, 0);
#endif

#if ORANGEBOX_ENGINE == 1
ICvar *GetICVar()
{
	return (ICvar *)((g_SMAPI->GetEngineFactory())(CVAR_INTERFACE_VERSION, NULL));
}
#endif

PLUGIN_EXPOSE(HLStatsIngame, g_MSCore);

HLStatsIngame::HLStatsIngame()
{
	m_EventPlayerDeath = new EventListenPlayerDeath;
	m_EventPlayerHurt = new EventListenPlayerHurt;
	m_EventPlayerFire = new EventListenPlayerWeaponFire;
	m_Stats = new Stats;
	m_SteamIDList = new SteamIDList;

	//-- Build a hash list of chat commands
	gSayCommandsCount = sizeof(gSayCommands)/sizeof(*gSayCommands);
	gSayCommandsHash = new unsigned long[gSayCommandsCount];

	m_MenuMngr = new BATMenuMngr;
	HLXMenu *FTKMenu = new HLXMenu;
	g_HLXMenu = m_MenuMngr->RegisterMenu(FTKMenu);
	m_MenuMngr->SetMenuType(1);

	for (int i=0;i<gSayCommandsCount;i++)
	{
		gSayCommandsHash[i] = StrHash(gSayCommands[i],strlen(gSayCommands[i]));
	}
}
HLStatsIngame::~HLStatsIngame()
{
	delete [] gSayCommandsHash;

	delete m_EventPlayerDeath;
	delete m_EventPlayerHurt;
	delete m_EventPlayerFire;
	delete m_Stats;
	delete m_SteamIDList;
	delete m_MenuMngr;
}

void HLStatsIngame::ServerActivate(edict_t *pEdictList, int edictCount, int clientMax)
{
	// Called on mm 1.4 and regular hl2
#if PREFORMANCE_SAMPLER == 1
	char FileName[256],date[32];
	time_t td; time(&td);
	strftime(date, 31, "%m-%d-%Y", localtime(&td));
	_snprintf(FileName,255,"PreFormance_%s_%d.htm",date,td);
	PreformanceSampler::WriteLogFile(FileName);
#endif

	LoadPluginSettings(clientMax);
	RETURN_META(MRES_IGNORED);
}
bool HLStatsIngame::LevelInit(const char *pMapName, const char *pMapEntities, const char *pOldLevel, const char *pLandmarkName, bool loadGame, bool background)
{
	// Called on mm 1.6 and regular and orangebox
#if PREFORMANCE_SAMPLER == 1
	char FileName[256],date[32];
	time_t td; time(&td);
	strftime(date, 31, "%m-%d-%Y", localtime(&td));
	_snprintf(FileName,255,"PreFormance_%s_%d.htm",date,td);
	PreformanceSampler::WriteLogFile(FileName);
#endif

	LoadPluginSettings(GetGlobals()->maxClients);
	RETURN_META_VALUE(MRES_IGNORED, true);
}

void HLStatsIngame::ClientDisconnect(edict_t *pEntity)
{
	int id = m_Engine->IndexOfEdict(pEntity);
	
	if(g_UserMoreStats[id].ShowStats && g_UserMoreStats[id].PlayerWasInList == false)
	{
		m_SteamIDList->AddSteamID(g_UserInfo[id].Steamid);
	}
	else if(g_UserMoreStats[id].ShowStats == false && g_UserMoreStats[id].PlayerWasInList == true)
	{
		m_SteamIDList->RemoveSteamID(g_UserInfo[id].Steamid);
	}

	m_MenuMngr->ResetPlayer(id);
	m_Stats->LogPlayerStats(id);
	g_MSCore.GetEngineUtils()->FastSetUserConnected(id,false);
	RETURN_META(MRES_IGNORED);
}
void HLStatsIngame::ClientPutInServer(edict_t *pEntity, char const *playername)
{
	int id = m_Engine->IndexOfEdict(pEntity);
	m_EngineUtils->FastSetUserConnected(id,true);

	g_UserInfo[id].PlayerEdict = pEntity;
	g_UserInfo[id].Userid = m_Engine->GetPlayerUserId(pEntity);

	sprintf(g_UserInfo[id].Steamid,m_Engine->GetPlayerNetworkIDString(pEntity));

	if(strcmp(g_UserInfo[id].Steamid,"BOT") == 0 || strcmp(g_UserInfo[id].Steamid,"HLTV") == 0 ) // || strcmp(g_UserInfo[id].steamid,"STEAM_ID_LAN") == 0
	{
		g_UserInfo[id].ShowStats = false;
		g_UserInfo[id].IsBot = true;
	}
	else
	{
		g_UserInfo[id].IsBot = false;
		g_UserInfo[id].ShowStats = true;

		if(m_SteamIDList->IsSteamIDInList(g_UserInfo[id].Steamid))
		{
			g_UserMoreStats[id].PlayerWasInList = true;
			g_UserMoreStats[id].ShowStats = true;
		} else {
			g_UserMoreStats[id].PlayerWasInList = false;
			g_UserMoreStats[id].ShowStats = false;
		}
	}	

	if(g_LastPlayerIndex == id)
		g_LastPlayerIndex = -1;

	m_Stats->ResetStatsArray(id);
	RETURN_META(MRES_IGNORED);
}
void HLStatsIngame::SetCommandClient(int index)
{
	g_IndexOfLastUserCmd = index + 1;
	RETURN_META(MRES_IGNORED);
}
void HLStatsIngame::GameFrame(bool simulating) // We dont hook GameFrame, we leave it in here incase we ever need some timer system 
{
	PreSamplerStart("GameFrame")
	float CurTime = GetGlobals()->curtime;

	if(CurTime - g_FlTime >= TASK_CHECKTIME)
	{
		g_FlTime = CurTime;

		BATMenuMngr::ReShowOldMenus();
	}
	PreSamplerEnd("HLStatsIngame::GameFrame")
	RETURN_META(MRES_IGNORED);
}
#if ORANGEBOX_ENGINE == 1
void HLStatsIngame::ClientCommand(edict_t *pEntity, const CCommand &args)
{
	g_LastCCommand = args;
#else
void HLStatsIngame::ClientCommand(edict_t *pEntity)
{
#endif	
	PreSamplerStart("HLStatsIngame::ClientCommand")

	const char *Command = g_LastCCommand.Arg(0);
	int id = m_Engine->IndexOfEdict(pEntity);

	if (strcmp(Command,"menuselect") == 0)
	{
		if(m_MenuMngr->HasMenuOpen(id))
		{
			const char *arg1 = g_LastCCommand.Arg(1);
			//catch menu commands
			if (arg1)
			{
				int arg = atoi(arg1);
				if(arg < 1 || arg > 10) // Make sure makes no invalid selection.
					return;

				m_MenuMngr->MenuChoice(id, arg);
			}
			PreSamplerEnd("HLStatsIngame::ClientCommand")
			RETURN_META(MRES_SUPERCEDE);
		}

		g_OldMenuShowTimes[id] = 0;
		PreSamplerEnd("HLStatsIngame::ClientCommand")
		RETURN_META(MRES_IGNORED);
	}
	PreSamplerEnd("HLStatsIngame::ClientCommand")
	RETURN_META(MRES_IGNORED);
}

void HLStatsIngame::FireGameEvent(IGameEvent *event)
{
	if (!event || !event->GetName())
		return;

	PreSamplerStart("HLStatsIngame::FireGameEvent")
	const char *EventName = event->GetName();
	
	if(g_ModSettings.GameMod == MOD_CSTRIKE && strcmp(EventName,"round_end") == 0 || 
		g_ModSettings.GameMod == MOD_DOD && strcmp(EventName,"dod_round_start") == 0 || 
		g_ModSettings.GameMod == MOD_TF2 && strcmp(EventName,"teamplay_round_win") == 0 )
	{
		for(int i=1;i<=PluginCore.GetEngineUtils()->MaxClients;i++) if(m_EngineUtils->FastIsUserConnected(i))
		{
			m_Stats->LogPlayerStats(i);
		}
		
		if(m_Stats->MoreStatsMode > 0 && g_HLSVars.GetMoreStatsReportTime() != 0)
		{
			if(g_LastRoundShow >= g_HLSVars.GetMoreStatsReportTime() && g_HLSVars.GetMoreStatsReportTime() != 0)
			{
				for (int i=1;i<=PluginCore.GetEngineUtils()->MaxClients;i++) if(m_EngineUtils->FastIsUserConnected(i))
				{
					if(!g_UserMoreStats[i].ShowStats)
						m_EngineUtils->MessagePlayer(i,"%s If you want to see more stats in game when you die or at round end: 'say /morestats'",StartStringTag);
				}			
				g_LastRoundShow = 0;
			}
			else
				g_LastRoundShow++;
		}

		if(g_ModSettings.GameMod == MOD_CSTRIKE)
		{
			if(Stats::MoreStatsMode == 2)
				m_Stats->ShowMoreStats();
		}
		else if(Stats::MoreStatsMode > 0 && g_ModSettings.GameMod != MOD_CSTRIKE)
		{
			m_Stats->ShowMoreStats();
			m_Stats->ClearMoreStats();
		}
	}
	else if(strcmp(EventName,"round_freeze_end") == 0)
	{
		if(Stats::MoreStatsMode == 1)
			m_Stats->ShowMoreStats();

		m_Stats->ClearMoreStats();
	}
	PreSamplerEnd("HLStatsIngame::FireGameEvent")
}
#if ORANGEBOX_ENGINE == 1
void HLStatsIngame::ClientSay(const CCommand &args)
{
	g_LastCCommand = args;
#else
void HLStatsIngame::ClientSay()
{
#endif

	PreSamplerStart("HLStatsIngame::ClientSay")
	char FirstWord[48];
	const char *pFirstWord;

	_snprintf(FirstWord,47,"%s",g_LastCCommand.Arg(1));
	int id = g_IndexOfLastUserCmd;

	if(g_ModSettings.GameMod == MOD_EMPIRES && strcmp(g_LastCCommand.Arg(0),"say_team") == 0)
	{
		FirstWord[0] = ' ';
		FirstWord[1] = ' ';
		FirstWord[2] = ' ';
		FirstWord[3] = ' ';
		g_MSCore.StrTrim(FirstWord);

		pFirstWord = (char *)FirstWord;
	}	
	else if(g_ModSettings.GameMod == MOD_INSURGENCY)
	{
		int SayType = atoi(g_LastCCommand.Arg(1));

		if(SayType != 1 && SayType != 2)
			return; // This was some odd say type like squad / server say or whatever we dont care but we dont handle it.

		_snprintf(FirstWord,191,"%s",g_LastCCommand.Arg(3));
		pFirstWord = (char *)FirstWord;
	}
	else 
		pFirstWord = (char *)FirstWord;

	if(g_BlockSayChat)
	{
		unsigned long FirstWordHash = StrHash(pFirstWord,strlen(pFirstWord));

		for(int i=0;i<gSayCommandsCount;i++)
		{
			if(gSayCommandsHash[i] == FirstWordHash && strcmp(gSayCommands[i],pFirstWord) == 0)
			{
				if(FirstWord[0] == '/')
					g_MSCore.GetEngineUtils()->FakeClientSay(id,"%s",FirstWord);
				else
					g_MSCore.GetEngineUtils()->FakeClientSay(id,"/%s",FirstWord);
				
				RETURN_META(MRES_SUPERCEDE);
				break;
			}
		}			
	}
	
	if(strcmp(pFirstWord,"/hlx_menu") == 0 || strcmp(pFirstWord,"hlx_menu") == 0 || strcmp(pFirstWord,"hlxmenu") == 0 ||  strcmp(pFirstWord,"/hlxmenu") == 0)
	{
		m_MenuMngr->ShowMenu(id,g_HLXMenu);
		
		if(g_BlockSayChat)
		{
			PreSamplerEnd("HLStatsIngame::ClientSay")
			RETURN_META(MRES_SUPERCEDE);
		}
	}
	else if(strcmp(pFirstWord,"/morestats") == 0 || strcmp(pFirstWord,"morestats") == 0 || strcmp(pFirstWord,"!morestats") == 0)
	{
		if(g_UserMoreStats[id].ShowStats)
		{
			g_MSCore.GetEngineUtils()->MessagePlayer(id,"%s Showing of MoreStats is now disabled",StartStringTag);
			g_UserMoreStats[id].ShowStats = false;
		}
		else
		{
			if(m_Stats->MoreStatsMode == 1)
				g_MSCore.GetEngineUtils()->MessagePlayer(id,"%s Showing of MoreStats is now enabled, will show on death and round start",StartStringTag);
			else
				g_MSCore.GetEngineUtils()->MessagePlayer(id,"%s Showing of MoreStats is now enabled, will show on death or round end",StartStringTag);

			g_UserMoreStats[id].ShowStats = true;
		}
	}
	else if(strcmp(pFirstWord,"/nostats") == 0)
	{
		if(g_UserInfo[id].ShowStats)
		{
			g_MSCore.GetEngineUtils()->MessagePlayer(id,"%s Showing of ingame stats is now disabled, enable again by 'say /nostats'",StartStringTag);
			g_UserInfo[id].ShowStats = false;
		}
		else
		{
			g_MSCore.GetEngineUtils()->MessagePlayer(id,"%s Showing of ingame stats is now enabled",StartStringTag);
			g_UserInfo[id].ShowStats = true;
		}
	}
	PreSamplerEnd("HLStatsIngame::ClientSay")
}

void HLStatsIngame::LoadPluginSettings(int clientMax)
{	
	if(clientMax == 0)
		return; // It was called way to early

	if(g_FirstLoad == false)
	{
		g_FirstLoad = true;
		m_WeaponList = new WeaponList;
		ModInfo *pModInfo = new ModInfo;
		pModInfo->GetModInformation(m_ServerDll,m_Engine);
		delete pModInfo;

		m_SteamIDList->ReadSteamIDFile();
		g_MSCore.GetEngineUtils()->MaxClients = clientMax;

		if(!m_GameEventManager->AddListener(m_EventPlayerDeath,"player_death",true))
			Log("Hooking player_death failed failed");

		if(!m_GameEventManager->AddListener(m_EventPlayerHurt,"player_hurt",true))
			Log("Hooking player_hurt failed failed");

		if(!m_GameEventManager->AddListener(m_EventPlayerFire,"weapon_fire",true) && g_ModSettings.GameMod != MOD_TF2  && g_ModSettings.GameMod != MOD_DOD)
			Log("Hooking player weapon_fire failed");
		
		if(g_ModSettings.GameMod == MOD_CSTRIKE)
		{
			m_GameEventManager->AddListener(this,"round_end",true);
			m_GameEventManager->AddListener(this,"team_info",true);
			m_GameEventManager->AddListener(this,"round_freeze_end",true);
		}
		else if(g_ModSettings.GameMod == MOD_DOD)
		{
			m_GameEventManager->AddListener(this,"dod_round_start",true);		
			m_GameEventManager->AddListener(m_EventPlayerFire,"dod_stats_weapon_attack",true);

			//g_ModHasKnownRoundReset = true;
		}
		else if(g_ModSettings.GameMod == MOD_TF2)
		{
			m_GameEventManager->AddListener(this,"teamplay_round_win",true);
		}

		HLSVars::SetupCallBacks();

		if(g_ModSettings.GameMod == MOD_INSURGENCY)
			pSayCmd = HookConsoleCmd("say2");	
		else
			pSayCmd = HookConsoleCmd("say");

		pSayTeamCmd = HookConsoleCmd("say_team");
	}
	for(int i=1;i<=PluginCore.GetEngineUtils()->MaxClients;i++)
		g_MSCore.GetEngineUtils()->FastSetUserConnected(i,false);

	m_MenuMngr->ClearForMapchange();

	g_FlTime = GetGlobals()->curtime;	
	g_BlockSayChat = g_HLSVars.BlockSayRank();
}


bool HLStatsIngame::Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late)
{
	PLUGIN_SAVEVARS();

#if ORANGEBOX_ENGINE == 1
	g_RequiresMenuReShow = true;

	GET_V_IFACE_ANY(GetServerFactory, m_ServerDll, IServerGameDLL, INTERFACEVERSION_SERVERGAMEDLL);
	GET_V_IFACE_ANY(GetServerFactory, m_ServerClients, IServerGameClients, INTERFACEVERSION_SERVERGAMECLIENTS);
	GET_V_IFACE_ANY(GetServerFactory, m_InfoMngr, IPlayerInfoManager, INTERFACEVERSION_PLAYERINFOMANAGER);

	GET_V_IFACE_CURRENT(GetEngineFactory, m_Engine, IVEngineServer, INTERFACEVERSION_VENGINESERVER);
	GET_V_IFACE_CURRENT(GetEngineFactory, m_GameEventManager, IGameEventManager2, INTERFACEVERSION_GAMEEVENTSMANAGER2);
	//GET_V_IFACE_CURRENT(GetEngineFactory, m_Helpers, IServerPluginHelpers, INTERFACEVERSION_ISERVERPLUGINHELPERS);
	//GET_V_IFACE_CURRENT(GetEngineFactory, m_Sound, IEngineSound, IENGINESOUND_SERVER_INTERFACE_VERSION);


	m_hooks.push_back(SH_ADD_HOOK(IServerGameDLL, LevelInit, m_ServerDll, SH_MEMBER(this, &HLStatsIngame::LevelInit), true));
	m_hooks.push_back(SH_ADD_HOOK(IServerGameDLL, ServerActivate, m_ServerDll, SH_MEMBER(this, &HLStatsIngame::ServerActivate), true));
	m_hooks.push_back(SH_ADD_HOOK(IServerGameDLL, GameFrame, m_ServerDll, SH_MEMBER(this, &HLStatsIngame::GameFrame), true));
	//m_hooks.push_back(SH_ADD_HOOK(IServerGameDLL, LevelShutdown, m_ServerDll, SH_MEMBER(this, &HLStatsIngame::LevelShutdown), false));
	//m_hooks.push_back(SH_ADD_HOOK(IServerGameClients, ClientActive, gameclients, SH_MEMBER(this, &g_BATCore::Hook_ClientActive), true));
	m_hooks.push_back(SH_ADD_HOOK(IServerGameClients, ClientDisconnect, m_ServerClients, SH_MEMBER(this, &HLStatsIngame::ClientDisconnect), true));
	m_hooks.push_back(SH_ADD_HOOK(IServerGameClients, ClientPutInServer, m_ServerClients, SH_MEMBER(this, &HLStatsIngame::ClientPutInServer), true));
	m_hooks.push_back(SH_ADD_HOOK(IServerGameClients, SetCommandClient, m_ServerClients, SH_MEMBER(this, &HLStatsIngame::SetCommandClient), true));
	//m_hooks.push_back(SH_ADD_HOOK(IServerGameClients, ClientSettingsChanged, gameclients, SH_MEMBER(this, &g_BATCore::Hook_ClientSettingsChanged), false));
	//m_hooks.push_back(SH_ADD_HOOK(IServerGameClients, ClientConnect, m_ServerClients, SH_MEMBER(this, &HLStatsIngame::ClientConnect), false));
	m_hooks.push_back(SH_ADD_HOOK(IServerGameClients, ClientCommand, m_ServerClients, SH_MEMBER(this, &HLStatsIngame::ClientCommand), false));
#else
	GET_V_IFACE_ANY(serverFactory, m_ServerDll, IServerGameDLL, INTERFACEVERSION_SERVERGAMEDLL);
	GET_V_IFACE_ANY(serverFactory, m_InfoMngr,IPlayerInfoManager, INTERFACEVERSION_PLAYERINFOMANAGER);
	GET_V_IFACE_ANY(serverFactory, m_ServerClients, IServerGameClients, INTERFACEVERSION_SERVERGAMECLIENTS);
	GET_V_IFACE_CURRENT(engineFactory, m_Engine, IVEngineServer, INTERFACEVERSION_VENGINESERVER);	
	GET_V_IFACE_CURRENT(engineFactory, m_GameEventManager, IGameEventManager2, INTERFACEVERSION_GAMEEVENTSMANAGER2);
	GET_V_IFACE_CURRENT(engineFactory, m_ICvar,ICvar, VENGINE_CVAR_INTERFACE_VERSION);
	//GET_V_IFACE_CURRENT(engineFactory, m_Helpers,IServerPluginHelpers, INTERFACEVERSION_ISERVERPLUGINHELPERS);
	//GET_V_IFACE_CURRENT(engineFactory, m_Sound,IEngineSound, IENGINESOUND_SERVER_INTERFACE_VERSION);

	//Hook LevelInit to our function
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, LevelInit, m_ServerDll, &g_MSCore, &HLStatsIngame::LevelInit, true);				//Hook ServerActivate to our function
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, m_ServerDll, &g_MSCore, &HLStatsIngame::ServerActivate, true);		//Hook GameFrame to our function
	//SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, m_ServerDll, &g_ForgiveTKCore, &HLStatsIngame::GameFrame, true);				//Hook LevelShutdown to our function -- this makes more sense as pre I guess
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, m_ServerClients, &g_MSCore, &HLStatsIngame::ClientDisconnect, true);		//Hook ClientPutInServer to our function
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, m_ServerClients, &g_MSCore, &HLStatsIngame::ClientPutInServer, true);	//Hook SetCommandClient to our function
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, SetCommandClient, m_ServerClients, &g_MSCore, &HLStatsIngame::SetCommandClient, true);		//Hook ClientSettingsChanged to our function
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientCommand, m_ServerClients, &g_MSCore, &HLStatsIngame::ClientCommand, false);	//This hook is a static hook, no member function
#endif
	
	m_Engine_CC = SH_GET_CALLCLASS(m_Engine); // invoking their hooks (when needed).
	SH_CALL(m_Engine_CC, &IVEngineServer::LogPrint)("[BAT] All hooks started!\n");

#if ORANGEBOX_ENGINE == 1
	g_pCVar = GetICVar();
	ConVar_Register(0,this); 
#else
	ConCommandBaseMgr::OneTimeInit(this);
	g_SMAPI->AddListener(g_PLAPI, this);

	GET_V_IFACE_CURRENT(engineFactory, g_pCVar, ICvar, VENGINE_CVAR_INTERFACE_VERSION);
#endif

	m_EngineUtils = new EngineUtils(m_Engine,m_InfoMngr);
	m_Engine_CC = SH_GET_CALLCLASS(m_Engine); // invoking their hooks (when needed).
	
	SH_CALL(m_Engine_CC, &IVEngineServer::LogPrint)("All hooks started!\n");
	if(late)
	{
		LoadPluginSettings(GetGlobals()->maxClients);
		g_MSCore.GetEngineUtils()->ServerCommand("echo [MS] Late load is not really supported, plugin will function properly after 1 map change",StartStringTag);
	}
	return true;
}

bool HLStatsIngame::Unload(char *error, size_t maxlen)
{
	m_SteamIDList->WriteSteamIDFile();

#if ORANGEBOX_ENGINE == 1
	for (size_t i = 0; i < m_hooks.size(); i++)
	{
		if (m_hooks[i] != 0)
		{
			SH_REMOVE_HOOK_ID(m_hooks[i]);
		}
	}
	m_hooks.clear();
#else
	SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, LevelInit, m_ServerDll, &g_MSCore, &HLStatsIngame::LevelInit, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, m_ServerDll, &g_MSCore, &HLStatsIngame::ServerActivate, true);
	//SH_REMOVE_HOOK_MEMFUNC(IServerGameDLL, GameFrame, m_ServerDll, &g_ForgiveTKCore, &HLStatsIngame::GameFrame, true);
	//SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientActive, m_ServerClients, &g_ForgiveTKCore, &HLStatsIngame::ClientActive, true);

	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, m_ServerClients, &g_MSCore, &HLStatsIngame::ClientDisconnect, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, m_ServerClients, &g_MSCore, &HLStatsIngame::ClientPutInServer, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, SetCommandClient, m_ServerClients, &g_MSCore, &HLStatsIngame::SetCommandClient, true);
	SH_REMOVE_HOOK_MEMFUNC(IServerGameClients, ClientCommand, m_ServerClients, &g_MSCore, &HLStatsIngame::ClientCommand, false);

	SH_REMOVE_HOOK_MEMFUNC(ConCommand, Dispatch, pSayCmd, &g_MSCore, &HLStatsIngame::ClientSay, true);
	SH_REMOVE_HOOK_MEMFUNC(ConCommand, Dispatch, pSayTeamCmd, &g_MSCore, &HLStatsIngame::ClientSay, true);
	//SH_ADD_HOOK_MEMFUNC(ConCommand, Dispatch, pTemp, &g_MSCore, &HLStatsIngame::ClientSay, false);
#endif

	m_GameEventManager->RemoveListener(m_EventPlayerDeath);
	m_GameEventManager->RemoveListener(m_EventPlayerHurt);
	m_GameEventManager->RemoveListener(m_EventPlayerFire);
	
	m_GameEventManager->RemoveListener(this);  // This will be your basic events that dont fire often, like round_end

	//this, sourcehook does not keep track of.  we must do this.
	SH_RELEASE_CALLCLASS(m_Engine_CC);
	return true;
}

void HLStatsIngame::AllPluginsLoaded()
{

}


void HLStatsIngame::Client_Authorized(int id)
{
#if HLX_DEBUG == 1
	ServerCommand("echo [ForgiveTK Debug]Client_Authorized: %d",id);
#endif
}
ConCommand *HLStatsIngame::HookConsoleCmd(const char *CmdName)
{
	ConCommandBase *pCmd = g_pCVar->GetCommands();
	ConCommand* pTemp;

	while (pCmd)
	{
		if (pCmd->IsCommand() && stricmp(pCmd->GetName(),CmdName) == 0) 
		{
			pTemp = (ConCommand *)pCmd;

#if ORANGEBOX_ENGINE == 1
			m_hooks.push_back(SH_ADD_HOOK(ConCommand, Dispatch, pTemp, SH_MEMBER(this, &HLStatsIngame::ClientSay), false));
#else
			SH_ADD_HOOK_MEMFUNC(ConCommand, Dispatch, pTemp, &g_MSCore, &HLStatsIngame::ClientSay, false);
#endif

			return pTemp;
		}

		pCmd = const_cast<ConCommandBase *>(pCmd->GetNext());
	}
	return NULL;
}
bool HLStatsIngame::RegisterConCommandBase(ConCommandBase *pVar)
{
	return META_REGCVAR(pVar);;
}
