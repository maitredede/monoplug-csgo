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

const char* Managed::ExecuteCommand(const char* cmd)
{
	/*META_LOG(g_PLAPI, cmd);
	return cmd;*/
	engine->ServerExecute();
	VirtualConsole* vConsole = new VirtualConsole()
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