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
	NativeEventArgs* nativeArgs = (NativeEventArgs*)CoTaskMemAlloc(sizeof(NativeEventArgs) * NATIVE_EVENT_ARGS_MAX);
	ZeroMemory(nativeArgs, sizeof(NativeEventArgs) * NATIVE_EVENT_ARGS_MAX);
	//nativeEvent->args = nativeArgs;

	nativeEvent->Event = e;
	nativeEvent->argsCount = 0;
	//nativeEvent->eventName = bstr_t(event->GetName());

	switch (e){
	case player_death:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "attacker");
		ADD_SHORT(nativeEvent, nativeArgs, event, "assister");
		ADD_STRING(nativeEvent, nativeArgs, event, "weapon");
		ADD_STRING(nativeEvent, nativeArgs, event, "weapon_itemid");
		ADD_STRING(nativeEvent, nativeArgs, event, "weapon_fauxitemid");
		ADD_STRING(nativeEvent, nativeArgs, event, "weapon_originalowner_xuid");
		ADD_BOOL(nativeEvent, nativeArgs, event, "headshot");
		ADD_SHORT(nativeEvent, nativeArgs, event, "dominated");
		ADD_SHORT(nativeEvent, nativeArgs, event, "revenge");
		ADD_SHORT(nativeEvent, nativeArgs, event, "penetrated");
		break;
	case player_hurt:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "attacker");
		ADD_BYTE(nativeEvent, nativeArgs, event, "health");
		ADD_BYTE(nativeEvent, nativeArgs, event, "armor");
		ADD_STRING(nativeEvent, nativeArgs, event, "weapon");
		ADD_SHORT(nativeEvent, nativeArgs, event, "dmg_health");
		ADD_BYTE(nativeEvent, nativeArgs, event, "dmg_armor");
		ADD_BYTE(nativeEvent, nativeArgs, event, "hitgroup");
		break;
	case item_purchase:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "team");
		ADD_STRING(nativeEvent, nativeArgs, event, "weapon");
		break;
	case bomb_beginplant:
	case bomb_abortplant:
	case bomb_planted:
	case bomb_defused:
	case bomb_exploded:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "site");
		break;
	case bomb_dropped:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_LONG(nativeEvent, nativeArgs, event, "entindex");
		break;

	case bomb_pickup:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;

	case defuser_dropped:
		ADD_LONG(nativeEvent, nativeArgs, event, "entityid");
		break;
	case defuser_pickup:
		ADD_LONG(nativeEvent, nativeArgs, event, "entityid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;

	case announce_phase_end:
		break;
	case cs_intermission:
		break;
	case bomb_begindefuse:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_BOOL(nativeEvent, nativeArgs, event, "haskit");
		break;
	case bomb_abortdefuse:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;
	case hostage_follows:
	case hostage_hurt:
	case hostage_killed:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "hostage");
		break;
	case hostage_rescued:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "hostage");
		ADD_SHORT(nativeEvent, nativeArgs, event, "site");
		break;
	case hostage_stops_following:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "hostage");
		break;
	case hostage_rescued_all:
		break;
	case hostage_call_for_help:
		ADD_SHORT(nativeEvent, nativeArgs, event, "hostage");
		break;
	case vip_escaped:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;
	case vip_killed:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "attacker");
		break;
	case player_radio:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "slot");
		break;
	case bomb_beep:
		ADD_LONG(nativeEvent, nativeArgs, event, "entindex");
		break;
	case weapon_fire:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_STRING(nativeEvent, nativeArgs, event, "weapon");
		ADD_BOOL(nativeEvent, nativeArgs, event, "silenced");
		break;
	case weapon_fire_on_empty:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_STRING(nativeEvent, nativeArgs, event, "weapon");
		break;
	case weapon_outofammo:
	case weapon_reload:
	case weapon_zoom:
	case silencer_detach:
	case inspect_weapon:
	case weapon_zoom_rifle:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;
	case player_spawned:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_BOOL(nativeEvent, nativeArgs, event, "inrestart");
		break;
	case item_pickup:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_STRING(nativeEvent, nativeArgs, event, "item");
		ADD_BOOL(nativeEvent, nativeArgs, event, "silent");
		break;
	case ammo_pickup:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_STRING(nativeEvent, nativeArgs, event, "item");
		ADD_LONG(nativeEvent, nativeArgs, event, "index");
		break;
	case item_equip:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_STRING(nativeEvent, nativeArgs, event, "item");
		ADD_BOOL(nativeEvent, nativeArgs, event, "canzoom");
		ADD_BOOL(nativeEvent, nativeArgs, event, "hassilencer");
		ADD_BOOL(nativeEvent, nativeArgs, event, "issilenced");
		ADD_BOOL(nativeEvent, nativeArgs, event, "hastracers");
		ADD_SHORT(nativeEvent, nativeArgs, event, "weptype");
		ADD_BOOL(nativeEvent, nativeArgs, event, "ispainted");
		break;
	case enter_buyzone:
	case exit_buyzone:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_BOOL(nativeEvent, nativeArgs, event, "canbuy");
		break;
	case buytime_ended:
		break;
	case enter_bombzone:
	case exit_bombzone:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_BOOL(nativeEvent, nativeArgs, event, "hasbomb");
		ADD_BOOL(nativeEvent, nativeArgs, event, "isplanted");
		break;
	case enter_rescue_zone:
	case exit_rescue_zone:
	case silencer_off:
	case silencer_on:
	case buymenu_open:
	case buymenu_close:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;
	case round_prestart:
	case round_poststart:
		break;
	case round_start:
		ADD_LONG(nativeEvent, nativeArgs, event, "timelimit");
		ADD_LONG(nativeEvent, nativeArgs, event, "fraglimit");
		ADD_STRING(nativeEvent, nativeArgs, event, "objective");
		break;
	case round_end:
		ADD_BYTE(nativeEvent, nativeArgs, event, "winner");
		ADD_BYTE(nativeEvent, nativeArgs, event, "reason");
		ADD_STRING(nativeEvent, nativeArgs, event, "message");
		break;
	case grenade_bounce:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;
	case hegrenade_detonate:
	case flashbang_detonate:
	case smokegrenade_detonate:
	case smokegrenade_expired:
	case molotov_detonate:
	case decoy_detonate:
	case decoy_started:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "entityid");
		ADD_FLOAT(nativeEvent, nativeArgs, event, "x");
		ADD_FLOAT(nativeEvent, nativeArgs, event, "y");
		ADD_FLOAT(nativeEvent, nativeArgs, event, "z");
		break;
	case inferno_startburn:
	case inferno_expire:
	case inferno_extinguish:
		ADD_SHORT(nativeEvent, nativeArgs, event, "entityid");
		ADD_FLOAT(nativeEvent, nativeArgs, event, "x");
		ADD_FLOAT(nativeEvent, nativeArgs, event, "y");
		ADD_FLOAT(nativeEvent, nativeArgs, event, "z");
		break;
	case decoy_firing:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "entityid");
		ADD_FLOAT(nativeEvent, nativeArgs, event, "x");
		ADD_FLOAT(nativeEvent, nativeArgs, event, "y");
		ADD_FLOAT(nativeEvent, nativeArgs, event, "z");
		break;
	case bullet_impact:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_FLOAT(nativeEvent, nativeArgs, event, "x");
		ADD_FLOAT(nativeEvent, nativeArgs, event, "y");
		ADD_FLOAT(nativeEvent, nativeArgs, event, "z");
		break;
	case player_footstep:
	case player_jump:
	case player_blind:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;
	case player_falldamage:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_FLOAT(nativeEvent, nativeArgs, event, "damage");
		break;
	case door_moving:
		ADD_LONG(nativeEvent, nativeArgs, event, "entindex");
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;
	case round_freeze_end:
	case mb_input_lock_success:
	case mb_input_lock_cancel:
		break;
	case nav_blocked:
		ADD_LONG(nativeEvent, nativeArgs, event, "area");
		ADD_BOOL(nativeEvent, nativeArgs, event, "blocked");
		break;
	case nav_generate:
		break;
	case player_stats_updated:
		ADD_BOOL(nativeEvent, nativeArgs, event, "forceupload");
		break;
	case achievement_info_loaded:
		break;
	case spec_target_updated:
	case spec_mode_updated:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;
	case hltv_changed_mode:
		ADD_LONG(nativeEvent, nativeArgs, event, "oldmode");
		ADD_LONG(nativeEvent, nativeArgs, event, "newmode");
		ADD_LONG(nativeEvent, nativeArgs, event, "obs_target");
		break;
	case cs_game_disconnected:
		break;
	case cs_win_panel_round:
		ADD_BOOL(nativeEvent, nativeArgs, event, "show_timer_defend");
		ADD_BOOL(nativeEvent, nativeArgs, event, "show_timer_attack");
		ADD_SHORT(nativeEvent, nativeArgs, event, "timer_time");
		ADD_BYTE(nativeEvent, nativeArgs, event, "final_event");
		ADD_STRING(nativeEvent, nativeArgs, event, "funfact_token");
		ADD_SHORT(nativeEvent, nativeArgs, event, "funfact_player");
		ADD_LONG(nativeEvent, nativeArgs, event, "funfact_data1");
		ADD_LONG(nativeEvent, nativeArgs, event, "funfact_data2");
		ADD_LONG(nativeEvent, nativeArgs, event, "funfact_data3");
		break;
	case cs_win_panel_match:
	case cs_match_end_restart:
	case cs_pre_restart:
		break;
	case show_freezepanel:
		ADD_SHORT(nativeEvent, nativeArgs, event, "victim");
		ADD_SHORT(nativeEvent, nativeArgs, event, "killer");
		ADD_SHORT(nativeEvent, nativeArgs, event, "hits_taken");
		ADD_SHORT(nativeEvent, nativeArgs, event, "damage_taken");
		ADD_SHORT(nativeEvent, nativeArgs, event, "hits_given");
		ADD_SHORT(nativeEvent, nativeArgs, event, "damage_given");
		break;
	case hide_freezepanel:
	case freezecam_started:
		break;
	case player_avenged_teammate:
		ADD_SHORT(nativeEvent, nativeArgs, event, "avenger_id");
		ADD_SHORT(nativeEvent, nativeArgs, event, "avenged_player_id");
		break;
	case achievement_earned:
		ADD_BYTE(nativeEvent, nativeArgs, event, "player");
		ADD_SHORT(nativeEvent, nativeArgs, event, "achievement");
		break;
	case achievement_earned_local:
		ADD_SHORT(nativeEvent, nativeArgs, event, "achievement");
		ADD_SHORT(nativeEvent, nativeArgs, event, "splitscreenplayer");
		break;
	case item_found:
		ADD_BYTE(nativeEvent, nativeArgs, event, "player");
		ADD_BYTE(nativeEvent, nativeArgs, event, "quality");
		ADD_BYTE(nativeEvent, nativeArgs, event, "method");
		ADD_LONG(nativeEvent, nativeArgs, event, "itemdef");
		ADD_LONG(nativeEvent, nativeArgs, event, "itemid");
		break;
	case item_gifted:
		ADD_BYTE(nativeEvent, nativeArgs, event, "player");
		ADD_LONG(nativeEvent, nativeArgs, event, "itemdef");
		ADD_BYTE(nativeEvent, nativeArgs, event, "numgifts");
		ADD_BYTE(nativeEvent, nativeArgs, event, "giftidx");
		ADD_LONG(nativeEvent, nativeArgs, event, "accountid");
		break;
	case repost_xbox_achievements:
		ADD_SHORT(nativeEvent, nativeArgs, event, "splitscreenplayer");
		break;
	case match_end_conditions:
		ADD_LONG(nativeEvent, nativeArgs, event, "frags");
		ADD_LONG(nativeEvent, nativeArgs, event, "max_rounds");
		ADD_LONG(nativeEvent, nativeArgs, event, "win_rounds");
		ADD_LONG(nativeEvent, nativeArgs, event, "time");
		break;
	case round_mvp:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "reason");
		break;
	case player_decal:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;
	case teamplay_round_start:
		ADD_BOOL(nativeEvent, nativeArgs, event, "full_reset");
		break;
	case client_disconnect:
		break;
	case gg_player_levelup:
	case ggtr_player_levelup:
	case ggprogressive_player_levelup:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "weaponrank");
		ADD_STRING(nativeEvent, nativeArgs, event, "weaponname");
		break;
	case gg_killed_enemy:
		ADD_SHORT(nativeEvent, nativeArgs, event, "victimid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "attackerid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "dominated");
		ADD_SHORT(nativeEvent, nativeArgs, event, "revenge");
		ADD_BOOL(nativeEvent, nativeArgs, event, "bonus");
		break;
	case gg_final_weapon_achieved:
	case gg_bonus_grenade_achieved:
		ADD_SHORT(nativeEvent, nativeArgs, event, "playerid");
		break;
	case switch_team:
		ADD_SHORT(nativeEvent, nativeArgs, event, "numPlayers");
		ADD_SHORT(nativeEvent, nativeArgs, event, "numSpectators");
		ADD_SHORT(nativeEvent, nativeArgs, event, "avg_rank");
		ADD_SHORT(nativeEvent, nativeArgs, event, "numTSlotsFree");
		ADD_SHORT(nativeEvent, nativeArgs, event, "numCTSlotsFree");
		break;
	case gg_leader:
		ADD_SHORT(nativeEvent, nativeArgs, event, "playerid");
		break;
	case gg_player_impending_upgrade:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;
	case write_profile_data:
		break;
	case trial_time_expired:
		ADD_SHORT(nativeEvent, nativeArgs, event, "slot");
		break;
	case update_matchmaking_stats:
		break;
	case player_reset_vote:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "vote");
		break;
	case enable_restart_voting:
		ADD_BOOL(nativeEvent, nativeArgs, event, "enable");
		break;
	case sfuievent:
		ADD_STRING(nativeEvent, nativeArgs, event, "action");
		ADD_STRING(nativeEvent, nativeArgs, event, "data");
		ADD_BYTE(nativeEvent, nativeArgs, event, "slot");
		break;
	case start_vote:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_BYTE(nativeEvent, nativeArgs, event, "type");
		ADD_SHORT(nativeEvent, nativeArgs, event, "vote_parameter");
		break;
	case player_given_c4:
	case gg_reset_round_start_sounds:
	case tr_player_flashbanged:
	case tr_highlight_ammo:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;
	case tr_mark_complete:
		ADD_SHORT(nativeEvent, nativeArgs, event, "complete");
		break;
	case tr_mark_best_time:
		ADD_LONG(nativeEvent, nativeArgs, event, "time");
		break;
	case tr_exit_hint_trigger:
		break;
	case bot_takeover:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "botid");
		ADD_SHORT(nativeEvent, nativeArgs, event, "index");
		break;
	case tr_show_finish_msgbox:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;
	case tr_show_exit_msgbox:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		break;
	case reset_player_controls:
		break;
	case jointeam_failed:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_BYTE(nativeEvent, nativeArgs, event, "reason");
		break;
	case teamchange_pending:
		ADD_SHORT(nativeEvent, nativeArgs, event, "userid");
		ADD_BYTE(nativeEvent, nativeArgs, event, "toteam");
		break;
	case material_default_complete:
		break;
	case cs_prev_next_spectator:
		ADD_BOOL(nativeEvent, nativeArgs, event, "next");
		break;
	case cs_handle_ime_event:
		// TODO : cs_handle_ime_event
		raise = false;
		break;
	case nextlevel_changed:
		ADD_STRING(nativeEvent, nativeArgs, event, "nextlevel");
		break;
	case seasoncoin_levelup:
		ADD_SHORT(nativeEvent, nativeArgs, event, "player");
		ADD_SHORT(nativeEvent, nativeArgs, event, "category");
		ADD_SHORT(nativeEvent, nativeArgs, event, "rank");
		break;
	case tournament_reward:
		ADD_LONG(nativeEvent, nativeArgs, event, "defindex");
		ADD_LONG(nativeEvent, nativeArgs, event, "totalrewards");
		ADD_LONG(nativeEvent, nativeArgs, event, "accountid");
		break;
	case start_halftime:
		break;
	case None:
	default:
		//META_LOG(g_PLAPI, "Unsupported event: %s", event->GetName());
		raise = false;
		break;
	}

	if (raise)
	{
		SAFEARRAY* args = SafeArrayCreateVector(VT_VARIANT, 0, 3);
		hr = SET_CALLBACK(args, 0, (LONGLONG)nativeEvent);
		hr = SET_INT(args, 1, nativeEvent->argsCount);
		hr = SET_CALLBACK(args, 2, (LONGLONG)nativeArgs);
		hr = this->m_Method_DotNetPlug_IPluginManager_RaiseGameEvent->Invoke_3(this->vtPluginManager, args, &vtNull);
		hr = SafeArrayDestroy(args);
	}
	CoTaskMemFree(nativeArgs);
	CoTaskMemFree(nativeEvent);
}

#endif
