#include "Plugin.h"
#include "Managed.h"
#include "LoggingListener.h"
#include "UserTracker.h"

Managed::Managed()
{
	this->s_inited = false;


	/*this->EVT_player_death = new EventListener(player_death);
	this->EVT_round_start = new EventListener(round_start);*/
}

//void Managed::AddEventListeners(IGameEventManager2* gameevents)
//{
//	/*this->EVT_Listeners = new EventListener*[GameEvent::MAX];
//	for (int i = None; i < MAX; i++){
//		if (g_CSGO_EventNames[i] && strlen(g_CSGO_EventNames[i])){
//			this->EVT_Listeners[i] = new EventListener(static_cast<GameEvent>(i));
//			gameevents->AddListener(this->EVT_Listeners[i], g_CSGO_EventNames[i], true);
//		}
//		else{
//			this->EVT_Listeners[i] = NULL;
//		}
//	}*/
//}
//
//void Managed::RemoveEventListeners(IGameEventManager2* gameevents)
//{
//	/*for (int i = None; i < MAX; i++){
//		if (this->EVT_Listeners[i]){
//			gameevents->RemoveListener(this->EVT_Listeners[i]);
//			delete this->EVT_Listeners[i];
//		}
//	}
//	delete[] this->EVT_Listeners;*/
//}

Managed::~Managed()
{

}

void Managed::Log(const char* msg)
{
	META_LOG(g_PLAPI, msg);
}

void Managed::ExecuteCommand(const char* cmd, void** output, int* length)
{
	/*META_LOG(g_PLAPI, cmd);
	return cmd;*/
	int len = strlen(cmd) + 2;
	char* cmdWithNewLine = new char[len];
	ZeroMemory(cmdWithNewLine, len);
	strcpy_s(cmdWithNewLine, len, cmd);
	strcat_s(cmdWithNewLine, len, "\n");

	LoggingListener vLog;

	engine->ServerExecute();
	LoggingSystem_PushLoggingState(false, false);
	LoggingSystem_RegisterLoggingListener(&vLog);
	engine->ServerCommand(cmdWithNewLine);
	engine->ServerExecute();
	LoggingSystem_PopLoggingState(false);
	delete[] cmdWithNewLine;

	char* outDump = NULL;
	vLog.Dump(&outDump, length);

#if MANAGED_WIN32
	*output = CoTaskMemAlloc(*length);
	memcpy_s(*output, *length, outDump, *length);
#endif
#if MANAGED_MONO
	*output = (void*)outDump;
#endif
}

bool Managed::Init(const char* sBaseDir)
{
	if (s_inited)
		return true;

	char sAssemblyFile[MAX_PATH];
	ZeroMemory(sAssemblyFile, MAX_PATH);

	V_ComposeFileName(sBaseDir, "dotnetplug/DotNetPlug.Managed.dll", sAssemblyFile, MAX_PATH);

	return this->InitPlateform(sAssemblyFile);
}

bool Managed::RegisterCommand(const char* cmd, const char* description, int flags, int id)
{
	std::map<const char*, int, char_cmp>::iterator it_id = g_Managed.m_commandsIndex.find(cmd);
	if (it_id != g_Managed.m_commandsIndex.end())
		return -1;

	ManagedCommand* mCmd = new ManagedCommand(id, cmd, description, flags);

	g_Managed.m_commandsIndex.insert(std::pair<const char*, int>(cmd, id));
	g_Managed.m_commandsClass.insert(std::pair<int, ManagedCommand*>(id, mCmd));

	g_SMAPI->RegisterConCommandBase(g_PLAPI, mCmd->GetNativeCommand());

	return true;
}

void Managed::UnregisterCommand(int id)
{
	std::map<const char*, int, char_cmp>::iterator it_id = g_Managed.m_commandsIndex.begin();
	while (it_id != g_Managed.m_commandsIndex.end())
	{
		if (it_id->second == id)
		{
			g_Managed.m_commandsIndex.erase(it_id);
			break;
		}
	}

	std::map<int, ManagedCommand*>::iterator it_cmd = g_Managed.m_commandsClass.find(id);
	if (it_cmd != g_Managed.m_commandsClass.end())
	{
		ManagedCommand* mCmd = it_cmd->second;
		g_SMAPI->UnregisterConCommandBase(g_PLAPI, mCmd->GetNativeCommand());
		delete mCmd;

		g_Managed.m_commandsClass.erase(it_cmd);
	}

	//ManagedCommand* mCmd = new ManagedCommand(id, cmd, description, flags);
	//g_Managed.m_commandsClass->insert(std::pair<int, ManagedCommand*>(id, mCmd));

	//g_SMAPI->RegisterConCommandBase(g_PLAPI, mCmd->GetNativeCommand());

	//return true;
}

void Managed::GetPlayers(NativePlayer** pOutData, int& nbr)
{
	nbr = gUserTracker.Count();
	if (nbr == 0)
	{
		*pOutData = NULL;
		return;
	}
	*pOutData = (NativePlayer*)CoTaskMemAlloc(sizeof(NativePlayer)*nbr);
	ZeroMemory(*pOutData, sizeof(NativePlayer)*nbr);

	player_t* pData = new player_t[nbr];
	ZeroMemory(pData, sizeof(player_t) * nbr);

	gUserTracker.GetAll(pData, nbr);

	for (int i = 0; i < nbr; i++)
	{
		(*pOutData)[i].id = pData[i].user_id;
		(*pOutData)[i].name = bstr_t(pData[i].name).copy();
		(*pOutData)[i].team = pData[i].team;
		(*pOutData)[i].health = pData[i].health;
		(*pOutData)[i].is_bot = pData[i].is_bot;
		(*pOutData)[i].is_dead = pData[i].is_dead;
		(*pOutData)[i].ip_address = bstr_t(pData[i].ip_address).copy();
		(*pOutData)[i].steam_id = bstr_t(pData[i].steam_id).copy();
	}

	delete[] pData;
}

void Managed::RaiseCommand(int argc, const char** argv){
	if (argc <= 0 || !argv)
		return;

	//Command name
	const char* cmdChar = argv[0];

	//Get command id from text
	std::map<const char*, int, char_cmp>::iterator it_id = this->m_commandsIndex.find(cmdChar);
	if (it_id == this->m_commandsIndex.end())
		return;
	int id = it_id->second;

	//Get command object from id
	std::map<int, ManagedCommand*>::iterator it_cmd = this->m_commandsClass.find(id);
	if (it_cmd == this->m_commandsClass.end())
		return;
	ManagedCommand* result = it_cmd->second;

	this->RaiseCommandPlateform(result, argc, argv);
}

//void Managed::GetServerInfo(){
//	engine->GetAppID();
//	engine->GetGameDir();
//	engine->GetGameServerSteamID();
//	engine->GetLaunchOptions();
//	engine->GetServerVersion();
//	engine->GetTimescale();
//	engine->IsDedicatedServer();
//
//	server->GetGameDescription();
//	server->GetTickInterval();
//
//	gameclients->GetPlayerLimits();
//
//	//gpGlobals
//}
//
//void Managed::GetPlayers(){
//	gameclients->
//}