#include "CMonoPlug.h"

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
	FIND_IFACE(GetServerFactory, g_ServerDll, num, iface_buffer, IServerGameDLL *);

	strcpy(iface_buffer, INTERFACEVERSION_VENGINESERVER);
	FIND_IFACE(GetEngineFactory, g_Engine, num, iface_buffer, IVEngineServer *);

	strcpy(iface_buffer, INTERFACEVERSION_SERVERGAMECLIENTS);
	FIND_IFACE(GetServerFactory, g_ServerClients, num, iface_buffer, IServerGameClients *);

	strcpy(iface_buffer, INTERFACEVERSION_GAMEEVENTSMANAGER2);
	FIND_IFACE(GetEngineFactory, g_GameEventManager, num, iface_buffer, IGameEventManager2 *);

	strcpy(iface_buffer, FILESYSTEM_INTERFACE_VERSION);
	FIND_IFACE(GetEngineFactory, g_filesystem, num, iface_buffer, IFileSystem*);

	strcpy(iface_buffer, INTERFACEVERSION_PLAYERINFOMANAGER);
	FIND_IFACE(GetServerFactory, g_playerinfomanager, num, iface_buffer, IPlayerInfoManager *);

	strcpy(iface_buffer, INTERFACEVERSION_ISERVERPLUGINHELPERS);
	FIND_IFACE(GetEngineFactory, g_helpers, num, iface_buffer, IServerPluginHelpers *);

	strcpy(iface_buffer, CVAR_INTERFACE_VERSION);
	FIND_IFACE(GetEngineFactory, icvar, num, iface_buffer, ICvar *);
	//GET_V_IFACE_CURRENT(GetEngineFactory, icvar, ICvar, CVAR_INTERFACE_VERSION);

	g_Globals = ismm->GetCGlobals();

	/* Load the VSP listener.  This is usually needed for IServerPluginHelpers. */
	if ((g_vsp_callbacks = ismm->GetVSPInfo(NULL)) == NULL)
	{
		ismm->AddListener(this, &g_Listener);
		ismm->EnableVSPListener();
	}

	//Hook GameFrame to our function
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, GameFrame, g_ServerDll, &g_MonoPlug, &CMonoPlug::GameFrame, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameDLL, ServerActivate, g_ServerDll, &g_MonoPlug, &CMonoPlug::ServerActivate, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientActive, g_ServerClients, &g_MonoPlug, &CMonoPlug::ClientActive, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientDisconnect, g_ServerClients, &g_MonoPlug, &CMonoPlug::ClientDisconnect, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientPutInServer, g_ServerClients, &g_MonoPlug, &CMonoPlug::ClientPutInServer, true);
	SH_ADD_HOOK_MEMFUNC(IServerGameClients, ClientConnect, g_ServerClients, &g_MonoPlug, &CMonoPlug::ClientConnect, false);

	g_Engine->LogPrint("All hooks started!\n");

#if SOURCE_ENGINE >= SE_ORANGEBOX
	g_pCVar = icvar;
	ConVar_Register(0, &g_Accessor);
#else
	ConCommandBaseMgr::OneTimeInit(&s_BaseAccessor);
#endif

	//g_SMAPI->AddListener(g_PLAPI, this);

	META_LOG(g_PLAPI, "Starting plugin (Mono)\n");
	
	//Get game dir
	char* dir = new char[MAX_PATH];
	g_Engine->GetGameDir(dir, MAX_PATH);
	char dllPath[MAX_PATH];
	Q_snprintf( dllPath, sizeof(dllPath), MONOPLUG_DLLFILE, dir); 
	delete dir;
	META_LOG(g_PLAPI, "  Managed dll file is : %s", dllPath);
	if(!g_filesystem->FileExists(dllPath))
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
	MONO_ATTACH("MonoPlug.ClsMain:Init()", this->m_ClsMain_Init, this->m_image);
	MONO_ATTACH("MonoPlug.ClsMain:Shutdown()", this->m_ClsMain_Shutdown, this->m_image);
	MONO_ATTACH("MonoPlug.ClsMain:GameFrame()", this->m_ClsMain_EVT_GameFrame, this->m_image);
	//MONO_ATTACH("MonoPlug.ClsMain:ConvarChanged(ulong)", this->m_ClsMain_ConvarChanged, this->m_image);

	MONO_GET_CLASS(this->m_image, this->m_Class_ClsPlayer, "MonoPlug", "ClsPlayer", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_id, this->m_Class_ClsPlayer, "_id", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_name, this->m_Class_ClsPlayer, "_name", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_frag, this->m_Class_ClsPlayer, "_frag", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_death, this->m_Class_ClsPlayer, "_death", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_ip, this->m_Class_ClsPlayer, "_ip", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_language, this->m_Class_ClsPlayer, "_language", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_avgLatency, this->m_Class_ClsPlayer, "_avgLatency", error, maxlen)
	MONO_GET_FIELD(this->m_Field_ClsPlayer_timeConnected, this->m_Class_ClsPlayer, "_timeConnected", error, maxlen)

	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Msg", (const void*)Mono_Msg);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Log", (const void*)Mono_Log);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_DevMsg", (const void*)Mono_DevMsg);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Warning", (const void*)Mono_Warning);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Error", (const void*)Mono_Error);

	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_RegisterConvar", (const void*)Mono_RegisterConvar);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_UnregisterConvar", (const void*)Mono_UnregisterConvar);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Convar_GetString", (const void*)Mono_Convar_GetString);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Convar_SetString", (const void*)Mono_Convar_SetString);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Convar_GetBoolean", (const void*)Mono_Convar_GetBoolean);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Convar_SetBoolean", (const void*)Mono_Convar_SetBoolean);

	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_RegisterConCommand", (const void*)Mono_RegisterConCommand);
	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_UnregisterConCommand", (const void*)Mono_UnregisterConCommand);

	//Console messages
	mono_add_internal_call ("MonoPlug.NativeMethods::Attach_ConMessage", (const void*)Attach_ConMessage);
	mono_add_internal_call ("MonoPlug.NativeMethods::Detach_ConMessage", (const void*)Detach_ConMessage);
	MONO_ATTACH("MonoPlug.ClsMain:Raise_ConMessage(bool,bool,int,int,int,int,string)", this->m_ClsMain_ConPrint, this->m_image);
	this->m_console = new CMonoConsole(this);

	//Client connection events
	MONO_ATTACH("MonoPlug.ClsMain:Raise_ClientPutInServer(MonoPlug.ClsPlayer)", this->m_ClsMain_ClientPutInServer, this->m_image);
	MONO_ATTACH("MonoPlug.ClsMain:Raise_ClientDisconnect(int)", this->m_ClsMain_ClientDisconnect, this->m_image);

	mono_add_internal_call ("MonoPlug.NativeMethods::Mono_ClientDialogMessage", (const void*)Mono_ClientDialogMessage);

	//Create object instance
	this->m_main = mono_object_new(g_Domain, this->m_Class_ClsMain);
	mono_runtime_object_init(this->m_main);

	//All object creation is OK, init internal ararys
	this->m_convarNextId = 0;
	this->m_convars = new CUtlMap<uint64, CMonoConvar*>();
	this->m_convars->SetLessFunc(LessFunc_uint64);
	this->m_commands = new CUtlVector<CMonoCommand*>();


	META_LOG(g_PLAPI, "Starting plugin (Remaining)\n");

	MonoObject* r = CMonoHelpers::MONO_CALL(this->m_main, this->m_ClsMain_Init, NULL);
	if(NULL == r)
	{
		Q_snprintf(error, maxlen, "Unhandled managed exception in ClsMain::Init()");
		return false;
	}
	bool ret = *(bool*)mono_object_unbox(r);

	return ret;
}

void CMonoPlug::OnVSPListening(IServerPluginCallbacks *iface)
{
	g_vsp_callbacks = iface;
}

void CMonoPlug::GameFrame(bool simulating)
{
	//don't log this, it just pumps stuff to the screen ;]
	//META_LOG(g_PLAPI, "GameFrame() called: simulating=%d", simulating);
	CMonoHelpers::MONO_CALL(this->m_main, this->m_ClsMain_EVT_GameFrame);
	//RETURN_META(MRES_IGNORED);
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

	////Create player tracking array
	////this->m_players = mono_array_new(g_Domain, this->m_Class_ClsPlayer, this->m_EdictCount);

	//mono_array_size_t max = (mono_array_size_t)this->m_EdictCount;

	//for(mono_array_size_t i = 0; i < max ; i++)
	//{
	//	mono_array_set(this->m_players, MonoObject*, i, NULL);
	//}
	RETURN_META(MRES_IGNORED);
}

bool CMonoPlug::ClientConnect(edict_t *pEntity, const char *pszName, const char *pszAddress, char *reject, int maxrejectlen)
{
	META_LOG(g_PLAPI, "ClientConnect(%s, %s)", pszName, pszAddress);

	//int id = this->m_Engine->IndexOfEdict(pEntity);
	//MonoObject* player = mono_object_new(g_Domain, this->m_Class_ClsPlayer);
	//mono_runtime_object_init(player);
	//mono_array_set(this->m_players, MonoObject*, id, player);

	//mono_field_set_value(player, g_MonoPlug.m_Field_ClsPlayer_ip, MONO_STRING(g_Domain, pszAddress));

	RETURN_META_VALUE(MRES_IGNORED, true);
}

void CMonoPlug::ClientActive(edict_t *pEntity, bool bLoadGame)
{
	META_LOG(g_PLAPI, "Hook_ClientActive(%d, %d)", g_Engine->IndexOfEdict(pEntity), bLoadGame);
	RETURN_META(MRES_IGNORED);
}


void CMonoPlug::ClientPutInServer(edict_t *pEntity, const char *playername)
{
	if(!pEntity || pEntity->IsFree())
	{
		return;
	}

	//int index = this->m_Engine->IndexOfEdict(pEntity);
	IPlayerInfo* pi = g_playerinfomanager->GetPlayerInfo(pEntity);

	MonoObject* player = mono_object_new(g_Domain, this->m_Class_ClsPlayer);
	mono_runtime_object_init(player);

	//Fill player data
	int playerId = g_Engine->GetPlayerUserId(pEntity);

	INetChannelInfo* net = g_Engine->GetPlayerNetInfo(playerId);
	float avgLatency = -1.0;
	float timeConnected = -1.0;
	MonoString* address;
	if(net)
	{
		avgLatency = net->GetAvgLatency(MAX_FLOWS);
		timeConnected = net->GetTimeConnected();
		address = CMonoHelpers::MONO_STRING(g_Domain, net->GetAddress());
	}
	else
	{
		address = CMonoHelpers::MONO_STRING(g_Domain, NULL);
	}
	int pfrag = pi->GetFragCount();
	int pdeath = pi->GetDeathCount();

	mono_field_set_value(player, this->m_Field_ClsPlayer_id, &playerId);
	mono_field_set_value(player, this->m_Field_ClsPlayer_name, CMonoHelpers::MONO_STRING(g_Domain, pi->GetName()));
	mono_field_set_value(player, this->m_Field_ClsPlayer_frag, &pfrag);
	mono_field_set_value(player, this->m_Field_ClsPlayer_death, &pdeath);
	mono_field_set_value(player, this->m_Field_ClsPlayer_ip, address);
	mono_field_set_value(player, this->m_Field_ClsPlayer_language, CMonoHelpers::MONO_STRING(g_Domain, g_Engine->GetClientConVarValue(playerId, "cl_language")));
	mono_field_set_value(player, this->m_Field_ClsPlayer_avgLatency, &avgLatency);
	mono_field_set_value(player, this->m_Field_ClsPlayer_timeConnected, &timeConnected);

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

	//int index = this->m_Engine->IndexOfEdict(pEntity);
	int playerId = g_Engine->GetPlayerUserId(pEntity);

	//MonoObject* player= mono_array_get(this->m_players, MonoObject*, id);

	//mono_array_set(this->m_players, MonoObject*, id, NULL);

	void* args[1];
	args[0] = &playerId;
	CMonoHelpers::MONO_CALL(this->m_main, this->m_ClsMain_ClientDisconnect, args);
}

void CMonoPlug::AllPluginsLoaded()
{
#ifdef _DEBUG
	g_Engine->ServerCommand("clr_plugin_load ConClock");
#endif
}