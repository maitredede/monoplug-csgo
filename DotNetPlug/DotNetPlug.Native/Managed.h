#ifndef _DOTNETPLUG_MANAGED_H_
#define _DOTNETPLUG_MANAGED_H_

#if defined WIN32

//#include <metahost.h>
//#include <mscoree.h>
//#pragma comment(lib, "mscoree.lib")

#include <windows.h>
#include <metahost.h>

#pragma comment(lib, "mscoree.lib")

// Import mscorlib.tlb (Microsoft Common Language Runtime Class Library).
//#import "mscorlib.tlb" raw_interfaces_only 				\
//    high_property_prefixes("_get","_put","_putref")		\
//    rename("ReportEvent", "InteropServices_ReportEvent")

#import <mscorlib.tlb> raw_interfaces_only					\
	high_property_prefixes("_get","_put","_putref")			\
	rename("ReportEvent", "InteropServices_ReportEvent")	\
	rename("Assert", "_Assert")


using namespace mscorlib;


#endif

class Managed{

public:
	Managed();
	bool Init(const char* sBaseDir);
	void Cleanup();
	void Tick();
private:
	bool s_inited;

#ifdef WIN32
private:
	ICLRMetaHost *pMetaHost;
	ICLRRuntimeInfo *pRuntimeInfo;
	// ICorRuntimeHost and ICLRRuntimeHost are the two CLR hosting interfaces
	// supported by CLR 4.0. Here we demo the ICorRuntimeHost interface that 
	// was provided in .NET v1.x, and is compatible with all .NET Frameworks. 
	ICorRuntimeHost *pCorRuntimeHost;

	//AppDomain
	//IUnknown *spAppDomainThunk;

	SAFEARRAY *psaStaticMethodArgs;
	SAFEARRAY *psaMethodArgs;

	LPWSTR pszVersion;
	LPWSTR pszAssemblyName;
	LPWSTR pszClassName;
#endif
};

extern Managed g_Managed;

#endif