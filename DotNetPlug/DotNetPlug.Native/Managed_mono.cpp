#include "Managed.h"
#include "Plugin.h"

#ifdef MANAGED_MONO

#define GETCLASS(destPtr, pImage, sNamespace, sName) {	\
	destPtr = mono_class_from_name(pImage, sNamespace, sName);	\
	if(!destPtr) {	\
		META_LOG(g_PLAPI, "Can't get class %s::%s\n", sNamespace, sName);	\
		return false;	\
																}	\
}

#define GETMETHOD(destPtr, pClass, sMethodName, iParamCount) {	\
	destPtr = mono_class_get_method_from_name(pClass, sMethodName, iParamCount);	\
	if(!destPtr) {	\
		META_LOG(g_PLAPI, "Can't get method %s (with %d params)\n", sMethodName, iParamCount);	\
		return false;	\
																	}	\
}
#define GETMETHODIMPL(destPtr, pObject, pMethod, sMethodName) {	\
	destPtr = mono_object_get_virtual_method(pObject, pMethod);	\
	if(!destPtr) {	\
		META_LOG(g_PLAPI, "Can't get method implementation of %s\n", sMethodName);	\
		return false;	\
																		}	\
}

void Managed::Cleanup()
{
	if (this->pAssemblyImage)
	{
		mono_image_close(this->pAssemblyImage);
		this->pAssemblyImage = NULL;
	}
	if (this->pDomain)
	{
		mono_jit_cleanup(this->pDomain);
		this->pDomain = NULL;
	}
}

bool Managed::InitPlateform(const char* sAssemblyFile)
{
	MonoObject* exception = NULL;

	mono_config_parse(NULL);

	this->pDomain = mono_jit_init_version("DotNetPlug", "v4.0.30319");
	if (!this->pDomain)
	{
		META_LOG(g_PLAPI, "Can't initialize Mono jit\n");
		return false;
	}

	this->pAssembly = mono_domain_assembly_open(this->pDomain, sAssemblyFile);
	if (!this->pAssembly)
	{
		META_LOG(g_PLAPI, "Can't load assembly\n");
		return false;
	}

	this->pAssemblyImage = mono_assembly_get_image(this->pAssembly);
	if (!this->pAssemblyImage)
	{
		META_LOG(g_PLAPI, "Can't get assembly image\n");
		return false;
	}

	GETCLASS(this->pStringClass, NULL, "System", "String");

	GETCLASS(this->pPluginManagerClass, this->pAssemblyImage, "DotNetPlug", "PluginManager");
	GETCLASS(this->pIPluginManagerClass, this->pAssemblyImage, "DotNetPlug", "IPluginManager");

	this->pPluginManagerInstanceProperty = mono_class_get_property_from_name(this->pPluginManagerClass, "Instance");
	if (!this->pPluginManagerInstanceProperty)
	{
		META_LOG(g_PLAPI, "Can't get PluginManager.Instance property \n");
		return false;
	}

	this->pPluginManagerInstancePropertyGetMethod = mono_property_get_get_method(this->pPluginManagerInstanceProperty);
	if (!this->pPluginManagerInstancePropertyGetMethod)
	{
		META_LOG(g_PLAPI, "Can't get PluginManager.Instance property get method \n");
		return false;
	}

	//Get object instance
	this->pPluginManagerInstanceObject = mono_runtime_invoke(this->pPluginManagerInstancePropertyGetMethod, NULL, NULL, &exception);
	if (!this->pPluginManagerInstanceObject)
	{
		META_LOG(g_PLAPI, "Can't get PluginManager instance \n");
		if (exception)
		{
			mono_print_unhandled_exception(exception);
		}
		return false;
	}
	/*this->pIPluginManagerInstanceObject = mono_object_castclass_mbyref(this->pPluginManagerInstanceObject, this->pIPluginManagerClass);
	if (!this->pIPluginManagerInstanceObject)
	{
	META_LOG(g_PLAPI, "Can't cast PluginManager instance to IPluginManager \n");
	return false;
	}*/

	////////////////////////////
	// PluginManager Methods
	GETMETHOD(this->pPluginManagerLoadMethod, this->pIPluginManagerClass, "Load", 0);
	GETMETHODIMPL(this->pPluginManagerLoadMethodImplementation, this->pPluginManagerInstanceObject, this->pPluginManagerLoadMethod, "Load");
	GETMETHOD(this->pPluginManagerTickMethod, this->pIPluginManagerClass, "Tick", 0);
	GETMETHODIMPL(this->pPluginManagerTickMethodImplementation, this->pPluginManagerInstanceObject, this->pPluginManagerTickMethod, "Tick");
	GETMETHOD(this->pPluginManagerUnloadMethod, this->pIPluginManagerClass, "Unload", 0);
	GETMETHODIMPL(this->pPluginManagerUnloadMethodImplementation, this->pPluginManagerInstanceObject, this->pPluginManagerUnloadMethod, "Unload");

	GETMETHOD(this->pPluginManagerLoadAssemblyMethod, this->pIPluginManagerClass, "LoadAssembly", 1);
	GETMETHODIMPL(this->pPluginManagerLoadAssemblyMethodImplementation, this->pPluginManagerInstanceObject, this->pPluginManagerLoadAssemblyMethod, "LoadAssembly");

	GETMETHOD(this->pPluginManagerRaiseCommandMethod, this->pIPluginManagerClass, "LoadAssembly", 1);
	GETMETHODIMPL(this->pPluginManagerRaiseCommandMethodImplementation, this->pPluginManagerInstanceObject, this->pPluginManagerRaiseCommandMethod, "RaiseCommand");

	////////////////////////////
	// Callbacks from managed to native : DllImport
	mono_add_internal_call("DotNetPlug.PluginManagerMono::Log", (void*)(&Managed::LogMono));
	mono_add_internal_call("DotNetPlug.PluginManagerMono::ExecuteCommand", (void*)(&Managed::ExecuteCommandMono));
	mono_add_internal_call("DotNetPlug.PluginManagerMono::RegisterCommand", (void*)(&Managed::RegisterCommandMono));

	////////////////////////////
	// Callback : assign callbacks in PluginManager
	GETMETHOD(this->pMapCallbacksToMono, this->pPluginManagerClass, "MapCallbacksToMono", 0);
	exception = NULL;
	mono_runtime_invoke(this->pMapCallbacksToMono, this->pPluginManagerInstanceObject, NULL, &exception);

	META_LOG(g_PLAPI, "PluginManager Ready");
	s_inited = true;
	return true;
}

void Managed::Unload()
{
	MonoObject* exception = NULL;
	mono_runtime_invoke(this->pPluginManagerUnloadMethodImplementation, this->pPluginManagerInstanceObject, NULL, &exception);
	if (exception){
		mono_print_unhandled_exception(exception);
	}
	this->s_inited = false;
	// mono_jit_cleanup(this->pDomain);
}

void Managed::Tick()
{
	MonoObject* exception = NULL;
	if (this->pPluginManagerTickMethodImplementation)
	{
		mono_runtime_invoke(this->pPluginManagerTickMethodImplementation, this->pPluginManagerInstanceObject, NULL, &exception);
		if (exception){
			mono_print_unhandled_exception(exception);
			META_LOG(g_PLAPI, "Disconnecting Tick Method\n");
			this->pPluginManagerTickMethodImplementation = NULL;
		}
	}
}

void Managed::Load()
{
	MonoObject* exception = NULL;
	mono_runtime_invoke(this->pPluginManagerLoadMethodImplementation, this->pPluginManagerInstanceObject, NULL, &exception);
	if (exception){
		mono_print_unhandled_exception(exception);
	}
}

void Managed::LogMono(MonoString* pMsg){
	if (!pMsg)
		return;
	char* pString = mono_string_to_utf8(pMsg);
	Managed::Log(pString);
	mono_free(pString);
}

void Managed::ExecuteCommandMono(MonoString* pCommand, MonoString* pOutput, int* pLength){
	if (!pCommand)
	{
		pOutput = NULL;
		*pLength = 0;
		return;
	}

	char* pString = mono_string_to_utf8(pCommand);
	void* dest = NULL;
	int destLen = 0;
	Managed::ExecuteCommand(pString, &dest, &destLen);
	mono_free(pString);

	MonoString* pResultMono = mono_string_new(g_Managed.pDomain, (char*)dest);
	delete dest;

	pOutput = pResultMono;
	*pLength = mono_string_length(pOutput);

	//icvar->InstallConsoleDisplayFunc
}

void Managed::RegisterCommandMono(MonoString* pCommand, MonoString* pDescription, int flags, int id)
{
	char* pCmd = mono_string_to_utf8(pCommand);
	char* pDesc = mono_string_to_utf8(pDescription);
	Managed::RegisterCommand(pCmd, pDesc, flags, id);
	mono_free(pCmd);
	mono_free(pDesc);
}

void Managed::RaiseCommandPlateform(ManagedCommand* cmd, int argc, const char** argv)
{
	int id = cmd->GetId();

	void *args[3];
	args[0] = &id;
	args[1] = &argc;
	MonoArray* pArgs = mono_array_new(this->pDomain, this->pStringClass, argc);
	for (int i = 0; i < argc; i++){
		MonoString* str = mono_string_new(this->pDomain, argv[i]);
		mono_array_set(pArgs, MonoString*, i, str);
	}
	args[2] = pArgs;
	MonoObject* exception = NULL;
	mono_runtime_invoke(this->pPluginManagerRaiseCommandMethodImplementation, this->pPluginManagerInstanceObject, args, &exception);
	if (exception){
		mono_print_unhandled_exception(exception);
	}
}

void Managed::RaiseLevelInit(const char *pMapName, const char *pMapEntities, const char *pOldLevel, const char *pLandmarkName, bool loadGame, bool background)
{
	META_LOG(g_PLAPI, "TODO : RaiseLevelInit\n");
}

void Managed::RaiseServerActivate(int clientMax)
{
	META_LOG(g_PLAPI, "TODO : RaiseServerActivate\n");
}

void Managed::RaiseLevelShutdown()
{
	META_LOG(g_PLAPI, "TODO : RaiseLevelShutdown\n");
}

void Managed::RaiseClientActive()
{
	META_LOG(g_PLAPI, "TODO : RaiseClientActive\n");
}

void Managed::RaiseClientDisconnect()
{
	META_LOG(g_PLAPI, "TODO : RaiseClientDisconnect\n");
}

void Managed::RaiseClientPutInServer()
{
	META_LOG(g_PLAPI, "TODO : RaiseClientPutInServer\n");
}

void Managed::RaiseClientSettingsChanged()
{
	META_LOG(g_PLAPI, "TODO : RaiseClientSettingsChanged\n");
}

void Managed::RaiseClientConnect()
{
	META_LOG(g_PLAPI, "TODO : RaiseClientConnect\n");
}

void Managed::RaiseClientCommand()
{
	META_LOG(g_PLAPI, "TODO : RaiseClientCommand\n");
}
#endif