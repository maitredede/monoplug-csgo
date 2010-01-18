#ifndef _MONOPLUG_H
#define _MONOPLUG_H

#include <stdio.h>

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

#define MONOPLUG_DLLFILE "%s/addons/MonoPlug.Managed.dll"
#define MONOPLUG_NAMESPACE "MonoPlug"
#define MONOPLUG_CLASSNAME_CLSMAIN "ClsMain"
#define MONOPLUG_CLASSNAME_CLSPLAYER "ClsPlayer"
#define MONOPLUG_FULLCLASSNAME "MonoPlug.ClsMain"

#define MONOPLUG_CALLBACK_MSG "MonoPlug.ClsMain::Mono_Msg"
#define MONOPLUG_CALLBACK_REGISTERCONCOMMAND "MonoPlug.ClsMain::Mono_RegisterConCommand(string,string,MonoPlug.ConCommandDelegate,int,MonoPlug.ConCommandCompleteDelegate)"
#define MONOPLUG_CALLBACK_UNREGISTERCONCOMMAND "MonoPlug.ClsMain::Mono_UnregisterConCommand(string)"
#define MONOPLUG_CALLBACK_REGISTERCONVARSTRING "MonoPlug.ClsMain::Mono_RegisterConVarString(string,string,int,string)"
#define MONOPLUG_CALLBACK_UNREGISTERCONVARSTRING "MonoPlug.ClsMain::Mono_UnregisterConVarString(ulong)"
#define MONOPLUG_CALLBACK_CONVARSTRING_GETVALUE "MonoPlug.ClsMain::Mono_GetConVarStringValue(ulong)"
#define MONOPLUG_CALLBACK_CONVARSTRING_SETVALUE "MonoPlug.ClsMain::Mono_SetConVarStringValue(ulong,string)"
#define MONOPLUG_CALLBACK_CONVAR_GETVALUE_STRING "MonoPlug.ClsMain::Mono_Convar_GetValue_String(string)"

#define MONOPLUG_NATMAN_INIT "MonoPlug.ClsMain:_Init()"
#define MONOPLUG_NATMAN_SHUTDOWN "MonoPlug.ClsMain:_Shutdown()"
#define MONOPLUG_NATMAN_CONVARSTRING_VALUECHANGED "MonoPlug.ClsMain:_ConVarStringChanged(ulong)"

#define MONOPLUG_NATMAN_GAMEFRAME "MonoPlug.ClsMain:EVT_GameFrame()"
#define MONOPLUG_CLSMAIN_EVT_LEVELINIT "MonoPlug.ClsMain:EVT_LevelInit(string,string,string,string,bool,bool)"

#if defined WIN32 && !defined snprintf
#define snprintf _snprintf
#endif


PLUGIN_GLOBALVARS();

class MonoConCommand : public ConCommand
{
public:
	MonoConCommand(char* name, char* description, MonoDelegate* code, int flags, MonoDelegate* complete = NULL);
	int AutoCompleteSuggest( const char *partial, CUtlVector< CUtlString > &commands );
	bool CanAutoComplete( void );
	void Dispatch( const CCommand &command );
private:
	MonoDelegate* m_code;
	MonoDelegate* m_complete;
};

class uint64Container
{
private:
	uint64 m_value;
public: 
	void SetValue(uint64 value){ this->m_value = value; } ;
	uint64 Value() { return this->m_value; } ;
	uint64Container(uint64 value) { this->SetValue(value); } ;

};

class CMonoPlug: public ISmmPlugin, public IMetamodListener, public IGameEventListener2
{
public:
	bool Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late);
	bool Unload(char *error, size_t maxlen);
public: //IMetamodListener stuff
	void OnVSPListening(IServerPluginCallbacks *iface);
public:
	const char *GetAuthor();
	const char *GetName();
	const char *GetDescription();
	const char *GetURL();
	const char *GetLicense();
	const char *GetVersion();
	const char *GetDate();
	const char *GetLogTag();
public: //Hooks
	void Hook_GameFrame(bool simulating);
public: //IGameEventListener2
	// game event listener
	void FireGameEvent( IGameEvent *event );
public:
	MonoObject* m_ClsMain;

	CUtlVector<MonoConCommand*>* m_conCommands;

	uint64 m_convarStringIdValue;
	CUtlVector<ConVar*>* m_convarStringPtr;
	CUtlVector<uint64Container*>* m_convarStringId;
	
	ConCommand* m_pSayCmd;
	ConCommand* m_pSayTeamCmd;
};


static void Mono_Msg(MonoString* msg)
{
	META_CONPRINT(mono_string_to_utf8(msg));
};


extern CMonoPlug g_MonoPlugPlugin;
extern MonoDomain* g_Domain;
extern MonoImage* g_Image;
extern MonoMethod* g_EVT_ConVarStringValueChanged;

PLUGIN_GLOBALVARS();

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
 
static MonoObject* MONO_CALL_ARGS(void* target, MonoMethod* methodHandle, void** args);
static MonoObject* MONO_DELEGATE_CALL(MonoDelegate* delegateObject, void** args);

#define MONO_CALL(target, methodHandle) MONO_CALL_ARGS(target, methodHandle, NULL)
#define MONO_STRING(domain, str) ((str == NULL) ? NULL : mono_string_new(domain, str))

#include "mono_event_hooks.h"

#define MONO_GET_CLASS(assemblyImage, classPtr, classNamespace, className, errstr, maxstr) \
	classPtr = mono_class_from_name(assemblyImage, classNamespace, className); \
	if(!classPtr) \
	{ \
		Q_snprintf(errstr, maxstr, "Can't get type %s", className); \
		META_CONPRINTF("%s\n", errstr); \
		return false; \
	}

#define MONO_SET_FIELD(classPtr, fieldName, object, value) mono_field_set_value(object, mono_class_get_field_from_name(classPtr, fieldName), value);

#endif //_MONOPLUG_H
