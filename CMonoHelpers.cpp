#include "CMonoHelpers.h"

//MonoObject* CMonoHelpers::MONO_CALL(void* target, MonoMethod* methodHandle)
//{
//	return CMonoHelpers::MONO_CALL_ARGS(target, methodHandle, NULL);
//}

MonoObject* CMonoHelpers::MONO_CALL(void* target, MonoMethod* methodHandle, void** args)
{
	MonoObject* exception = NULL;
	MonoObject* ret = mono_runtime_invoke(methodHandle, target, args, &exception);
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
