#ifndef _CMONOHELPERS_H_
#define _CMONOHELPERS_H_

#include "monoBase.h"

namespace MonoPlugin
{
	class CMonoHelpers
	{
	public:
		static bool GetDomain(const char* file, const char* rootDir, MonoDomain*& domain, char *error, size_t maxlen);
		static bool GetAssembly(const char* file, MonoDomain* domain, MonoAssembly*& assembly, char *error, size_t maxlen);
		static bool GetImage(const char* file, MonoAssembly* assembly, MonoImage*& image, char *error, size_t maxlen);
		static bool GetClass(MonoImage* assemblyImage, const char* classNamespace, const char* className, MonoClass*& classPtr, char* error, size_t maxlen);
		static bool GetMethod(MonoImage* assemblyImage, const char* methodSig, MonoMethod*& methodptr, char *error, size_t maxlen);
		static bool GetField(MonoClass* classPtr, MonoClassField*& fieldPtr, const char* fieldName, char *error, size_t maxlen);

		static MonoString* GetString(MonoDomain* domain, const char* str);
		static MonoArray* GetStringArray(MonoDomain* domain, mono_array_size_t argc, const char** nArray);
		static MonoObject* CallMethod(void* target, MonoMethod* methodHandle, void** args = NULL);
		static MonoObject* CallDelegate(MonoDelegate* delegateObject, void** args);
		static MonoObject* ClassNew(MonoDomain* domain, MonoClass *cls);
	};
}

PLUGIN_GLOBALVARS();

#endif //_CMONOHELPERS_H_
