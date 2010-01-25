#ifndef _COMMON_H_
#define _COMMON_H_

//Mono
#include <glib-2.0/glib.h>
#include <mono/jit/jit.h>
#include <mono/metadata/class.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>

#include <ISmmPlugin.h>
//#include <sourcehook/sourcehook.h>
#include <igameevents.h>
#include <iplayerinfo.h>
#include <sh_vector.h>
#include <utlmap.h>
#include <filesystem.h>
#include <convar.h>
#include <inetchannelinfo.h>

class CMonoPlug;
class CMonoPlugListener;
class CMonoCommand;
class CMonoConsole;

extern CMonoPlug g_MonoPlug;
extern CMonoPlugListener g_Listener;

extern MonoDomain *g_Domain;

extern IGameEventManager2 *g_GameEventManager;	
extern IVEngineServer *g_Engine;
extern IServerGameDLL *g_ServerDll;
extern IServerGameClients *g_ServerClients;
extern IPlayerInfoManager *g_playerinfomanager;
extern IServerPluginHelpers *g_helpers;
extern IServerPluginCallbacks *g_vsp_callbacks;
extern ICvar *icvar;
extern CGlobalVars *g_Globals;
extern IFileSystem *g_filesystem;

PLUGIN_GLOBALVARS();

#define MONOPLUG_DLLFILE "%s/addons/MonoPlug.Managed.dll"

#define MONO_GET_CLASS(assemblyImage, classPtr, classNamespace, className, errstr, maxstr) \
	classPtr = mono_class_from_name(assemblyImage, classNamespace, className); \
	if(!classPtr) \
	{ \
		Q_snprintf(errstr, maxstr, "Can't get type %s", className); \
		return false; \
	}

#define MONO_GET_FIELD(fieldPtr, classPtr, fieldName, errstr, maxstr) \
	fieldPtr = mono_class_get_field_from_name(classPtr, fieldName); \
	if(!fieldPtr) \
	{ \
		Q_snprintf(errstr, maxstr, "Can't get field %s", fieldName); \
		return false; \
	}

#define MONO_ATTACH(managedName, methodHandle, asmImage) \
{\
	MonoMethodDesc* desc = mono_method_desc_new(managedName, true); \
	if(!desc) \
	{ \
		Q_snprintf(error, maxlen, "Can't describe method %s", managedName); \
		return false; \
	} \
	methodHandle = mono_method_desc_search_in_image(desc, asmImage); \
	mono_method_desc_free(desc); \
	if(!methodHandle) \
	{ \
		Q_snprintf(error, maxlen, "Can't attach method %s", managedName); \
		return false; \
	} \
};

//#define ENGINE_CALL(func) SH_CALL(engine, &IVEngineServer::func)

edict_t *EdictOfUserId( int UserId );
int UTIL_FindOffset(const char *ClassName, const char *PropertyName);

inline bool FStrEq(const char *sz1, const char *sz2)
{
	return(stricmp(sz1, sz2) == 0);
};

#endif //_COMMON_H_
