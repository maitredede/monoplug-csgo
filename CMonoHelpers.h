#ifndef _CMONOHELPERS_H_
#define _CMONOHELPERS_H_

#include "Common.h"

class CMonoHelpers
{
public:
	static MonoObject* MONO_CALL(void* target, MonoMethod* methodHandle, void** args = NULL);
	static MonoObject* MONO_DELEGATE_CALL(MonoDelegate* delegateObject, void** args);
};

#endif //_CMONOHELPERS_H_
