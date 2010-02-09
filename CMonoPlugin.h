#ifndef _CMONOPLUGIN_H_
#define _CMONOPLUGIN_H_

#include "monoBase.h"
#include "CMonoHelpers.h"
#include "CBaseAccessor.h"
#include "sourcehook.h"
#include "CMonoConsole.h"
#include "CMonoCommand.h"
#include "utlmap.h"
#include "Events.h"
#include "CMonoPlayer.h"

namespace MonoPlugin
{
	class CMonoConsole;
	class CMonoPlayer;

	class CMonoPlugin : public ISmmPlugin, public IMetamodListener/*, public IGameEventListener2*/
	{
	public:
		bool Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late);
		bool Unload(char *error, size_t maxlen);
		bool Pause(char *error, size_t maxlen);
		bool Unpause(char *error, size_t maxlen);
		void AllPluginsLoaded();
	public:
		const char *GetAuthor();
		const char *GetName();
		const char *GetDescription();
		const char *GetURL();
		const char *GetLicense();
		const char *GetVersion();
		const char *GetDate();
		const char *GetLogTag();
	private:
		bool InitMono(const char* dllFile, const char* rootDir, char *error, size_t maxlen);
		bool StartMono(char *error, size_t maxlen);
		void AddHooks();
		void RemoveHooks();
		static bool Less_uint64(const uint64 &, const uint64 & );
		ConCommand* m_pSayCmd;
		ConCommand* m_pSayTeamCmd;
	public:
		int m_clientCommand;
	public:
		void Hook_Attach_ServerActivate();
		void Hook_Detach_ServerActivate();
		void Hook_Raise_ServerActivate(edict_t *pEdictList, int edictCount, int clientMax);

		void Hook_Attach_LevelShutdown();
		void Hook_Detach_LevelShutdown();
		void Hook_Raise_LevelShutdown();

		void Hook_Attach_ClientConnect();
		void Hook_Detach_ClientConnect();
		bool Hook_Raise_ClientConnect(edict_t *pEntity, const char *pszName, const char *pszAddress, char *reject, int maxrejectlen);

		void Hook_Attach_ClientDisconnect();
		void Hook_Detach_ClientDisconnect();
		void Hook_Raise_ClientDisconnect(edict_t *pEntity);

		void Hook_Attach_ClientPutInServer();
		void Hook_Detach_ClientPutInServer();
		void Hook_Raise_ClientPutInServer(edict_t *pEntity, char const *playername);
		void Hook_PlayerSay(const CCommand &command);
		void Hook_SetCommandClient(int index);
		void Hook_NetworkIDValidated(const char *pszUserName, const char *pszNetworkID);
	public:// Main
		MonoAssembly* m_assembly;
		MonoImage* m_image;

		MonoClass* m_class_ClsMain;
		MonoMethod* m_ClsMain_Init;
		MonoMethod* m_ClsMain_AllPluginsLoaded;

		MonoMethod* m_ClsMain_Shutdown;
		MonoMethod* m_ClsMain_GameFrame;
		MonoMethod* m_ClsMain_ConPrint;

		MonoMethod* m_ClsMain_Raise_ConVarChange;
		MonoMethod* m_ClsMain_Raise_ServerActivate;
		MonoMethod* m_ClsMain_Raise_LevelShutdown;
		MonoMethod* m_ClsMain_Raise_PlayerSay;

		MonoMethod* m_ClsMain_Raise_ClientDisconnect;
		MonoMethod* m_ClsMain_Raise_ClientPutInServer;
		MonoMethod* m_ClsMain_Raise_ClientConnect;
	public:
		MonoClass* m_Class_ClsPlayer;
		MonoClassField* m_Field_ClsPlayer_userId;
		MonoClassField* m_Field_ClsPlayer_isBot;
		MonoClassField* m_Field_ClsPlayer_isConnecting;
		MonoClassField* m_Field_ClsPlayer_isConnected;
		MonoClassField* m_Field_ClsPlayer_name;
		MonoClassField* m_Field_ClsPlayer_ip;
		MonoClassField* m_Field_ClsPlayer_steamId;
		MonoClassField* m_Field_ClsPlayer_isSteamValid;

		MonoClassField* m_Field_ClsPlayer_frag;
		MonoClassField* m_Field_ClsPlayer_death;
		MonoClassField* m_Field_ClsPlayer_language;
		MonoClassField* m_Field_ClsPlayer_avgLatency;
		MonoClassField* m_Field_ClsPlayer_timeConnected;
	public:
		MonoObject* m_main;
		CMonoConsole* m_console;
		uint64 m_nextConbaseId;
		CUtlMap<uint64, ConCommandBase*>* m_conbase;
		int m_EdictCount;
	public:
		//MonoObject* GetPlayerInfo(edict_t *pEntity);
		//MonoObject* GetPlayerInfo(int userid);
		//CMonoPlayer* GetPlayerByUserId(int index);
		//CMonoPlayer* CreatePlayer(int index, int userid, const char* name, const char* networkid, const char* address, bool bot, edict_t* entity);
		int m_clientMax;
		
		CMonoPlayerInfo m_players[MAXPLAYERS + 2];
		//void RefreshPlayerInfo(int index, const char* name, const char* address, edict_t *pEntity);
		//CMonoPlayer** m_allClients;

	public: //IMetamodListener stuff
		void OnVSPListening(IServerPluginCallbacks *iface);
	public:
		void Hook_GameFrame(bool simulating);
#if SOURCE_ENGINE >= SE_ORANGEBOX == 1
	int GetApiVersion() { return METAMOD_PLAPI_VERSION; }
#endif
	public: //Main events
		MonoMethod* m_ClsMain_event_server_spawn;
		IGameEventListener2* m_event_server_spawn;

		MonoMethod* m_ClsMain_event_server_shutdown;
		IGameEventListener2* m_event_server_shutdown;

		MonoMethod* m_ClsMain_event_player_connect;
		IGameEventListener2* m_event_player_connect;

		MonoMethod* m_ClsMain_event_player_disconnect;
		IGameEventListener2* m_event_player_disconnect;
	};
}

extern MonoPlugin::CMonoPlugin g_MonoPlugin;

PLUGIN_GLOBALVARS();

#if defined WIN32 || defined _WIN32
	#define MONOPLUG_DLLPATH "%s\\addons"
	#define MONOPLUG_DLLFILE "%s\\MonoPlug.Managed.dll"
#else
	#define MONOPLUG_DLLPATH "%s/addons"
	#define MONOPLUG_DLLFILE "%s/MonoPlug.Managed.dll"
#endif

#endif //_CMONOPLUGIN_H_
