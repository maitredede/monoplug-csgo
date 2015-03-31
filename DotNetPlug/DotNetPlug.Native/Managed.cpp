#include "Plugin.h"
#include "Managed.h"

Managed g_Managed;

Managed::Managed()
{
	this->s_inited = false;
	this->m_commandsIndex = new std::map<const char*, int, char_cmp>();
	this->m_commandsClass = new std::map<int, ManagedCommand*>();
	this->m_nextCmdId = 0;
}

Managed::~Managed(){
	delete this->m_commandsClass;
	delete this->m_commandsIndex;
}

void Managed::Log(const char* msg)
{
	META_LOG(g_PLAPI, msg);
}

void Managed::ExecuteCommand(const char* cmd, char** output, size_t* size)
{
	/*META_LOG(g_PLAPI, cmd);
	return cmd;*/
	engine->ServerExecute();
	VirtualConsole* vConsole = new VirtualConsole();
	IConsoleDisplayFunc* conDispFunc = (IConsoleDisplayFunc*)vConsole;
	icvar->InstallConsoleDisplayFunc(conDispFunc);
	engine->ServerCommand(cmd);
	engine->ServerExecute();
	icvar->RemoveConsoleDisplayFunc(conDispFunc);
	vConsole->Dump(output, size);
	delete vConsole;
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

int Managed::RegisterCommand(const char* cmd, const char* description, int flags, MANAGED_COMMAND_CALLBACK callback)
{
	std::map<const char*, int, char_cmp>::iterator it_id = g_Managed.m_commandsIndex->find(cmd);
	if (it_id != g_Managed.m_commandsIndex->end())
		return -1;

	int id = g_Managed.m_nextCmdId++;
	ManagedCommand* mCmd = new ManagedCommand(id, cmd, description, flags, callback);

	g_Managed.m_commandsIndex->insert(std::pair<const char*, int>(cmd, id));
	g_Managed.m_commandsClass->insert(std::pair<int, ManagedCommand*>(id, mCmd));

	g_SMAPI->RegisterConCommandBase(g_PLAPI, mCmd->GetNativeCommand());

	return id;
	////ConCommand* pCC = new ConCommand(cmd, (FnCommandCallback_t)callback, description, flags);
	//ConCommand* pCC = new ConCommand(cmd, (FnCommandCallback_t)&(g_Managed.CommandCallback), description, flags);
	//g_SMAPI->RegisterConCommandBase(g_PLAPI, pCC);
	////void(*FnCommandCallback_t)(const CCommand &command)
}

ManagedCommand* Managed::GetCommand(const char* cmdchar)
{
	//Get command id from text
	std::map<const char*, int, char_cmp>::iterator it_id = this->m_commandsIndex->find(cmdchar);
	if (it_id == this->m_commandsIndex->end())
		return NULL;
	int id = it_id->second;

	//Get command object from id
	std::map<int, ManagedCommand*>::iterator it_cmd = this->m_commandsClass->find(id);
	if (it_cmd == this->m_commandsClass->end())
		return NULL;
	ManagedCommand* result = it_cmd->second;
	return result;
}
