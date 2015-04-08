#include "Managed.h"
#include "Plugin.h"

#ifdef MANAGED_WIN32

#include <comutil.h>
#include <stdio.h>
#include "Helpers.h"

#pragma comment(lib, "comsuppw.lib")
#pragma comment(lib, "kernel32.lib")

#define GET_TYPE(pAssembly, sType, pType) hr = GET_TYPE_FUNC(pAssembly, sType, pType); \
	if (FAILED(hr)) { this->Cleanup(); return false; }

#define LOAD_ASSEMBLY(pAppDomain, sAssemblyName, pAssembly) hr = LOAD_ASSEMBLY_FUNC(pAppDomain, sAssemblyName, pAssembly); \
	if (FAILED(hr)) { this->Cleanup(); return false; }

void Managed::Cleanup(){
	if (pMetaHost)
	{
		pMetaHost->Release();
		pMetaHost = NULL;
	}
	if (pRuntimeInfo)
	{
		pRuntimeInfo->Release();
		pRuntimeInfo = NULL;
	}
	if (pCorRuntimeHost)
	{
		// Please note that after a call to Stop, the CLR cannot be 
		// reinitialized into the same process. This step is usually not 
		// necessary. You can leave the .NET runtime loaded in your process.
		//META_CONPRINTF("Stop the .NET runtime\n");
		//pCorRuntimeHost->Stop();

		pCorRuntimeHost->Release();
		pCorRuntimeHost = NULL;
	}
}

bool Managed::InitPlateform(const char* sAssemblyFile)
{
	//this->pszVersion = L"v4.0.30319";
	//this->pszAssemblyName = L"DotNetPlug.Managed";
	//this->pszClassName = L"DotNetPlug.PluginManager";
	//this->pszIfaceName = L"DotNetPlug.IPluginManager";

	HRESULT hr;

	//// The .NET assembly to load.
	//bstr_t bstrAssemblyName(sAssemblyFile);
	//_AssemblyPtr spAssembly = NULL;

	// Load and start the .NET runtime.
	hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&pMetaHost));
	if (FAILED(hr))
	{
		META_CONPRINTF("CLRCreateInstance failed w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	// Get the ICLRRuntimeInfo corresponding to a particular CLR version. It 
	// supersedes CorBindToRuntimeEx with STARTUP_LOADER_SAFEMODE.
	const LPWSTR pszVersion = L"v4.0.30319";
	hr = pMetaHost->GetRuntime(pszVersion, IID_PPV_ARGS(&pRuntimeInfo));
	if (FAILED(hr))
	{
		META_CONPRINTF("ICLRMetaHost::GetRuntime failed w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	// Check if the specified runtime can be loaded into the process. This 
	// method will take into account other runtimes that may already be 
	// loaded into the process and set pbLoadable to TRUE if this runtime can 
	// be loaded in an in-process side-by-side fashion. 
	BOOL fLoadable;
	hr = pRuntimeInfo->IsLoadable(&fLoadable);
	if (FAILED(hr))
	{
		META_CONPRINTF("ICLRRuntimeInfo::IsLoadable failed w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	if (!fLoadable)
	{
		META_CONPRINTF(".NET runtime %s cannot be loaded\n", pszVersion);
		this->Cleanup();
		return false;
	}

	// Load the CLR into the current process and return a runtime interface 
	// pointer. ICorRuntimeHost and ICLRRuntimeHost are the two CLR hosting  
	// interfaces supported by CLR 4.0. Here we demo the ICorRuntimeHost 
	// interface that was provided in .NET v1.x, and is compatible with all 
	// .NET Frameworks. 
	hr = pRuntimeInfo->GetInterface(CLSID_CorRuntimeHost, IID_PPV_ARGS(&pCorRuntimeHost));
	if (FAILED(hr))
	{
		META_CONPRINTF("ICLRRuntimeInfo::GetInterface failed w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	// Start the CLR.
	hr = pCorRuntimeHost->Start();
	if (FAILED(hr))
	{
		META_CONPRINTF("CLR failed to start w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	// 
	// Load the NET assembly. Call the static method GetStringLength of the 
	// class CSSimpleObject. Instantiate the class CSSimpleObject and call 
	// its instance method ToString.
	// 

	// The following C++ code does the same thing as this C# code:
	// 
	//   Assembly assembly = AppDomain.CurrentDomain.Load(pszAssemblyName);
	//   object length = type.InvokeMember("GetStringLength", 
	//       BindingFlags.InvokeMethod | BindingFlags.Static | 
	//       BindingFlags.Public, null, null, new object[] { "HelloWorld" });
	//   object obj = assembly.CreateInstance("CSClassLibrary.CSSimpleObject");
	//   object str = type.InvokeMember("ToString", 
	//       BindingFlags.InvokeMethod | BindingFlags.Instance | 
	//       BindingFlags.Public, null, obj, new object[] { });

	// Get a pointer to the default AppDomain in the CLR.
	hr = pCorRuntimeHost->GetDefaultDomain(&spAppDomainThunk);
	if (FAILED(hr))
	{
		META_CONPRINTF("ICorRuntimeHost::GetDefaultDomain failed w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	hr = spAppDomainThunk->QueryInterface(IID_PPV_ARGS(&spDefaultAppDomain));
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to get default AppDomain w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	//Load mscorlib
	LOAD_ASSEMBLY(this->spDefaultAppDomain, "mscorlib", &this->m_Assembly_Mscorlib);
	// Get the Type of System.Reflection.Assembly.
	GET_TYPE(this->m_Assembly_Mscorlib, "System.Reflection.Assembly", &this->m_Type_System_Reflection_Assembly);

	// Create a safe array to contain the arguments of the method. The safe 
	// array must be created with vt = VT_VARIANT because .NET reflection 
	// expects an array of Object - VT_VARIANT. There is only one argument, 
	// so cElements = 1.
	SAFEARRAY *psaAssemblyLoadMethodArgs = SafeArrayCreateVector(VT_VARIANT, 0, 1);
	variant_t vtStringLoadFromArg(sAssemblyFile);
	LONG index = 0;
	hr = SafeArrayPutElement(psaAssemblyLoadMethodArgs, &index, &vtStringLoadFromArg);
	if (FAILED(hr))
	{
		META_CONPRINTF("SafeArrayPutElement failed vtStringLoadFromArg w/hr 0x%08lx\n", hr);
		SafeArrayDestroy(psaAssemblyLoadMethodArgs);
		this->Cleanup();
		return false;
	}

	// Invoke the "GetStringLength" method from the Type interface.
	bstr_t bstrLoadFromMethodName(L"LoadFrom");
	variant_t vtLoadFromMethodTarget = NULL;
	variant_t vtLoadFromMethodOutput = NULL;
	hr = this->m_Type_System_Reflection_Assembly->InvokeMember_3(bstrLoadFromMethodName, static_cast<BindingFlags>(
		BindingFlags_InvokeMethod | BindingFlags_Static | BindingFlags_Public),
		NULL, vtLoadFromMethodTarget, psaAssemblyLoadMethodArgs, &vtLoadFromMethodOutput);
	SafeArrayDestroy(psaAssemblyLoadMethodArgs);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to invoke LoadFrom w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	this->m_Assembly_DotNetPlug_Managed = (_AssemblyPtr)vtLoadFromMethodOutput;

	META_CONPRINTF("Assembly file loaded : %s\n", sAssemblyFile);

	GET_TYPE(this->m_Assembly_DotNetPlug_Managed, "DotNetPlug.PluginManager", &this->m_Type_DotNetPlug_PluginManager);
	GET_TYPE(this->m_Assembly_DotNetPlug_Managed, "DotNetPlug.IPluginManager", &this->m_Type_DotNetPlug_IPluginManager);
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, "RaiseGameEvent", &this->m_Method_DotNetPlug_IPluginManager_RaiseGameEvent);
	//GET_TYPE(this->m_Assembly_DotNetPlug_Managed, "DotNetPlug.TypeHelper", &this->m_Type_DotNetPlug_TypeHelper);

	//Load System.Core
	//LOAD_ASSEMBLY(this->spDefaultAppDomain, "System.Core", &this->m_Assembly_System_Core);
	//GET_TYPE(this->m_Assembly_System_Core, "System.Dynamic.ExpandoObject", &this->m_Type_System_Dynamic_ExpandoObject);
	//hr = this->m_Type_DotNetPlug_TypeHelper->GetMethod_2(bstr_t("ExpandoAdd"), (BindingFlags)(BindingFlags_Public | BindingFlags_Static), &this->m_Method_DotNetPlug_TypeHelper_ExpandoAdd);
	//hr = this->m_Type_DotNetPlug_TypeHelper->GetMethod_2(bstr_t("ExpandoNew"), (BindingFlags)(BindingFlags_Public | BindingFlags_Static), &this->m_Method_DotNetPlug_TypeHelper_ExpandoNew);

	bstr_t spPropName(L"Instance");
	hr = this->m_Type_DotNetPlug_PluginManager->GetProperty(spPropName, (BindingFlags)(BindingFlags_NonPublic | BindingFlags_Static), &this->m_Property_DotNetPlug_PluginManager_Instance);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to get PluginManager Instance PropertyInfo w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	hr = this->m_Property_DotNetPlug_PluginManager_Instance->GetGetMethod(true, &this->m_Method_DotNetPlug_PluginManager_Instance_Get);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to get PluginManager Instance Property GetMethod w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	hr = this->m_Method_DotNetPlug_PluginManager_Instance_Get->Invoke_3((variant_t)NULL, NULL, &vtPluginManager);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to get PluginManager Instance Property value w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	////////////////////////////
	// PluginManager Methods
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"Tick", &spPluginManagerTick);
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"Load", &spPluginManagerLoad);
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"Unload", &spPluginManagerUnload);
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"LoadAssembly", &spPluginManagerLoadAssembly);
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"RaiseCommand", &spPluginManagerRaiseCommand);
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"RaiseLevelInit", &spPluginManagerLevelInit);
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"RaiseServerActivate", &spPluginManagerServerActivate);

	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"RaiseLevelShutdown", &spPluginManagerLevelShutdown);
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"RaiseClientActive", &spPluginManagerClientActive);
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"RaiseClientDisconnect", &spPluginManagerClientDisconnect);
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"RaiseClientPutInServer", &spPluginManagerClientPutInServer);
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"RaiseClientSettingsChanged", &spPluginManagerClientSettingsChanged);
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"RaiseClientConnect", &spPluginManagerClientConnect);
	GETMETHOD(hr, this->m_Type_DotNetPlug_IPluginManager, L"RaiseClientCommand", &spPluginManagerClientCommand);

	////////////////////////////
	// Callbacks from managed to native : FunctionPointers
	GETMETHOD_F(hr, this->m_Type_DotNetPlug_PluginManager, L"InitWin32Engine", &spPluginManagerInitWin32Engine, (BindingFlags)(BindingFlags_Instance | BindingFlags_NonPublic));

	////////////////////////////
	// Callback : assign callbacks in PluginManager
	SAFEARRAY* params = SafeArrayCreateVector(VT_VARIANT, 0, 4);
	hr = SET_CALLBACK(params, 0, (LONGLONG)&Managed::Log);
	hr = SET_CALLBACK(params, 1, (LONGLONG)&Managed::ExecuteCommand);
	hr = SET_CALLBACK(params, 2, (LONGLONG)&Managed::RegisterCommand);
	hr = SET_CALLBACK(params, 3, (LONGLONG)&Managed::UnregisterCommand);

	variant_t vtEmptyCallback;
	hr = this->spPluginManagerInitWin32Engine->Invoke_3(this->vtPluginManager, params, &vtEmptyCallback);
	SafeArrayDestroy(params);

	if (FAILED(hr))	{

		META_CONPRINTF("Call failed InitWin32Engine w/hr 0x%08lxn", hr);
		this->Cleanup();
		return false;
	}

	META_LOG(g_PLAPI, "PluginManager Ready");
	s_inited = true;
	return true;
}

void Managed::Load()
{
	variant_t vtOutput = NULL;
	HRESULT hr = this->spPluginManagerLoad->Invoke_3(this->vtPluginManager, NULL, &vtOutput);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to call PluginManager.Load w/hr 0x%08lx\n", hr);
	}
}

void Managed::Tick()
{
	variant_t vtOutput = NULL;
	HRESULT hr = this->spPluginManagerTick->Invoke_3(this->vtPluginManager, NULL, &vtOutput);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to call PluginManager.Tick w/hr 0x%08lx\n", hr);
	}
}

void Managed::Unload(){
	variant_t vtEmptyCallback;
	HRESULT hr = this->spPluginManagerUnload->Invoke_3(this->vtPluginManager, NULL, &vtEmptyCallback);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to call PluginManager.Unload w/hr 0x%08lx\n", hr);
	}
	this->Cleanup();
	this->s_inited = false;
}

void Managed::RaiseCommandPlateform(ManagedCommand* pCmd, int argc, const char** argv)
{
	HRESULT hr;
	long i;

	SAFEARRAY *methodArgs = SafeArrayCreateVector(VT_VARIANT, 0, 3);
	VARIANT vtId;
	i = 0;
	VariantInit(&vtId);
	vtId.vt = VT_INT;
	vtId.intVal = pCmd->GetId();
	hr = SafeArrayPutElement(methodArgs, &i, &vtId);

	VARIANT vtArgc;
	i = 1;
	VariantInit(&vtArgc);
	vtArgc.vt = VT_INT;
	vtArgc.intVal = argc;
	hr = SafeArrayPutElement(methodArgs, &i, &vtArgc);

	VARIANT vtArgv;
	i = 2;
	hr = CREATE_STRING_ARRAY(argc, argv, &vtArgv);
	hr = SafeArrayPutElement(methodArgs, &i, &vtArgv);

	variant_t vtOutput = NULL;
	hr = this->spPluginManagerRaiseCommand->Invoke_3(this->vtPluginManager, methodArgs, &vtOutput);
}

void Managed::RaiseLevelInit(const char *pMapName, const char *pMapEntities, const char *pOldLevel, const char *pLandmarkName, bool loadGame, bool background)
{
	HRESULT hr;
	long i;

	SAFEARRAY *methodArgs = SafeArrayCreateVector(VT_VARIANT, 0, 6);
	i = 0;
	hr = SET_STRING_PARAM(methodArgs, &i, pMapName);
	i = 1;
	hr = SET_STRING_PARAM(methodArgs, &i, pMapEntities);
	i = 2;
	hr = SET_STRING_PARAM(methodArgs, &i, pOldLevel);
	i = 3;
	hr = SET_STRING_PARAM(methodArgs, &i, pLandmarkName);
	i = 4;
	hr = SET_BOOL_PARAM(methodArgs, &i, loadGame);
	i = 5;
	hr = SET_BOOL_PARAM(methodArgs, &i, background);

	variant_t vtOutput = NULL;
	hr = this->spPluginManagerLevelInit->Invoke_3(this->vtPluginManager, methodArgs, &vtOutput);
	hr = SafeArrayDestroy(methodArgs);
}

void Managed::RaiseServerActivate(int clientMax)
{
	HRESULT hr;
	long i;

	SAFEARRAY *methodArgs = SafeArrayCreateVector(VT_VARIANT, 0, 1);
	i = 0;
	hr = SET_INT_PARAM(methodArgs, &i, clientMax);

	variant_t vtOutput = NULL;
	hr = this->spPluginManagerServerActivate->Invoke_3(this->vtPluginManager, methodArgs, &vtOutput);
	hr = SafeArrayDestroy(methodArgs);
}

void Managed::RaiseLevelShutdown()
{
	HRESULT hr;

	variant_t vtOutput = NULL;
	hr = this->spPluginManagerLevelShutdown->Invoke_3(this->vtPluginManager, NULL, &vtOutput);
}

void Managed::RaiseClientActive()
{
	HRESULT hr;

	variant_t vtOutput = NULL;
	hr = this->spPluginManagerClientActive->Invoke_3(this->vtPluginManager, NULL, &vtOutput);
}

void Managed::RaiseClientDisconnect()
{
	HRESULT hr;

	variant_t vtOutput = NULL;
	hr = this->spPluginManagerClientDisconnect->Invoke_3(this->vtPluginManager, NULL, &vtOutput);
}

void Managed::RaiseClientPutInServer()
{
	HRESULT hr;

	variant_t vtOutput = NULL;
	hr = this->spPluginManagerClientPutInServer->Invoke_3(this->vtPluginManager, NULL, &vtOutput);
}

void Managed::RaiseClientSettingsChanged()
{
	HRESULT hr;

	variant_t vtOutput = NULL;
	hr = this->spPluginManagerClientSettingsChanged->Invoke_3(this->vtPluginManager, NULL, &vtOutput);
}

void Managed::RaiseClientConnect()
{
	HRESULT hr;

	variant_t vtOutput = NULL;
	hr = this->spPluginManagerClientConnect->Invoke_3(this->vtPluginManager, NULL, &vtOutput);
}

void Managed::RaiseClientCommand()
{
	HRESULT hr;

	variant_t vtOutput = NULL;
	hr = this->spPluginManagerClientCommand->Invoke_3(this->vtPluginManager, NULL, &vtOutput);
}

void Managed::RaiseGameEvent(GameEvent e, IGameEvent *event)
{
	variant_t vtNull;
	HRESULT hr;

	long i = 0;
	bool raise = true;

	NativeEventData* nativeEvent = (NativeEventData*)CoTaskMemAlloc(sizeof(NativeEventData));
	ZeroMemory(nativeEvent, sizeof(NativeEventData));
	nativeEvent->Event = e;
	nativeEvent->argsCount = 0;
	//nativeEvent->eventName = bstr_t(event->GetName());

	switch (e){
	case player_death:
		ADD_SHORT(nativeEvent, event, "userid");
		ADD_STRING(nativeEvent, event, "weapon");
		//nativeEvent->args[nativeEvent->argsCount++].SetShort("userid", event->GetInt("userid"));
		//nativeEvent->args[nativeEvent->argsCount++].SetString("weapon", event->GetString("weapon"));
		//hr = this->m_Method_DotNetPlug_TypeHelper_ExpandoNew->Invoke_3(vtNull, NULL, &vtExpando);
		////CREATE_INSTANCE(this->m_Assembly_System_Core, "System.Dynamic.ExpandoObject", &vtExpando);
		////hr = this->m_Type_DotNetPlug_TypeHelper->InvokeMember_3(bstr_t("ExpandoNew"), (BindingFlags)(BindingFlags_Public | BindingFlags_Static), NULL, 
		//hr = SET_EXPANDO_STRING_FROM_EVENT_SHORT(vtExpando, event, "userid");
		//hr = SET_EXPANDO_STRING_FROM_EVENT_SHORT(vtExpando, event, "attacker");
		//hr = SET_EXPANDO_STRING_FROM_EVENT_SHORT(vtExpando, event, "assister");
		//hr = SET_EXPANDO_STRING_FROM_EVENT_STRING(vtExpando, event, "weapon");
		//hr = SET_EXPANDO_STRING_FROM_EVENT_STRING(vtExpando, event, "weapon_itemid");
		//hr = SET_EXPANDO_STRING_FROM_EVENT_STRING(vtExpando, event, "weapon_fauxitemid");
		//hr = SET_EXPANDO_STRING_FROM_EVENT_STRING(vtExpando, event, "weapon_originalowner_xuid");
		//hr = SET_EXPANDO_STRING_FROM_EVENT_BOOL(vtExpando, event, "headshot");
		//hr = SET_EXPANDO_STRING_FROM_EVENT_SHORT(vtExpando, event, "dominated");
		//hr = SET_EXPANDO_STRING_FROM_EVENT_SHORT(vtExpando, event, "revenge");
		//hr = SET_EXPANDO_STRING_FROM_EVENT_SHORT(vtExpando, event, "penetrated");
		break;
	case round_start:
		ADD_LONG(nativeEvent, event, "timelimit");
		ADD_LONG(nativeEvent, event, "fraglimit");
		ADD_STRING(nativeEvent, event, "objective");
		break;
	case None:
	default:
		META_LOG(g_PLAPI, "Unsupported event: %s", event->GetName());
		raise = raise;
		break;
	}

	if (raise)
	{
		SAFEARRAY* args = SafeArrayCreateVector(VT_VARIANT, 0, 1);
		hr = SET_CALLBACK(args, 0, (LONGLONG)nativeEvent);
		hr = this->m_Method_DotNetPlug_IPluginManager_RaiseGameEvent->Invoke_3(this->vtPluginManager, args, &vtNull);
		hr = SafeArrayDestroy(args);
	}
	CoTaskMemFree(nativeEvent);
}

#endif
