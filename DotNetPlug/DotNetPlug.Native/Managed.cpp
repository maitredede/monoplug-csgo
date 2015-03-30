#include "Plugin.h"
#include "Managed.h"

Managed g_Managed;

Managed::Managed()
{
	this->s_inited = false;
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
	icvar->RegisterConsole(vConsole);
	engine->ServerCommand(cmd);
	engine->ServerExecute();
	icvar->UnregisterConsole(vConsole);
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