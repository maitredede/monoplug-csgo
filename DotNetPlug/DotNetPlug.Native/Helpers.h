#ifndef _DOTNETPLUG_HELPERS_H_
#define _DOTNETPLUG_HELPERS_H_

#include <stdlib.h>
#include <map>
#include "Player.h"

struct char_cmp {
	bool operator () (const char *a, const char *b) const
	{
		return strcmp(a, b) < 0;
	}
};

#ifdef MANAGED_WIN32
#include <comutil.h>
#include <stdio.h>

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

#endif //MANAGED_WIN32

bool FindPlayerByEntity(player_t* player);
bool FStrEq(const char* str1, const char* str2);
void GetIPAddressFromPlayer(player_t* player);

#if !defined ZeroMemory
#define ZeroMemory(ptr, size) memset(ptr, 0, size);
#endif

#endif //_DOTNETPLUG_HELPERS_H_