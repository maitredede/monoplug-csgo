#ifndef _DOTNETPLUG_HELPERS_H_
#define _DOTNETPLUG_HELPERS_H_

#if !defined ZeroMemory
#define ZeroMemory(ptr, size) memset(ptr, 0, size);
#endif

#include <stdlib.h>
#include <map>

struct char_cmp {
	bool operator () (const char *a, const char *b) const
	{
		return strcmp(a, b) < 0;
	}
};
//typedef std::map<const char *, int, char_cmp> Map;
//template<typename TElem>
//std::map<const char*, TElem, char_cmp> StringDic<T>

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

//#define SETCALLBACK(hr, funcPtr, setCallbackMethodInfo) {	\
//	LONG index = 0;	\
//	SAFEARRAY* params = SafeArrayCreateVector(VT_VARIANT, 0, 1);	\
//	variant_t vtEmptyCallback;	\
//	variant_t params0;	\
//	params0.llVal = (LONGLONG)funcPtr;	\
//	params0.vt = VT_I8;	\
//	hr = SafeArrayPutElement(params, &index, &params0);	\
//	if (FAILED(hr))	{	\
//		META_CONPRINTF("SafeArrayPutElement failed SetCallback_Log 0 w/hr 0x%08lx\n", hr);	\
//		this->Cleanup();	\
//		SafeArrayDestroy(params);	\
//		return false;	\
//						}	\
//	hr = setCallbackMethodInfo->Invoke_3(this->vtPluginManager, params, &vtEmptyCallback);	\
//	SafeArrayDestroy(params);	\
//	if (FAILED(hr))	{	\
//		META_CONPRINTF("Call failed SETCALLBACK w/hr 0x%08lx\n", hr);	\
//		this->Cleanup();	\
//		return false;	\
//						}	\
//}
inline HRESULT SET_CALLBACK(SAFEARRAY* params, long idx, LONGLONG funcPtr)
{
	HRESULT hr;
	variant_t params0;
	params0.llVal = funcPtr;
	params0.vt = VT_I8;
	hr = SafeArrayPutElement(params, &idx, &params0);
	return hr;
};

inline HRESULT CREATE_STRING_ARRAY(int argc, const char** argv, VARIANT* vtPsa)
{
	long i;
	HRESULT hr;

	vtPsa->vt = (VT_ARRAY | VT_BSTR);
	vtPsa->parray = SafeArrayCreateVector(VT_BSTR, 0, argc); // create an array of strings

	for (i = 0; i < argc; i++)
	{
		VARIANT item;
		VariantInit(&item);
		item.vt = VT_BSTR;
		item.bstrVal = _com_util::ConvertStringToBSTR(argv[i]);
		hr = SafeArrayPutElement(vtPsa->parray, &i, item.bstrVal); // insert the string from argv[i] into the safearray
		if (FAILED(hr))
		{
			SafeArrayDestroy(vtPsa->parray);
			SysFreeString(item.bstrVal);
			return hr;
		}
		SysFreeString(item.bstrVal);
	}
	return hr;
}

inline HRESULT CREATE_STRING_ARRAY_ARGS(int argc, const char** argv, SAFEARRAY** params)
{
	HRESULT hr;
	long i = 0;
	*params = SafeArrayCreateVector(VT_VARIANT, 0, 1);
	VARIANT vtPsa;
	hr = CREATE_STRING_ARRAY(argc, argv, &vtPsa);
	i = 0;
	hr = SafeArrayPutElement(*params, &i, &vtPsa);
	return hr;
}
#endif

#endif //_DOTNETPLUG_HELPERS_H_