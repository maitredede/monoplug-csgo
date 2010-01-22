#include "CMonoHelpers.h"

//MonoObject* CMonoHelpers::MONO_CALL(void* target, MonoMethod* methodHandle)
//{
//	return CMonoHelpers::MONO_CALL_ARGS(target, methodHandle, NULL);
//}

MonoString* CMonoHelpers::MONO_STRING(MonoDomain* domain, const char* string)
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

MonoObject* CMonoHelpers::MONO_CALL(void* target, MonoMethod* methodHandle, void** args)
{
	if(methodHandle == NULL) META_CONPRINT("MONO_CALL ERROR : methodHandle is NULL\n");

	MonoObject* exception = NULL;
	MonoObject* ret = mono_runtime_invoke(methodHandle, target, args, &exception);
	if(exception)
	{
		mono_print_unhandled_exception(exception);
		//mono_property_get_value(mono_class_get_property_from_name(mono_class_from_name(, "
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
};

//ml_get_prop_string(MonoObject *obj, char *field)
//{
//	MonoClass *klass;
//	MonoProperty *prop;
//	MonoString *str;
//	
//	klass = mono_object_get_class(obj);
//	
//	prop = mono_class_get_property_from_name(klass, field);
//	
//	str = (MonoString*)mono_property_get_value(prop, obj, NULL, NULL);
//	
//	return mono_string_to_utf8(str);
//}


MonoObject* CMonoHelpers::MONO_DELEGATE_CALL(MonoDelegate* delegateObject, void** args)
{
	//Code from : http://www.mail-archive.com/mono-list@lists.ximian.com/msg26230.html
	//MonoClass* dlgClass = mono_object_get_class((MonoObject*)delegateObject);
	//MonoMethod* dlgMethod = mono_get_delegate_invoke(dlgClass);
	//return MONO_CALL_ARGS(delegateObject, dlgMethod, args);
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
};
