#ifndef _DOTNETPLUG_MANAGED_H_
#define _DOTNETPLUG_MANAGED_H_

#ifdef MANAGED_WIN32

#include <windows.h>
#include <metahost.h>

#pragma comment(lib, "mscoree.lib")

#import <mscorlib.tlb> raw_interfaces_only					\
	high_property_prefixes("_get","_put","_putref")			\
	rename("ReportEvent", "InteropServices_ReportEvent")	\
	rename("Assert", "_Assert")

// #import "DotNetPlug.Managed.tlb" raw_interfaces_only

using namespace mscorlib;
#endif

#ifdef MANAGED_MONO
//#include <glib/glib.h>
#include <mono/jit/jit.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/assembly.h>
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
	bool s_inited = false;
	bool InitPlateform(const char* sAssemblyFile);

#ifdef MANAGED_WIN32
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
#endif
#ifdef MANAGED_MONO
private: //Private members
	MonoDomain *pDomain = NULL;
	MonoAssembly *pAssembly = NULL;
	MonoImage *pAssemblyImage = NULL;
	MonoClass *pPluginManagerClass = NULL;
	MonoClass *pIPluginManagerClass = NULL;
	MonoClass *pPluginManagerMonoClass = NULL;
	MonoProperty* pPluginManagerInstanceProperty = NULL;
	MonoMethod* pPluginManagerInstancePropertyGetMethod = NULL;
	MonoObject* pPluginManagerInstanceObject = NULL;
	MonoObject* pIPluginManagerInstanceObject = NULL;

	MonoMethod* pMapCallbacksToMono = NULL;
	MonoMethod* pPluginManagerAllPluginsLoadedMethod = NULL;
	MonoMethod* pPluginManagerAllPluginsLoadedMethodImplementation = NULL;
private: //Private methods
	static void LogMono(MonoString* pMsg);
	static MonoString* ExecuteCommandMono(MonoString* pMsg);
#endif
};

extern Managed g_Managed;

#endif