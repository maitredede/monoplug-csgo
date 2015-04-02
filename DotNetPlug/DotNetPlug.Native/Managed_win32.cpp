#include "Managed.h"
#include "Plugin.h"

#ifdef MANAGED_WIN32

#include <comutil.h>
#include <stdio.h>
#include "Helpers.h"

#pragma comment(lib, "comsuppw.lib")
#pragma comment(lib, "kernel32.lib")

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
	this->pszVersion = L"v4.0.30319";
	this->pszAssemblyName = L"DotNetPlug.Managed";
	this->pszClassName = L"DotNetPlug.PluginManager";
	this->pszIfaceName = L"DotNetPlug.IPluginManager";

	HRESULT hr;

	// The .NET assembly to load.
	//bstr_t bstrAssemblyName(pszAssemblyName);
	bstr_t bstrAssemblyName(sAssemblyFile);
	_AssemblyPtr spAssembly = NULL;

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
	hr = pRuntimeInfo->GetInterface(CLSID_CorRuntimeHost,
		IID_PPV_ARGS(&pCorRuntimeHost));
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

	IUnknownPtr spAppDomainThunk = NULL;
	_AppDomainPtr spDefaultAppDomain = NULL;

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
	bstr_t bstrMscorlibAssemblyName(L"mscorlib");
	_AssemblyPtr spMscorlibAssembly = NULL;
	hr = hr = spDefaultAppDomain->Load_2(bstrMscorlibAssemblyName, &spMscorlibAssembly);
	if (FAILED(hr)){
		META_CONPRINTF("Failed to load the assembly mscorlib w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	// Get the Type of System.Reflection.Assembly.
	bstr_t bstrAssemblyClassName(L"System.Reflection.Assembly");
	_TypePtr spAssemblyType = NULL;
	hr = spMscorlibAssembly->GetType_2(bstrAssemblyClassName, &spAssemblyType);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to get the Type interface System.Reflection.Assembly w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	// Call the static method of the class: 
	//   public static int GetStringLength(string str);

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

	hr = spAssemblyType->InvokeMember_3(bstrLoadFromMethodName, static_cast<BindingFlags>(
		BindingFlags_InvokeMethod | BindingFlags_Static | BindingFlags_Public),
		NULL, vtLoadFromMethodTarget, psaAssemblyLoadMethodArgs, &vtLoadFromMethodOutput);
	SafeArrayDestroy(psaAssemblyLoadMethodArgs);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to invoke LoadFrom w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	_AssemblyPtr assembly = (_AssemblyPtr)vtLoadFromMethodOutput;

	META_CONPRINTF("Assembly file loaded : %s\n", sAssemblyFile);

	_Type* spPluginManagerType = NULL;
	bstr_t bstrPluginManagerClassName(this->pszClassName);
	hr = assembly->GetType_2(bstrPluginManagerClassName, &spPluginManagerType);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to get PluginManager type w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	_PropertyInfo* spPluginManagerInstanceProperty = NULL;
	bstr_t spPropName(L"Instance");
	hr = spPluginManagerType->GetProperty(spPropName, (BindingFlags)(BindingFlags_NonPublic | BindingFlags_Static), &spPluginManagerInstanceProperty);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to get PluginManager Instance PropertyInfo w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	_MethodInfo* spPluginManagerInstancePropertyGet = NULL;
	hr = spPluginManagerInstanceProperty->GetGetMethod(true, &spPluginManagerInstancePropertyGet);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to get PluginManager Instance Property GetMethod w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	hr = spPluginManagerInstancePropertyGet->Invoke_3((variant_t)NULL, NULL, &vtPluginManager);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to get PluginManager Instance Property value w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	_Type* spIPluginManagerType = NULL;
	bstr_t bstrIPluginManagerClassName(this->pszIfaceName);
	hr = assembly->GetType_2(bstrIPluginManagerClassName, &spIPluginManagerType);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to get IPluginManager type w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	////////////////////////////
	// PluginManager Methods
	GETMETHOD(hr, spIPluginManagerType, L"Tick", &spPluginManagerTick);
	GETMETHOD(hr, spIPluginManagerType, L"Load", &spPluginManagerLoad);
	GETMETHOD(hr, spIPluginManagerType, L"Unload", &spPluginManagerUnload);
	GETMETHOD(hr, spIPluginManagerType, L"LoadAssembly", &spPluginManagerLoadAssembly);
	GETMETHOD(hr, spIPluginManagerType, L"RaiseCommand", &spPluginManagerRaiseCommand);

	////////////////////////////
	// Callbacks from managed to native : FunctionPointers
	GETMETHOD_F(hr, spPluginManagerType, L"InitWin32Engine", &spPluginManagerInitWin32Engine, (BindingFlags)(BindingFlags_Instance | BindingFlags_NonPublic));

	////////////////////////////
	// Callback : assign callbacks in PluginManager
	//SETCALLBACK(hr, &Managed::Log, spPluginManagerSetCallback_Log);
	//SETCALLBACK(hr, &Managed::ExecuteCommand, spPluginManagerSetCallback_ExecuteCommand);
	//SETCALLBACK(hr, &Managed::RegisterCommand, spPluginManagerSetCallback_RegisterCommand);

	SAFEARRAY* params = SafeArrayCreateVector(VT_VARIANT, 0, 3);
	hr = SET_CALLBACK(params, 0, (LONGLONG)&Managed::Log);
	hr = SET_CALLBACK(params, 1, (LONGLONG)&Managed::ExecuteCommand);
	hr = SET_CALLBACK(params, 2, (LONGLONG)&Managed::RegisterCommand);

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

void Managed::Tick()
{
	variant_t vtOutput = NULL;
	this->spPluginManagerTick->Invoke_3(this->vtPluginManager, NULL, &vtOutput);
}

void Managed::Load()
{
	variant_t vtOutput = NULL;
	this->spPluginManagerLoad->Invoke_3(this->vtPluginManager, NULL, &vtOutput);
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
#endif
