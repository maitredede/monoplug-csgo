#ifndef _DOTNETPLUG_MANAGED_H_
#define _DOTNETPLUG_MANAGED_H_
#ifdef _WIN32
#pragma once
#endif

#include "Types.h"
#include "Helpers.h"
#include "ManagedCommand.h"
#include "EventListener.h"
#include "GameEvent.h"

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

	void AddEventListeners(IGameEventManager2* gameevents);
	void RemoveEventListeners(IGameEventManager2* gameevents);


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
	//_TypePtr m_Type_DotNetPlug_TypeHelper;
public:
	//_MethodInfoPtr m_Method_DotNetPlug_TypeHelper_ExpandoNew;
	//_MethodInfoPtr m_Method_DotNetPlug_TypeHelper_ExpandoAdd;
private:
	//_AssemblyPtr m_Assembly_System_Core;
	//_TypePtr m_Type_System_Dynamic_ExpandoObject;
	//_MethodInfoPtr m_Method_ExpandoObject_Add;
	_MethodInfoPtr m_Method_DotNetPlug_IPluginManager_RaiseGameEvent;
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
	EventListener** EVT_Listeners;
public:
	void RaiseGameEvent(GameEvent e, IGameEvent *event);
};

#define NATIVE_EVENT_NAME_LENGTH 255
#define NATIVE_EVENT_VALUE_LENGTH 255
#define NATIVE_EVENT_ARGS_MAX 16

#ifdef MANAGED_WIN32

//[uuid(21602F40-CC62-11d4-AA2B-00A0CC39CFE0)]
//class INativeEventArgs :public IUnknown
//{
//
//};
//
typedef struct {
	BSTR name;
	int type;
	int intVal;
	BSTR strVal;
	bool boolVal;
	UINT64 longval;
	float floatVal;
} NativeEventArgs;

//[uuid(21602F40 - CC62 - 11d4 - AA2B - 00A0CC39CFE1)]
typedef struct
{
	GameEvent Event;
	int argsCount;
} NativeEventData;

inline void ADD_SHORT(NativeEventData* nativeEvent, NativeEventArgs* args, IGameEvent *event, const char* paramName)
{
	int i = nativeEvent->argsCount;
	args[i].type = 0;
	args[i].name = bstr_t(paramName).copy();
	args[i].intVal = event->GetInt(paramName);
	nativeEvent->argsCount++;
}

inline void ADD_BYTE(NativeEventData* nativeEvent, NativeEventArgs* args, IGameEvent *event, const char* paramName)
{
	ADD_SHORT(nativeEvent, args, event, paramName);
	/*int i = nativeEvent->argsCount;
	args[i].type = 0;
	args[i].name = bstr_t(paramName).copy();
	args[i].intVal = event->GetInt(paramName);
	nativeEvent->argsCount++;*/
}

inline void ADD_STRING(NativeEventData* nativeEvent, NativeEventArgs* args, IGameEvent *event, const char* paramName)
{
	int i = nativeEvent->argsCount;
	args[i].type = 1;
	args[i].name = bstr_t(paramName).copy();
	args[i].strVal = bstr_t(event->GetString(paramName));
	nativeEvent->argsCount++;
}

inline void ADD_BOOL(NativeEventData* nativeEvent, NativeEventArgs* args, IGameEvent *event, const char* paramName)
{
	int i = nativeEvent->argsCount;
	args[i].type = 2;
	args[i].name = bstr_t(paramName).copy();
	args[i].boolVal = event->GetBool(paramName);
	nativeEvent->argsCount++;
}

inline void ADD_LONG(NativeEventData* nativeEvent, NativeEventArgs* args, IGameEvent *event, const char* paramName)
{
	int i = nativeEvent->argsCount;
	args[i].type = 3;
	args[i].name = bstr_t(paramName).copy();
	args[i].longval = event->GetUint64(paramName);
	nativeEvent->argsCount++;
}

inline void ADD_FLOAT(NativeEventData* nativeEvent, NativeEventArgs* args, IGameEvent *event, const char* paramName)
{
	int i = nativeEvent->argsCount;
	args[i].type = 4;
	args[i].name = bstr_t(paramName).copy();
	args[i].floatVal = event->GetFloat(paramName);
	nativeEvent->argsCount++;
}

#endif

extern Managed g_Managed;

#endif // _DOTNETPLUG_MANAGED_H_