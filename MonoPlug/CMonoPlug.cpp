#include "CMonoPlug.h"
//#include "cvars.h"

//#include "sh_string.h"
//#define GAME_DLL 1
//#include "cbase.h"


CMonoPlug g_MonoPlug;
CMonoPlugListener g_Listener;
MonoDomain* g_Domain = NULL;

PLUGIN_EXPOSE(CMonoPlug, g_MonoPlug);

//This has all of the necessary hook declarations.  Read it!
SH_DECL_HOOK1_void(IServerGameDLL, GameFrame, SH_NOATTRIB, 0, bool);
SH_DECL_HOOK3_void(IServerGameDLL, ServerActivate, SH_NOATTRIB, 0, edict_t *, int, int);

SH_DECL_HOOK5(IServerGameClients, ClientConnect, SH_NOATTRIB, 0, bool, edict_t *, const char*, const char *, char *, int);
SH_DECL_HOOK2_void(IServerGameClients, ClientActive, SH_NOATTRIB, 0, edict_t *, bool);
SH_DECL_HOOK1_void(IServerGameClients, ClientDisconnect, SH_NOATTRIB, 0, edict_t *);
SH_DECL_HOOK2_void(IServerGameClients, ClientPutInServer, SH_NOATTRIB, 0, edict_t *, char const *);

bool CMonoPlug::Unload(char* error, size_t maxlen)
{
	Q_snprintf(error, maxlen, "MonoPlug unloading denied because of Mono crash");
	return false;
}

bool CMonoPlug::Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late)
{
	PLUGIN_SAVEVARS();

	//Get inferfaces to engine
	char iface_buffer[255];
	int num=0;
	strcpy(iface_buffer, INTERFACEVERSION_SERVERGAMEDLL);
	FIND_IFACE(GetServerFactory, this->m_ServerDll, num, iface_buffer, IServerGameDLL *)
	strcpy(iface_buffer, INTERFACEVERSION_VENGINESERVER);
	FIND_IFACE(GetEngineFactory, this->m_Engine, num, iface_buffer, IVEngineServer *)
	strcpy(iface_buffer, INTERFACEVERSION_SERVERGAMECLIENTS);
	FIND_IFACE(GetServerFactory, this->m_ServerClients, num, iface_buffer, IServerGameClients *)
	strcpy(iface_buffer, INTERFACEVERSION_GAMEEVENTSMANAGER2);
	FIND_IFACE(GetEngineFactory, this->m_GameEventManager, num, iface_buffer, IGameEventManager2 *);
	strcpy(iface_buffer, FILESYSTEM_INTERFACE_VERSION);
	FIND_IFACE(GetEngineFactory, this->m_filesystem, num, iface_buffer, IFileSystem*);
	strcpy(iface_buffer, CVAR_INTERFACE_VERSION);
	FIND_IFACE(GetEngineFactory, this->m_icvar, num, iface_buffer, ICvar *);
	strcpy(iface_buffer, INTERFACEVERSION_PLAYERINFOMANAGER);
	FIND_IFACE(GetServerFactory, this->m_playerinfomanager, num, iface_buffer, IPlayerInfoManager *);

	META_LOG(g_PLAPI, "Starting plugin (Mono)\n");
	//Get game dir
	char* dir = new char[MAX_PATH];
	this->m_Engine->GetGameDir(dir, MAX_PATH);
	char dllPath[MAX_PATH];
	Q_snprintf( dllPath, sizeof(dllPath), MONOPLUG_DLLFILE, dir); 
	delete dir;
	META_LOG(g_PLAPI, "  Managed dll file is : %s", dllPath);
	if(!this->m_filesystem->FileExists(dllPath))
	{
		Q_snprintf(error, maxlen, "Can't find managed DLL : %s", dllPath);
		META_LOG(g_PLAPI, "%s\n", error);
		return false;
	}

	//One time init
	if(!g_Domain)
	{
#ifndef _WIN32
		//crash on windows
		mono_config_parse(NULL);
#endif

		g_Domain = mono_jit_init(dllPath);
		if(!g_Domain)
		{
			Q_snprintf(error, maxlen, "Can't init Mono JIT");
			return false;
		}
	}

	this->m_assembly = mono_domain_assembly_open(g_Domain, dllPath);
	if(!this->m_assembly)
	{
		Q_snprintf(error, maxlen, "Can't open assembly : %s", dllPath);
		return false;
	}
	this->m_image = mono_assembly_get_image(this->m_assembly);
	if(!this->m_image)
	{
		Q_snprintf(error, maxlen, "Can't get assembly image");
		return false;
	}
	MONO_GET_CLASS(this->m_image, this->m_Class_ClsMain, "MonoPlug", "ClsMain", error, maxlen)

	MONO_GET_CLASS(this->m_image, this->m_Class_ClsPlayer, "MonoPlug", "ClsPlayer", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_id, this->m_Class_ClsPlayer, "_id", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_name, this->m_Class_ClsPlayer, "_name", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_frag, this->m_Class_ClsPlayer, "_frag", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_death, this->m_Class_ClsPlayer, "_death", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_ip, this->m_Class_ClsPlayer, "_ip", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_language, this->m_Class_ClsPlayer, "_language", error, maxlen)

	MONO_ATTACH("MonoPlug.ClsMain:Init()", this->m_ClsMain_Init, this->m_image);
	MONO_ATTACH("MonoPlug.ClsMain:Shutdown()", this->m_ClsMain_Shutdown, this->m_image);

	MONO_ATTACH("MonoPlug.ClsMain:GameFrame()", this->m_ClsMain_EVT_GameFrame, this->m_image);
	MONO_ATTACH("MonoPlug.ClsMain:ConvarChanged(ulong)", this->m_ClsMain_ConvarChanged, this->m_image);

	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Msg", Mono_Msg);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Log", Mono_Log);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_RegisterConvar", Mono_RegisterConvar);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_UnregisterConvar", Mono_UnregisterConvar);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Convar_GetString", Mono_Convar_GetString);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Convar_SetString", Mono_Convar_SetString);

	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_RegisterConCommand", Mono_RegisterConCommand);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_UnregisterConCommand", Mono_UnregisterConCommand);

	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_GetPlayers", Mono_GetPlayers);

	//Console messages
	mono_add_internal_call ("MonoPlug.NativeMethods::Attach_ConMessage", Attach_ConMessage);
	mono_add_internal_call ("MonoPlug.NativeMethods::Detach_ConMessage", Detach_ConMessage);
	MONO_ATTACH("MonoPlug.ClsMain:Raise_ConMessage(bool,bool,int,int,int,int,string)", this->m_ClsMain_ConPrint, this->m_image);
	this->m_console = new CMonoConsole(this);

	//Client connection events
	MONO_ATTACH("MonoPlug.ClsMain:Raise_ClientPutInServer(MonoPlug.ClsPlayer)", this->m_ClsMain_ClientPutInServer, this->m_image);
	MONO_ATTACH("MonoPlug.ClsMain:Raise_ClientDisconnect(MonoPlug.ClsPlayer)", this->m_ClsMain_ClientDisconnect, this->m_image);

	//Create object instance
	this->m_main = mono_object_new(g_Domain, this->m_Class_ClsMain);
	mono_runtime_object_init(this->m_main);

	//All object creation is OK, init internal ararys
	this->m_convarNextId = 0;
	this->m_convars = new CUtlMap<ConVar*, uint64>();
	this->m_commands = new CUtlVector<CMonoCommand*>();

	META_LOG(g_PLAPI, "Starting plugin (Remaining)\n");
	ismm->AddListener(this, &g_Listener);

	//Init our cvars/concmds
#if SOURCE_ENGINE >= SE_ORANGEBOX
	//g_pCVar = icvar;
	ConVar_Register(0, &g_Accessor);
#else
	ConCommandBaseMgr::OneTimeInit(&s_BaseAccessor);
#endif

	//We're hooking the following things as POST, in order to seem like Server Plugins.
	//However, I don't actually know if Valve has done server plugins as POST or not.
	//Change the last parameter to 'false' in order to change this to PRE.
	//SH_ADD_HOOK_MEMFUNC means "SourceHook, Add Hook, Member Function".

	//Hook GameFrame to our function
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, m_ServerDll, &g_MonoPlug, &CMonoPlug::GameFrame, true);

	//Hook LevelInit to our function
	//SH_ADD_HOOK_MEMFUNC(IServerGameDLL, LevelInit, m_ServerDll, &g_MonoPlug, &CMonoPlug::LevelInit, true);
	//Hook ServerActivate to our function
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, m_ServerDll, &g_MonoPlug, &CMonoPlug::ServerActivate, true);
	//Hook LevelShutdown to our function -- this makes more sense as pre I guess
	//SH_ADD_HOOK_MEMFUNC(IServerGameDLL, LevelShutdown, m_ServerDll, &g_MonoPlug, &CMonoPlug::LevelShutdown, false);
	//Hook ClientActivate to our function
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientActive, m_ServerClients, &g_MonoPlug, &CMonoPlug::ClientActive, true);
	//Hook ClientDisconnect to our function
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, m_ServerClients, &g_MonoPlug, &CMonoPlug::ClientDisconnect, true);
	//Hook ClientPutInServer to our function
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, m_ServerClients, &g_MonoPlug, &CMonoPlug::ClientPutInServer, true);
	//Hook SetCommandClient to our function
	//SH_ADD_HOOK_MEMFUNC(IServerGameClients, SetCommandClient, m_ServerClients, &g_MonoPlug, &CMonoPlug::SetCommandClient, true);
	//Hook ClientSettingsChanged to our function
	//SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientSettingsChanged, m_ServerClients, &g_MonoPlug, &CMonoPlug::ClientSettingsChanged, true);

	//The following functions are pre handled, because that's how they are in IServerPluginCallbacks

	//Hook ClientConnect to our function
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientConnect, m_ServerClients, &g_MonoPlug, &CMonoPlug::ClientConnect, false);
	//Hook ClientCommand to our function
	//SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientCommand, m_ServerClients, &g_MonoPlug, &CMonoPlug::ClientCommand, false);

	//Get the call class for IVServerEngine so we can safely call functions without
	// invoking their hooks (when needed).
	m_Engine_CC = SH_GET_CALLCLASS(m_Engine);

	SH_CALL(m_Engine_CC, &IVEngineServer::LogPrint)("All hooks started!\n");

	g_SMAPI->AddListener(g_PLAPI, this);

	MonoObject* r = CMonoHelpers::MONO_CALL(this->m_main, this->m_ClsMain_Init, NULL);
	bool ret = *(bool*)mono_object_unbox(r);

	return ret;

	//return true;
}

void CMonoPlug::GameFrame(bool simulating)
{
	//don't log this, it just pumps stuff to the screen ;]
	//META_LOG(g_PLAPI, "GameFrame() called: simulating=%d", simulating);
	CMonoHelpers::MONO_CALL(this->m_main, this->m_ClsMain_EVT_GameFrame);
	RETURN_META(MRES_IGNORED);
}

void CMonoPlug::FireGameEvent( IGameEvent *event )
{
	if (!event)
		return;
	//const char *name = event->GetName();

	//if (FStrEq(name, "round_end"))
	//{
	//	META_LOG(g_PLAPI, "event '%s' received\n", name);
	//} // end "round_end"
}

void CMonoPlug::ServerActivate(edict_t *pEdictList, int edictCount, int clientMax)
{
	this->m_EdictCount = edictCount;
	this->m_MaxPlayers = clientMax;

	//Create player tracking array
	this->m_players = mono_array_new(g_Domain, this->m_Class_ClsPlayer, this->m_EdictCount);

	for(mono_array_size_t i = 0; i < this->m_EdictCount ; i++)
	{
		mono_array_set(this->m_players, MonoObject*, i, NULL);
	}
	RETURN_META(MRES_IGNORED);
}

bool CMonoPlug::ClientConnect(edict_t *pEntity, const char *pszName, const char *pszAddress, char *reject, int maxrejectlen)
{
	META_LOG(g_PLAPI, "ClientConnect(%s, %s)", pszName, pszAddress);

	int id = this->m_Engine->IndexOfEdict(pEntity);
	MonoObject* player = mono_object_new(g_Domain, this->m_Class_ClsPlayer);
	mono_runtime_object_init(player);
	mono_array_set(this->m_players, MonoObject*, id, player);

	mono_field_set_value(player, g_MonoPlug.m_Field_ClsPlayer_ip, MONO_STRING(g_Domain, pszAddress));

	RETURN_META_VALUE(MRES_IGNORED, true);
}

void CMonoPlug::ClientActive(edict_t *pEntity, bool bLoadGame)
{
	META_LOG(g_PLAPI, "Hook_ClientActive(%d, %d)", this->m_Engine->IndexOfEdict(pEntity), bLoadGame);
	RETURN_META(MRES_IGNORED);
}


void CMonoPlug::ClientPutInServer(edict_t *pEntity, const char *playername)
{
	if(!pEntity || pEntity->IsFree())
	{
		return;
	}

	int id = this->m_Engine->IndexOfEdict(pEntity);
	IPlayerInfo* pi = this->m_playerinfomanager->GetPlayerInfo(pEntity);

	MonoObject* player = mono_array_get(this->m_players, MonoObject*, id);
	if(!player)
	{
		//Ugly bot hack
		player = mono_object_new(g_Domain, this->m_Class_ClsPlayer);
		mono_runtime_object_init(player);
		mono_array_set(this->m_players, MonoObject*, id, player);
	}
		

	//Fill player data
	int pid = this->m_Engine->GetPlayerUserId(pEntity);

	INetChannelInfo* net = this->m_Engine->GetPlayerNetInfo(id);
	int pfrag = pi->GetFragCount();
	int pdeath = pi->GetDeathCount();

	mono_field_set_value(player, this->m_Field_ClsPlayer_id, &pid);
	mono_field_set_value(player, this->m_Field_ClsPlayer_name, MONO_STRING(g_Domain, pi->GetName()));
	mono_field_set_value(player, this->m_Field_ClsPlayer_frag, &pfrag);
	mono_field_set_value(player, this->m_Field_ClsPlayer_death, &pdeath);
	mono_field_set_value(player, this->m_Field_ClsPlayer_language, MONO_STRING(g_Domain, this->m_Engine->GetClientConVarValue(id, "cl_language")));

	void* args[1];
	args[0] = player;
	CMonoHelpers::MONO_CALL(this->m_main, this->m_ClsMain_ClientPutInServer, args);
}

void CMonoPlug::ClientDisconnect(edict_t *pEntity)
{
	if(!pEntity || pEntity->IsFree())
	{
		return;
	}

	int id = this->m_Engine->IndexOfEdict(pEntity);

	MonoObject* player= mono_array_get(this->m_players, MonoObject*, id);

	mono_array_set(this->m_players, MonoObject*, id, NULL);

	void* args[1];
	args[0] = player;
	CMonoHelpers::MONO_CALL(this->m_main, this->m_ClsMain_ClientDisconnect, args);
}