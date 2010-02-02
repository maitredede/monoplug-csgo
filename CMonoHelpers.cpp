#include "CMonoHelpers.h"

bool MonoPlugin::CMonoHelpers::GetDomain(const char* file, const char* rootDir, MonoDomain*& domain, char *error, size_t maxlen)
{
	domain = mono_jit_init(file);
	if(!domain)
	{
		g_SMAPI->Format(error, maxlen, "Can't init Mono JIT");
		return false;
	}
	return true;
}

bool MonoPlugin::CMonoHelpers::GetAssembly(const char* file, MonoDomain* domain, MonoAssembly*& assembly, char *error, size_t maxlen)
{
	assembly = mono_domain_assembly_open(domain, file);
	if(!assembly)
	{
		g_SMAPI->Format(error, maxlen, "Can't open assembly : %s", file);
		return false;
	}
	return true;
}

bool MonoPlugin::CMonoHelpers::GetImage(const char* file, MonoAssembly* assembly, MonoImage*& image, char *error, size_t maxlen)
{
	image = mono_assembly_get_image(assembly);
	if(!image)
	{
		g_SMAPI->Format(error, maxlen, "Can't get assembly image");
		return false;
	}
	return true;
}

bool MonoPlugin::CMonoHelpers::GetClass(MonoImage *assemblyImage, const char *classNamespace, const char *className, MonoClass*& classPtr, char *error, size_t maxlen)
{
	classPtr = mono_class_from_name(assemblyImage, classNamespace, className);
	if(!classPtr)
	{
		g_SMAPI->Format(error, maxlen, "Can't get type %s", className);
		return false;
	}
	return true;
}

MonoString* MonoPlugin::CMonoHelpers::GetString(MonoDomain* domain, const char* string)
{
	if(string == NULL)
	{
		return NULL;
	}
	else
	{
		return mono_string_new(domain, string);
	}
}

MonoObject* MonoPlugin::CMonoHelpers::CallDelegate(MonoDelegate *delegateObject, void **args)
{
	MonoObject* exception = NULL;
	MonoObject* ret = mono_runtime_delegate_invoke((MonoObject*)delegateObject, args, &exception);
	if(exception)
	{
		mono_print_unhandled_exception(exception);
		return NULL;
	}
	else
	{
		return ret;
	}
}

MonoObject* MonoPlugin::CMonoHelpers::CallMethod(void *target, MonoMethod *methodHandle, void **args)
{
	if(methodHandle == NULL) META_CONPRINT("MONO_CALL ERROR : methodHandle is NULL\n");

	MonoObject* exception = NULL;
	MonoObject* ret = mono_runtime_invoke(methodHandle, target, args, &exception);
	if(exception)
	{
		mono_print_unhandled_exception(exception);
		
		MonoClass* ex_class = mono_object_get_class(exception);

		MonoProperty* ex_prop_message = mono_class_get_property_from_name(ex_class, "Message");
		MonoProperty* ex_prop_stacktrace = mono_class_get_property_from_name(ex_class, "StackTrace");
		MonoString* msg = (MonoString*)mono_property_get_value(ex_prop_message, exception, NULL, NULL);
		MonoString* stk = (MonoString*)mono_property_get_value(ex_prop_stacktrace, exception, NULL, NULL);

		META_CONPRINTF("MONO_CALL ERROR: %s\n", mono_string_to_utf8(msg));
		META_CONPRINTF("MONO_CALL ERROR: %s\n", mono_string_to_utf8(stk));

		return NULL;
	}
	else
	{
		return ret;
	}
}

MonoObject* MonoPlugin::CMonoHelpers::ClassNew(MonoDomain* domain, MonoClass *cls)
{
	MonoObject* obj = mono_object_new(domain, cls);
	mono_runtime_object_init(obj);
	return obj;
}

bool MonoPlugin::CMonoHelpers::GetMethod(MonoImage* assemblyImage, const char* methodSig, MonoMethod*& methodptr, char *error, size_t maxlen)
{
	MonoMethodDesc* desc = mono_method_desc_new(methodSig, true);
	if(!desc)
	{
		g_SMAPI->Format(error, maxlen, "Can't get desc for method %s", methodSig);
		return false;
	}
	methodptr = mono_method_desc_search_in_image(desc, assemblyImage);
	mono_method_desc_free(desc);
	if(!methodptr)
	{
		g_SMAPI->Format(error, maxlen, "Can't get method pointer for %s", methodSig);
		return false;
	} 
	return true;
}
