#ifndef _DOTNETPLUG_HELPERS_H_
#define _DOTNETPLUG_HELPERS_H_
#ifdef _WIN32
#pragma once
#endif

#include <stdlib.h>
#include <map>
#include "Player.h"
#include <igameevents.h>

struct char_cmp {
	bool operator () (const char *a, const char *b) const
	{
		return strcmp(a, b) < 0;
	}
};

#ifdef MANAGED_WIN32
#include <comutil.h>
#include <stdio.h>
#include <windows.h>
#include <metahost.h>

#pragma comment(lib, "mscoree.lib")

#import <mscorlib.tlb> raw_interfaces_only					\
	high_property_prefixes("_get","_put","_putref")			\
	rename("ReportEvent", "InteropServices_ReportEvent")	\
	rename("Assert", "_Assert")

// #import "DotNetPlug.Managed.tlb" raw_interfaces_only

using namespace mscorlib;

#define GETMETHOD_F(hr, tType, lpwstrName, ppOut, flags) {	\
	bstr_t bstrMethodName(lpwstrName);				\
	hr = tType->GetMethod_2(bstrMethodName, flags, ppOut);	\
	if (FAILED(hr))	\
																					{	\
		META_CONPRINTF("Failed to get method %s w/hr 0x%08lx\n", lpwstrName, hr);	\
		this->Cleanup();	\
		return false;	\
																					}	\
}

#define GETMETHOD(hr, tType, lpwstrName, ppOut) GETMETHOD_F(hr, tType, lpwstrName, ppOut, (BindingFlags)(BindingFlags_Instance | BindingFlags_Public))

HRESULT SET_CALLBACK(SAFEARRAY* params, long idx, LONGLONG funcPtr);
HRESULT CREATE_STRING_ARRAY(int argc, const char** argv, VARIANT* vtPsa);
HRESULT CREATE_STRING_ARRAY_ARGS(int argc, const char** argv, long paramIndex, SAFEARRAY** params);
HRESULT SET_STRING_PARAM(SAFEARRAY* psa, LONG* i, const char* pStr);
HRESULT SET_BOOL_PARAM(SAFEARRAY* psa, LONG* i, bool bVal);
HRESULT SET_INT_PARAM(SAFEARRAY* psa, LONG* i, int iVal);
HRESULT CREATE_INSTANCE(_AssemblyPtr spAssembly, const char* className, VARIANT* vtInstance);

//HRESULT SET_EXPANDO_STRING_FROM_EVENT_SHORT(variant_t vtExpando, IGameEvent *evt, const char* name);
//HRESULT SET_EXPANDO_STRING_FROM_EVENT_STRING(variant_t vtExpando, IGameEvent *evt, const char* name);
//HRESULT SET_EXPANDO_STRING_FROM_EVENT_BOOL(variant_t vtExpando, IGameEvent *evt, const char* name);

HRESULT GET_TYPE_FUNC(_AssemblyPtr pAssembly, const char* sType, _Type** pType);
HRESULT LOAD_ASSEMBLY_FUNC(_AppDomainPtr pAppDomain, const char* sAssemblyName, _Assembly** pAssembly);
#endif //MANAGED_WIN32

#ifdef MANAGED_MONO
#include <mono/jit/jit.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/assembly.h>
#endif //MANAGED_WIN32

bool FindPlayerByEntity(player_t* player);
bool FindPlayerByIndex(player_t *player_ptr);
bool FStrEq(const char* str1, const char* str2);
void GetIPAddressFromPlayer(player_t* player);


#if !defined ZeroMemory
#define ZeroMemory(ptr, size) memset(ptr, 0, size);
#endif

#if defined WIN32 && !defined snprintf
#define snprintf _snprintf
#endif

#endif //_DOTNETPLUG_HELPERS_H_