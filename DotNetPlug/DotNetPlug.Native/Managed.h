#ifndef _DOTNETPLUG_MANAGED_H_
#define _DOTNETPLUG_MANAGED_H_

#include "Types.h"
#include "Helpers.h"
#include "ManagedCommand.h"

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
#include <mono/jit/jit.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/assembly.h>
#endif

class ManagedCommand;

class Managed {

public:
	Managed();
	~Managed();
	bool Init(const char* sBaseDir);
	void Cleanup();

	void Tick();
	void AllPluginsLoaded();
	void Unload();

	void LoadAssembly(int argc, const char** argv);

	void RaiseCommand(int argc, const char** argv);
	void RaiseCommandPlateform(ManagedCommand* cmd, int argc, const char** argv);

	static void Log(const char* msg);
	static void ExecuteCommand(const char* cmd, void** output, int* length);
	static bool RegisterCommand(const char* cmd, const char* description, int flags, int id);
	static void UnregisterCommand(int id);
	//static void GetServerInfo();
	//static void GetPlayers();
private:
	bool s_inited;
	bool InitPlateform(const char* sAssemblyFile);

	std::map<const char*, int, char_cmp>* m_commandsIndex;
	std::map<int, ManagedCommand*>* m_commandsClass;

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
	_MethodInfo* spPluginManagerLoadAssembly;
	_MethodInfo* spPluginManagerRaiseCommand;

	_MethodInfo* spPluginManagerInitWin32Engine;

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
	MonoClass *pStringClass;
	MonoClass *pPluginManagerClass;
	MonoClass *pIPluginManagerClass;

	MonoProperty* pPluginManagerInstanceProperty;
	MonoMethod* pPluginManagerInstancePropertyGetMethod;
	MonoObject* pPluginManagerInstanceObject;

	MonoMethod* pMapCallbacksToMono;

	MonoMethod* pPluginManagerAllPluginsLoadedMethod;
	MonoMethod* pPluginManagerAllPluginsLoadedMethodImplementation;

	MonoMethod* pPluginManagerTickMethod;
	MonoMethod* pPluginManagerTickMethodImplementation;

	MonoMethod* pPluginManagerUnloadMethod;
	MonoMethod* pPluginManagerUnloadMethodImplementation;

	MonoMethod* pPluginManagerLoadAssemblyMethod;
	MonoMethod* pPluginManagerLoadAssemblyMethodImplementation;

	MonoMethod* pPluginManagerRaiseCommandMethod;
	MonoMethod* pPluginManagerRaiseCommandMethodImplementation;
private: //Private methods
	static void LogMono(MonoString* pMsg);
	static void ExecuteCommandMono(MonoString* pMsg, MonoString* pOutput, int* pLength);
	static void RegisterCommandMono(MonoString* pMsg, MonoString* pDesc, int flags, int id);
#endif
};


extern Managed g_Managed;

#endif // _DOTNETPLUG_MANAGED_H_