#ifndef _CMONOPLUG_H_
#define _CMONOPLUG_H_

//Mono
#include "Common.h"
#include "CMonoCommand.h"
#include "CMonoPlugAccessor.h"
#include "CMonoPlugListener.h"
#include "CMonoConsole.h"
#include "monoCallbacks.h"

#if defined WIN32 && !defined snprintf
#define snprintf _snprintf
#endif

class CMonoPlug : public ISmmPlugin, public IMetamodListener, public IGameEventListener2
{
public:
	bool Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late);
	bool Unload(char *error, size_t maxlen);
	//void AllPluginsLoaded();
	bool Pause(char *error, size_t maxlen)
	{
		return true;
	}
	bool Unpause(char *error, size_t maxlen)
	{
		return true;
	}
//public:
//	int GetApiVersion() { return PLAPI_VERSION; }
public:
	const char *GetAuthor()
	{
		return "MaitreDede";
	}
	const char *GetName()
	{
		return "MonoPlug";
	}
	const char *GetDescription()
	{
		return "Plugin engine usine Mono";
	}
	const char *GetURL()
	{
		return "http://www.sourcemm.net/";
	}
	const char *GetLicense()
	{
		return "Licence";
	}
	const char *GetVersion()
	{
		return "1.0";
	}
	const char *GetDate()
	{
		return __DATE__;
	}
	const char *GetLogTag()
	{
		return "MONOPLUG";
	}
public:
	//These functions are from IServerPluginCallbacks
	//Note, the parameters might be a little different to match the actual calls!

	//Called on LevelInit.  Server plugins only have pMapName
	//bool LevelInit(const char *pMapName, char const *pMapEntities, char const *pOldLevel, char const *pLandmarkName, bool loadGame, bool background);

	//Called on ServerActivate.  Same definition as server plugins
	void ServerActivate(edict_t *pEdictList, int edictCount, int clientMax);

	//Called on a game tick.  Same definition as server plugins
	void GameFrame(bool simulating);

	//Called on level shutdown.  Same definition as server plugins 
	//void LevelShutdown(void);

	//Sets the client index - same as server plugins
	//void SetCommandClient(int index);

	//Called on client settings changed (duh) - same as server plugins
	//void ClientSettingsChanged(edict_t *pEdict);

	//Called on client connect.  Unlike server plugins, we return whether the 
	// connection is allowed rather than set it through a pointer in the first parameter.
	// You can still supercede the GameDLL through RETURN_META_VALUE(MRES_SUPERCEDE, true/false)
	bool ClientConnect(edict_t *pEntity, const char *pszName, const char *pszAddress, char *reject, int maxrejectlen);

	//Client is activate (whatever that means).  We get an extra parameter...
	// "If bLoadGame is true, don't spawn the player because its state is already setup."
	void ClientActive(edict_t *pEntity, bool bLoadGame);

	//Client is put in server - same as server plugins
	void ClientPutInServer(edict_t *pEntity, char const *playername);

	//Client disconnects - same as server plugins
	void ClientDisconnect(edict_t *pEntity);

	//Called when a client uses a command.  Unlike server plugins, it's void.
	// You can still supercede the gamedll through RETURN_META(MRES_SUPERCEDE).
	//void ClientCommand(edict_t *pEntity);

	//From IMetamodListener
	//virtual void OnLevelShutdown();

	// game event listener
	void FireGameEvent( IGameEvent *event );

public:
	IGameEventManager2 *m_GameEventManager;	
	IVEngineServer *m_Engine;
	IServerGameDLL *m_ServerDll;
	IServerGameClients *m_ServerClients;
	ICvar *m_icvar;
	IPlayerInfoManager *m_playerinfomanager;

	SourceHook::CallClass<IVEngineServer> *m_Engine_CC;

	IFileSystem* m_filesystem;
public:
	MonoAssembly* m_assembly;
	MonoImage* m_image;

	MonoClass* m_Class_ClsMain;
	MonoMethod* m_ClsMain_Init;
	MonoMethod* m_ClsMain_Shutdown;
	MonoMethod* m_ClsMain_EVT_GameFrame;
	MonoMethod* m_ClsMain_ConvarChanged;
	MonoMethod* m_ClsMain_ConPrint;
	MonoMethod* m_ClsMain_ClientPutInServer;
	MonoMethod* m_ClsMain_ClientDisconnect;
	MonoObject* m_main;

	MonoClass* m_Class_ClsPlayer;
	MonoClassField* m_Field_ClsPlayer_id;
	MonoClassField* m_Field_ClsPlayer_name;
	MonoClassField* m_Field_ClsPlayer_frag;
	MonoClassField* m_Field_ClsPlayer_death;
	MonoClassField* m_Field_ClsPlayer_ip;
	MonoClassField* m_Field_ClsPlayer_language;
	MonoClassField* m_Field_ClsPlayer_avgLatency;
	MonoClassField* m_Field_ClsPlayer_timeConnected;

	MonoArray* m_players;

	CUtlVector<CMonoCommand*>* m_commands;

	uint64 m_convarNextId;
	CUtlMap<ConVar*, uint64>* m_convars;
public:
	int m_EdictCount;
	int m_MaxPlayers;
	CMonoConsole* m_console;
};

//extern CMonoPlug g_MonoPlug;
//extern MonoDomain* g_Domain;
//
//PLUGIN_GLOBALVARS();
//
edict_t *EdictOfUserId( int UserId );
int UTIL_FindOffset(const char *ClassName, const char *PropertyName);

#define	FIND_IFACE(func, assn_var, num_var, name, type) \
	do { \
		if ( (assn_var=(type)((ismm->func())(name, NULL))) != NULL ) { \
			num = 0; \
			break; \
		} \
		if (num >= 999) \
			break; \
	} while ( num_var=ismm->FormatIface(name, sizeof(name)-1) ); \
	if (!assn_var) { \
		if (error) \
			snprintf(error, maxlen, "Could not find interface %s", name); \
		return false; \
	}

#endif //_CMONOPLUG_H_
