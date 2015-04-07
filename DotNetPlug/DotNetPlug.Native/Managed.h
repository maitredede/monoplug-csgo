#ifndef _DOTNETPLUG_MANAGED_H_
#define _DOTNETPLUG_MANAGED_H_
#ifdef _WIN32
#pragma once
#endif

#include "Types.h"
#include "Helpers.h"
#include "ManagedCommand.h"
#include "EventListener.h"

class ManagedCommand;

class Managed {

public:
	Managed();
	~Managed();
	bool Init(const char* sBaseDir);
	void Cleanup();

	void Load();
	void Tick();
	void Unload();

	void RaiseCommand(int argc, const char** argv);

	static void Log(const char* msg);
	static void ExecuteCommand(const char* cmd, void** output, int* length);
	static bool RegisterCommand(const char* cmd, const char* description, int flags, int id);
	static void UnregisterCommand(int id);

	void RaiseLevelInit(const char *pMapName, const char *pMapEntities, const char *pOldLevel, const char *pLandmarkName, bool loadGame, bool background);
	void RaiseServerActivate(int clientMax);
	void RaiseLevelShutdown();
	void RaiseClientActive();
	void RaiseClientDisconnect();
	void RaiseClientPutInServer();
	void RaiseClientSettingsChanged();
	void RaiseClientConnect();
	void RaiseClientCommand();
private:
	bool s_inited;
	bool InitPlateform(const char* sAssemblyFile);
	void RaiseCommandPlateform(ManagedCommand* cmd, int argc, const char** argv);

	std::map<const char*, int, char_cmp> m_commandsIndex;
	std::map<int, ManagedCommand*> m_commandsClass;

#ifdef MANAGED_WIN32
private:
	ICLRMetaHost *pMetaHost;
	ICLRRuntimeInfo *pRuntimeInfo;
	// ICorRuntimeHost and ICLRRuntimeHost are the two CLR hosting interfaces
	// supported by CLR 4.0. Here we demo the ICorRuntimeHost interface that 
	// was provided in .NET v1.x, and is compatible with all .NET Frameworks. 
	ICorRuntimeHost *pCorRuntimeHost;

	IUnknownPtr spAppDomainThunk = NULL;
	_AppDomainPtr spDefaultAppDomain = NULL;

	_AssemblyPtr m_Assembly_Mscorlib;
	_TypePtr m_Type_System_Reflection_Assembly;

	variant_t vtPluginManager;
	_MethodInfoPtr spPluginManagerTick;
	_MethodInfoPtr spPluginManagerLoad;
	_MethodInfoPtr spPluginManagerUnload;
	_MethodInfoPtr spPluginManagerLoadAssembly;
	_MethodInfoPtr spPluginManagerRaiseCommand;

	_MethodInfoPtr spPluginManagerInitWin32Engine;

	_MethodInfoPtr spPluginManagerLevelInit;
	_MethodInfoPtr spPluginManagerServerActivate;

	_MethodInfoPtr spPluginManagerLevelShutdown;
	_MethodInfoPtr spPluginManagerClientActive;
	_MethodInfoPtr spPluginManagerClientDisconnect;
	_MethodInfoPtr spPluginManagerClientPutInServer;
	_MethodInfoPtr spPluginManagerClientSettingsChanged;
	_MethodInfoPtr spPluginManagerClientConnect;
	_MethodInfoPtr spPluginManagerClientCommand;

	_AssemblyPtr m_Assembly_DotNetPlug_Managed;
	_TypePtr m_Type_DotNetPlug_PluginManager;
	_PropertyInfoPtr m_Property_DotNetPlug_PluginManager_Instance;
	_MethodInfoPtr m_Method_DotNetPlug_PluginManager_Instance_Get;
	_TypePtr m_Type_DotNetPlug_IPluginManager;
	_TypePtr m_Type_DotNetPlug_TypeHelper;
public:
	_MethodInfoPtr m_Method_DotNetPlug_TypeHelper_ExpandoNew;
	_MethodInfoPtr m_Method_DotNetPlug_TypeHelper_ExpandoAdd;
private:
	//_AssemblyPtr m_Assembly_System_Core;
	//_TypePtr m_Type_System_Dynamic_ExpandoObject;
	//_MethodInfoPtr m_Method_ExpandoObject_Add;
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

	MonoMethod* pPluginManagerLoadMethod;
	MonoMethod* pPluginManagerLoadMethodImplementation;

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

public: //Events
	EventListener* EVT_player_death;
public:
	void RaiseGameEvent(GameEvent e, IGameEvent *event);
};


extern Managed g_Managed;

#endif // _DOTNETPLUG_MANAGED_H_