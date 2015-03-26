#include "Plugin.h"
#include "Managed.h"

Managed g_Managed;

Managed::Managed()
{
}

void Managed::Log(const char* msg)
{
	META_LOG(g_PLAPI, msg);
}

const char* Managed::ExecuteCommand(const char* cmd)
{
	META_LOG(g_PLAPI, cmd);
	return cmd;
}

bool Managed::Init(const char* sBaseDir)
{
	if (s_inited)
		return true;

	char sAssemblyFile[MAX_PATH];
	memset(sAssemblyFile, 0, MAX_PATH);

	V_ComposeFileName(sBaseDir, "dotnetplug/DotNetPlug.Managed.dll", sAssemblyFile, MAX_PATH);

	return this->InitPlateform(sAssemblyFile);
}