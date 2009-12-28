#ifndef _MONOPLUG_COMMON_H_
#define _MONOPLUG_COMMON_H_

#include <glib-2.0/glib.h>
#include <mono/jit/jit.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>

#include <ISmmPlugin.h>
#include <convar.h>
#include "engine_wrappers.h"
#include <filesystem.h>

#define MONOPLUG_DLLFILE "%s/addons/MonoPlug.Managed.dll"
#define MONOPLUG_NAMESPACE "MonoPlug"
#define MONOPLUG_CLASSNAME "ClsMain"
#define MONOPLUG_FULLCLASSNAME "MonoPlug.ClsMain"
#define MONOPLUG_CALLBACK_MSG "MonoPlug.ClsMain::Mono_Msg"

#define MONOPLUG_NATMAN_INIT "MonoPlug.ClsMain:_Init()"
#define MONOPLUG_NATMAN_SHUTDOWN "MonoPlug.ClsMain:_Shutdown()"
#define MONOPLUG_NATMAN_HANDLEMESSAGE "MonoPlug.ClsMain:_HandleMessages()"

#define MONOPLUG_CALLBACK_REGISTERCONCOMMAND "MonoPlug.ClsMain::Mono_RegisterConCommand(string,string,MonoPlug.ConCommandDelegate,int)"
#define MONOPLUG_CALLBACK_UNREGISTERCONCOMMAND "MonoPlug.ClsMain::Mono_UnregisterConCommand(string)"

#define MONOPLUG_CALLBACK_REGISTERCONVARSTRING "MonoPlug.ClsMain::Mono_RegisterConVarString(string,string,int,string)"
#define MONOPLUG_CALLBACK_CONVARSTRING_GETVALUE "MonoPlug.ClsMain::Mono_GetConVarStringValue(int)"
#define MONOPLUG_CALLBACK_CONVARSTRING_SETVALUE "MonoPlug.ClsMain::Mono_SetConVarStringValue(int,string)"
#define MONOPLUG_NATMAN_CONVARSTRING_VALUECHANGED "MonoPlug.ClsMain:_ConVarStringChanged(System.UInt64)"

#define MONOPLUG_CLSMAIN_EVT_LEVELINIT "MonoPlug.ClsMain:EVT_LevelInit(string,string,string,string,bool,bool)"
#define MONOPLUG_CLSMAIN_EVT_LEVELSHUTDOWN "MonoPlug.ClsMain:EVT_LevelShutdown()"

#if defined WIN32 && !defined snprintf
#define snprintf _snprintf
#endif

PLUGIN_GLOBALVARS();

// Class definitions
class CMonoPlug;
class MonoConCommand;
class MonoConVarString;
class BaseAccessor;

// EXTERN VARS
//extern BaseAccessor s_BaseAccessor;
extern MonoDomain* g_Domain;

extern MonoAssembly* g_Assembly;
extern MonoImage *g_Image;
extern MonoClass *g_Class;

extern MonoMethod* g_Init;
extern MonoMethod* g_Shutdown;
extern MonoMethod* g_HandleMessage;

extern MonoMethod* g_EVT_LevelInit;
extern MonoMethod* g_EVT_LevelShutdown;

extern CMonoPlug g_MonoPlugPlugin;

#define ATTACH(managedName, methodHandle, asmImage) \
{\
	MonoMethodDesc* desc = mono_method_desc_new(managedName, true); \
	if(!desc) \
	{ \
		Warning("Can't describe method %s\n", managedName); \
		return false; \
	} \
	methodHandle = mono_method_desc_search_in_image(desc, asmImage); \
	mono_method_desc_free(desc); \
	if(!methodHandle) \
	{ \
		Warning("Can't attach method %s\n", managedName); \
		return false; \
	} \
};

#define MONO_CALL_ARGS(target, methodHandle, args) \
{\
	MonoObject* exception = NULL; \
	mono_runtime_invoke(methodHandle, target, args, &exception); \
	if(exception) \
	{ \
		mono_print_unhandled_exception(exception); \
	} \
};

//Code from : http://www.mail-archive.com/mono-list@lists.ximian.com/msg26230.html
#define MONO_DELEGATE_CALL(delegateObject, args) \
{\
	MonoMethod* dlgMethod = mono_get_delegate_invoke(mono_object_get_class((MonoObject*)delegateObject)); \
	MONO_CALL_ARGS(delegateObject, dlgMethod, args); \
};

#define MONO_CALL(target, methodHandle) MONO_CALL_ARGS(target, methodHandle, NULL)

#define MONO_STRING(domain, str) ((str == NULL) ? NULL : mono_string_new(domain, str))

#endif //_MONOPLUG_COMMON_H_
