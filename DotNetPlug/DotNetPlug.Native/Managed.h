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
	bool s_inited;
	bool InitPlateform(const char* sAssemblyFile);

#ifdef MANAGED_WIN32
private:
	ICLRMetaHost *pMetaHost;
	ICLRRuntimeInfo *pRuntimeInfo;
	// ICorRuntimeHost and ICLRRuntimeHost are the two CLR hosting interfaces
	// supported by CLR 4.0. Here we demo the ICorRuntimeHost interface that 
	// was provided in .NET v1.x, and is compatible with all .NET Frameworks. 
	ICorRuntimeHost *pCorRuntimeHost;

	variant_t vtPluginManager;
	_MethodInfo* spPluginManagerTick;
	_MethodInfo* spPluginManagerAllPluginsLoaded;
	_MethodInfo* spPluginManagerUnload;
	_MethodInfo* spPluginManagerSetCallback_Log;
	_MethodInfo* spPluginManagerSetCallback_ExecuteCommand;

	LPWSTR pszVersion;
	LPWSTR pszAssemblyName;
	LPWSTR pszClassName;
	LPWSTR pszIfaceName;
#endif
#ifdef MANAGED_MONO
private: //Private members
	MonoDomain *pDomain;
	MonoAssembly *pAssembly;
	MonoImage *pAssemblyImage;
	MonoClass *pPluginManagerClass;
	MonoClass *pIPluginManagerClass;
	MonoClass *pPluginManagerMonoClass;
	MonoProperty* pPluginManagerInstanceProperty;
	MonoMethod* pPluginManagerInstancePropertyGetMethod;
	MonoObject* pPluginManagerInstanceObject;
	//MonoObject* pIPluginManagerInstanceObject;

	MonoMethod* pMapCallbacksToMono;
	MonoMethod* pPluginManagerAllPluginsLoadedMethod;
	MonoMethod* pPluginManagerAllPluginsLoadedMethodImplementation;
private: //Private methods
	static void LogMono(MonoString* pMsg);
	static MonoString* ExecuteCommandMono(MonoString* pMsg);
#endif
};


extern Managed g_Managed;

#endif // _DOTNETPLUG_MANAGED_H_