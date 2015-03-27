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

	GETCLASS(this->pPluginManagerClass, this->pAssemblyImage, "DotNetPlug", "PluginManager");
	GETCLASS(this->pIPluginManagerClass, this->pAssemblyImage, "DotNetPlug", "IPluginManager");
	GETCLASS(this->pPluginManagerMonoClass, this->pAssemblyImage, "DotNetPlug", "PluginManagerMono");

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
	//Initial methods
	this->pPluginManagerAllPluginsLoadedMethod = mono_class_get_method_from_name(this->pIPluginManagerClass, "AllPluginsLoaded", 0);
	if (!this->pPluginManagerAllPluginsLoadedMethod)
	{
		META_LOG(g_PLAPI, "Can't get method AllPluginsLoaded \n");
		return false;
	}
	this->pPluginManagerAllPluginsLoadedMethodImplementation = mono_object_get_virtual_method(this->pPluginManagerInstanceObject, this->pPluginManagerAllPluginsLoadedMethod);
	if (!this->pPluginManagerAllPluginsLoadedMethod)
	{
		META_LOG(g_PLAPI, "Can't get method AllPluginsLoaded implementation on PluginManager Instance \n");
		return false;
	}
	GETMETHOD(this->pPluginManagerTickMethod, this->pIPluginManagerClass, "Tick", 0);
	GETMETHODIMPL(this->pPluginManagerTickMethodImplementation, this->pPluginManagerInstanceObject, this->pPluginManagerAllPluginsLoadedMethod, "Tick");


	//Callbacks
	mono_add_internal_call("DotNetPlug.PluginManagerMono::Log", (void*)(&Managed::LogMono));
	mono_add_internal_call("DotNetPlug.PluginManagerMono::ExecuteCommand", (void*)(&Managed::ExecuteCommandMono));


	GETMETHOD(this->pMapCallbacksToMono, this->pPluginManagerClass, "MapCallbacksToMono", 0);

	exception = NULL;
	mono_runtime_invoke(this->pMapCallbacksToMono, this->pPluginManagerInstanceObject, NULL, &exception);

	META_LOG(g_PLAPI, "\n\nMono Ready\n\n\n");
	return true;
}

void Managed::Unload()
{
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

void Managed::AllPluginsLoaded()
{
	MonoObject* exception = NULL;
	mono_runtime_invoke(this->pPluginManagerAllPluginsLoadedMethodImplementation, this->pPluginManagerInstanceObject, NULL, &exception);
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

MonoString* Managed::ExecuteCommandMono(MonoString* pMsg){
	return NULL;
}

#endif