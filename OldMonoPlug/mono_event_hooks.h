#ifndef _MONO_EVENT_HOOKS_H_
#define _MONO_EVENT_HOOKS_H_

//Mono
#include <glib-2.0/glib.h>
#include <mono/jit/jit.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>

//SampleMM
#include <ISmmPlugin.h>
#include <igameevents.h>
#include <iplayerinfo.h>
#include <sh_vector.h>
#include "engine_wrappers.h"
#include <filesystem.h>

#define MONO_EVENT_DECL_HOOK0_VOID(ifacetype, ifacefunc, attr, overload, ifaceptr, post) \
SH_DECL_HOOK0_void(ifacetype, ifacefunc, attr, overload); \
MonoMethod* g_MethodRaise##ifacefunc = NULL; \
static void EventHookRaise_##ifacefunc() \
{ \
	if(g_MethodRaise##ifacefunc) \
	{ \
		META_CONPRINTF("EVT : native raise %s::%s\n", #ifacetype, #ifaceptr); \
		MONO_CALL(g_MonoPlugPlugin.m_ClsMain, g_MethodRaise##ifacefunc); \
	} \
}; \
static void EventHookAttach_##ifacefunc() \
{ \
	META_CONPRINTF("EVT : add hook %s::%s\n", #ifacetype, #ifaceptr); \
	SH_ADD_HOOK_STATICFUNC(ifacetype, ifacefunc, ifaceptr, &EventHookRaise_##ifacefunc, post); \
}; \
static void EventHookDetach_##ifacefunc() \
{ \
	META_CONPRINTF("EVT : remove hook %s::%s\n", #ifacetype, #ifaceptr); \
	SH_REMOVE_HOOK_STATICFUNC(ifacetype, ifacefunc, ifaceptr, &EventHookRaise_##ifacefunc, post); \
};

#define MONO_EVENT_INIT_HOOK0_VOID(ifacefunc, managedRaise, managedAttach, managedDetach) \
	META_CONPRINTF("EVT : hook init %s\n", #ifacefunc); \
	ATTACH(managedRaise, g_MethodRaise##ifacefunc, g_Image); \
	mono_add_internal_call(managedAttach, EventHookAttach_##ifacefunc); \
	mono_add_internal_call(managedDetach, EventHookDetach_##ifacefunc);

#define MONO_EVENT_UNLOAD_HOOK0_VOID(ifacefunc) g_MethodRaise##ifacefunc = NULL;

/////////////////////////////////////////////
// 2
/////////////////////////////////////////////
#define MONO_EVENT_DECL_HOOK2_VOID(ifacetype, ifacefunc, attr, overload, ifaceptr, post, t1, t2) \
SH_DECL_HOOK2_void(ifacetype, ifacefunc, attr, overload, t1, t2); \
MonoMethod* g_MethodRaise##ifacefunc = NULL; \
static void EventHookRaise_##ifacefunc(t1 p1, t2 p2); \
static void EventHookAttach_##ifacefunc() \
{ \
	META_CONPRINTF("EVT : add hook %s::%s -> %s\n", #ifacetype, #ifacefunc, #ifaceptr); \
	SH_ADD_HOOK_STATICFUNC(ifacetype, ifacefunc, ifaceptr, &EventHookRaise_##ifacefunc, post); \
}; \
static void EventHookDetach_##ifacefunc() \
{ \
	META_CONPRINTF("EVT : remove hook %s::%s -> %s\n", #ifacetype, #ifacefunc, #ifaceptr); \
	SH_REMOVE_HOOK_STATICFUNC(ifacetype, ifacefunc, ifaceptr, &EventHookRaise_##ifacefunc, post); \
}; \
static void EventHookRaiseReal_##ifacefunc(MonoMethod* method, t1 p1, t2 p2); \
static void EventHookRaise_##ifacefunc(t1 p1, t2 p2) \
{ \
	if(g_MethodRaise##ifacefunc) \
	{ \
		META_CONPRINTF("EVT : native raise %s::%s\n", #ifacetype, #ifaceptr); \
		EventHookRaiseReal_##ifacefunc(g_MethodRaise##ifacefunc, p1, p2); \
	} \
}; \
static void EventHookRaiseReal_##ifacefunc(MonoMethod* method, t1 param1, t2 param2)


#define MONO_EVENT_INIT_HOOK2_VOID(ifacefunc, managedRaise, managedAttach, managedDetach) \
	META_CONPRINTF("EVT : hook init %s\n", #ifacefunc); \
	ATTACH(managedRaise, g_MethodRaise##ifacefunc, g_Image); \
	mono_add_internal_call(managedAttach, EventHookAttach_##ifacefunc); \
	mono_add_internal_call(managedDetach, EventHookDetach_##ifacefunc);

#define MONO_EVENT_UNLOAD_HOOK2_VOID(ifacefunc) g_MethodRaise##ifacefunc = NULL;

#endif //_MONO_EVENT_HOOKS_H_
