#include "Plugin.h"
#include "Managed.h"
#include "LoggingListener.h"

Managed g_Managed;

Managed::Managed()
{
	this->s_inited = false;
	this->m_commandsIndex = new std::map<const char*, int, char_cmp>();
	this->m_commandsClass = new std::map<int, ManagedCommand*>();
	//this->m_nextCmdId = 0;
}

Managed::~Managed(){
	delete this->m_commandsClass;
	delete this->m_commandsIndex;
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

	*output = CoTaskMemAlloc(*length);
	memcpy_s(*output, *length, outDump, *length);
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
	std::map<const char*, int, char_cmp>::iterator it_id = g_Managed.m_commandsIndex->find(cmd);
	if (it_id != g_Managed.m_commandsIndex->end())
		return -1;

	ManagedCommand* mCmd = new ManagedCommand(id, cmd, description, flags);

	g_Managed.m_commandsIndex->insert(std::pair<const char*, int>(cmd, id));
	g_Managed.m_commandsClass->insert(std::pair<int, ManagedCommand*>(id, mCmd));

	g_SMAPI->RegisterConCommandBase(g_PLAPI, mCmd->GetNativeCommand());

	return true;
}

void Managed::RaiseCommand(int argc, const char** argv){
	if (argc <= 0 || !argv)
		return;

	//Command name
	const char* cmdChar = argv[0];

	//Get command id from text
	std::map<const char*, int, char_cmp>::iterator it_id = this->m_commandsIndex->find(cmdChar);
	if (it_id == this->m_commandsIndex->end())
		return;
	int id = it_id->second;

	//Get command object from id
	std::map<int, ManagedCommand*>::iterator it_cmd = this->m_commandsClass->find(id);
	if (it_cmd == this->m_commandsClass->end())
		return;
	ManagedCommand* result = it_cmd->second;

	this->RaiseCommandPlateform(result, argc, argv);
}