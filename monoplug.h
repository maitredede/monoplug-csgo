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
#define MONOPLUG_CLASSNAME "ClsMain"
#define MONOPLUG_FULLCLASSNAME "MonoPlug.ClsMain"

#define MONOPLUG_CALLBACK_MSG "MonoPlug.ClsMain::Mono_Msg"

#define MONOPLUG_NATMAN_INIT "MonoPlug.ClsMain:_Init()"
#define MONOPLUG_NATMAN_SHUTDOWN "MonoPlug.ClsMain:_Shutdown()"
//#define MONOPLUG_NATMAN_HANDLEMESSAGE "MonoPlug.ClsMain:_HandleMessages()"
#define MONOPLUG_CLSMAIN_EVT_GAMEFRAME "MonoPlug.ClsMain:EVT_GameFrame()"

#define MONOPLUG_CALLBACK_REGISTERCONCOMMAND "MonoPlug.ClsMain::Mono_RegisterConCommand(string,string,MonoPlug.ConCommandDelegate,int)"
#define MONOPLUG_CALLBACK_UNREGISTERCONCOMMAND "MonoPlug.ClsMain::Mono_UnregisterConCommand(string)"

#define MONOPLUG_CALLBACK_REGISTERCONVARSTRING "MonoPlug.ClsMain::Mono_RegisterConVarString(string,string,int,string)"
#define MONOPLUG_CALLBACK_UNREGISTERCONVARSTRING "MonoPlug.ClsMain::Mono_UnregisterConVarString(ulong)"
#define MONOPLUG_CALLBACK_CONVARSTRING_GETVALUE "MonoPlug.ClsMain::Mono_GetConVarStringValue(ulong)"
#define MONOPLUG_CALLBACK_CONVARSTRING_SETVALUE "MonoPlug.ClsMain::Mono_SetConVarStringValue(ulong,string)"
#define MONOPLUG_NATMAN_CONVARSTRING_VALUECHANGED "MonoPlug.ClsMain:_ConVarStringChanged(ulong)"

#define MONOPLUG_CLSMAIN_EVT_LEVELINIT "MonoPlug.ClsMain:EVT_LevelInit(string,string,string,string,bool,bool)"
#define MONOPLUG_CLSMAIN_EVT_LEVELSHUTDOWN "MonoPlug.ClsMain:EVT_LevelShutdown()"

#if defined WIN32 && !defined snprintf
#define snprintf _snprintf
#endif


PLUGIN_GLOBALVARS();

class MonoConCommand : public ConCommand
{
public:
	MonoConCommand(char* name, char* description, MonoDelegate* code, int flags);
private:
	void Dispatch( const CCommand &command );
	MonoDelegate* m_code;
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

class CMonoPlug: public ISmmPlugin, public IMetamodListener
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
	void Hook_ServerActivate(edict_t *pEdictList, int edictCount, int clientMax);
	void Hook_GameFrame(bool simulating);
	bool Hook_LevelInit(const char *pMapName,
		char const *pMapEntities,
		char const *pOldLevel,
		char const *pLandmarkName,
		bool loadGame,
		bool background);
	void Hook_LevelShutdown(void);
private:
	MonoMethod* m_ListPlugins;
	MonoMethod* m_PluginLoad;
public:
	MonoObject* m_ClsMain;

	CUtlVector<MonoConCommand*>* m_conCommands;

	uint64 m_convarStringIdValue;
	CUtlVector<ConVar*>* m_convarStringPtr;
	CUtlVector<uint64Container*>* m_convarStringId;
};


static void Mono_Msg(MonoString* msg)
{
	META_CONPRINT(mono_string_to_utf8(msg));
};


extern CMonoPlug g_MonoPlugPlugin;
extern MonoDomain* g_Domain;
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

#endif //_MONOPLUG_H
