#ifndef _DOTNETPLUG_MANAGED_H_
#define _DOTNETPLUG_MANAGED_H_

#if defined WIN32

#include <windows.h>
#include <metahost.h>

#pragma comment(lib, "mscoree.lib")

#import <mscorlib.tlb> raw_interfaces_only					\
	high_property_prefixes("_get","_put","_putref")			\
	rename("ReportEvent", "InteropServices_ReportEvent")	\
	rename("Assert", "_Assert")

// #import "DotNetPlug.Managed.tlb" raw_interfaces_only

using namespace mscorlib;
#else

#endif

class Managed {

public:
	Managed();
	bool Init(const char* sBaseDir);
	void Cleanup();
	void Tick();
	void AllPluginsLoaded();
	void Unload();

	static void Log(const char* msg);
	static const char* ExecuteCommand(const char* cmd);
private:
	bool s_inited;

#ifdef WIN32
private:
	ICLRMetaHost *pMetaHost = NULL;
	ICLRRuntimeInfo *pRuntimeInfo = NULL;
	// ICorRuntimeHost and ICLRRuntimeHost are the two CLR hosting interfaces
	// supported by CLR 4.0. Here we demo the ICorRuntimeHost interface that 
	// was provided in .NET v1.x, and is compatible with all .NET Frameworks. 
	ICorRuntimeHost *pCorRuntimeHost = NULL;

	variant_t vtPluginManager = NULL;
	_MethodInfo* spPluginManagerTick = NULL;
	_MethodInfo* spPluginManagerAllPluginsLoaded = NULL;
	_MethodInfo* spPluginManagerUnload = NULL;
	_MethodInfo* spPluginManagerSetCallback_Log = NULL;
	_MethodInfo* spPluginManagerSetCallback_ExecuteCommand = NULL;

	LPWSTR pszVersion = NULL;
	LPWSTR pszAssemblyName = NULL;
	LPWSTR pszClassName = NULL;
	LPWSTR pszIfaceName = NULL;
#else
#endif
};

extern Managed g_Managed;

#endif