#include "Managed.h"
#include "Plugin.h"

Managed::Managed()
{
	s_inited = false;

	this->pMetaHost = NULL;
	this->pRuntimeInfo = NULL;
	this->pCorRuntimeHost = NULL;

	this->psaStaticMethodArgs = NULL;
	this->psaMethodArgs = NULL;

	this->pszVersion = L"v4.0.30319";
	this->pszAssemblyName = L"DotNetPlug.Managed";
	this->pszClassName = L"DotNetPlug.PluginManager";
}

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
		//wprintf(L"Stop the .NET runtime\n");
		//pCorRuntimeHost->Stop();

		pCorRuntimeHost->Release();
		pCorRuntimeHost = NULL;
	}

	if (psaStaticMethodArgs)
	{
		SafeArrayDestroy(psaStaticMethodArgs);
		psaStaticMethodArgs = NULL;
	}
	if (psaMethodArgs)
	{
		SafeArrayDestroy(psaMethodArgs);
		psaMethodArgs = NULL;
	}
}

bool Managed::Init(const char* sBaseDir)
{
	if (s_inited)
		return true;

	HRESULT hr;

	char sAssemblyFile[MAX_PATH];
	ZeroMemory(sAssemblyFile, MAX_PATH);

	V_ComposeFileName(sBaseDir, "dotnetplug/DotNetPlug.Managed.dll", sAssemblyFile, MAX_PATH);

	// The .NET assembly to load.
	//bstr_t bstrAssemblyName(pszAssemblyName);
	bstr_t bstrAssemblyName(sAssemblyFile);
	_AssemblyPtr spAssembly = NULL;

	// The .NET class to instantiate.
	bstr_t bstrClassName(pszClassName);
	_TypePtr spType = NULL;
	variant_t vtObject;
	variant_t vtEmpty;

	// The static method in the .NET class to invoke.
	bstr_t bstrStaticMethodName(L"GetStringLength");
	SAFEARRAY *psaStaticMethodArgs = NULL;
	variant_t vtStringArg(L"HelloWorld");
	variant_t vtLengthRet;

	// The instance method in the .NET class to invoke.
	bstr_t bstrMethodName(L"ToString");
	SAFEARRAY *psaMethodArgs = NULL;
	variant_t vtStringRet;

	// Load and start the .NET runtime.
	hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&pMetaHost));
	if (FAILED(hr))
	{
		wprintf(L"CLRCreateInstance failed w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	// Get the ICLRRuntimeInfo corresponding to a particular CLR version. It 
	// supersedes CorBindToRuntimeEx with STARTUP_LOADER_SAFEMODE.
	hr = pMetaHost->GetRuntime(pszVersion, IID_PPV_ARGS(&pRuntimeInfo));
	if (FAILED(hr))
	{
		wprintf(L"ICLRMetaHost::GetRuntime failed w/hr 0x%08lx\n", hr);
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
		wprintf(L"ICLRRuntimeInfo::IsLoadable failed w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	if (!fLoadable)
	{
		wprintf(L".NET runtime %s cannot be loaded\n", pszVersion);
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
		wprintf(L"ICLRRuntimeInfo::GetInterface failed w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	// Start the CLR.
	hr = pCorRuntimeHost->Start();
	if (FAILED(hr))
	{
		wprintf(L"CLR failed to start w/hr 0x%08lx\n", hr);
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
		wprintf(L"ICorRuntimeHost::GetDefaultDomain failed w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	hr = spAppDomainThunk->QueryInterface(IID_PPV_ARGS(&spDefaultAppDomain));
	if (FAILED(hr))
	{
		wprintf(L"Failed to get default AppDomain w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	//Load mscorlib
	bstr_t bstrMscorlibAssemblyName(L"mscorlib");
	_AssemblyPtr spMscorlibAssembly = NULL;
	hr = hr = spDefaultAppDomain->Load_2(bstrMscorlibAssemblyName, &spMscorlibAssembly);
	if (FAILED(hr)){
		wprintf(L"Failed to load the assembly mscorlib w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	// Get the Type of System.Reflection.Assembly.
	bstr_t bstrAssemblyClassName(L"System.Reflection.Assembly");
	_TypePtr spAssemblyType = NULL;
	hr = spMscorlibAssembly->GetType_2(bstrAssemblyClassName, &spAssemblyType);
	if (FAILED(hr))
	{
		wprintf(L"Failed to get the Type interface System.Reflection.Assembly w/hr 0x%08lx\n", hr);
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
		wprintf(L"SafeArrayPutElement failed vtStringLoadFromArg w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	// Invoke the "GetStringLength" method from the Type interface.
	bstr_t bstrLoadFromMethodName(L"LoadFrom");
	hr = spAssemblyType->InvokeMember_3(bstrLoadFromMethodName, static_cast<BindingFlags>(
		BindingFlags_InvokeMethod | BindingFlags_Static | BindingFlags_Public),
		NULL, vtEmpty, psaAssemblyLoadMethodArgs, &vtLengthRet);
	if (FAILED(hr))
	{
		wprintf(L"Failed to invoke LoadFrom w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	_AssemblyPtr assembly = (_AssemblyPtr)vtLengthRet;

	META_CONPRINTF("Assembly file loaded : %s\n", sAssemblyFile);

	_Type* spPluginManagerType = NULL;
	bstr_t bstrPluginManagerClassName(L"DotNetPlug.PluginManager");
	hr = assembly->GetType_2(bstrPluginManagerClassName, &spPluginManagerType);
	if (FAILED(hr))
	{
		wprintf(L"Failed to get PluginManager type w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	_PropertyInfo* spPluginManagerInstanceProperty = NULL;
	bstr_t spPropName(L"Instance");
	hr = spPluginManagerType->GetProperty(spPropName, (BindingFlags)(BindingFlags_NonPublic | BindingFlags_Static), &spPluginManagerInstanceProperty);
	if (FAILED(hr))
	{
		wprintf(L"Failed to get PluginManager Instance PropertyInfo w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	_MethodInfo* spPluginManagerInstancePropertyGet = NULL;
	hr = spPluginManagerInstanceProperty->GetGetMethod(true, &spPluginManagerInstancePropertyGet);
	if (FAILED(hr))
	{
		wprintf(L"Failed to get PluginManager Instance Property GetMethod w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	variant_t vtObj = NULL;
	variant_t vtRet = NULL;
	SAFEARRAY* param = NULL;
	hr = spPluginManagerInstancePropertyGet->Invoke_3(vtObj, param, &vtRet);
	if (FAILED(hr))
	{
		wprintf(L"Failed to get PluginManager Instance Property value w/hr 0x%08lx\n", hr);
		this->Cleanup();
		return false;
	}

	return true;
}

void Managed::Tick()
{

}