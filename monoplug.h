#ifndef _MONOPLUG_H
#define _MONOPLUG_H

#include "monoplug_common.h"
#include "monoconcommand.h"

class CMonoPlug: public ISmmPlugin
{
public:
	bool Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late);
	bool Unload(char *error, size_t maxlen);
	//bool Pause(char *error, size_t maxlen);
	//bool Unpause(char *error, size_t maxlen);
	//void AllPluginsLoaded();
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
	MonoMethod* g_EVT_ConVarStringValueChanged;

	CUtlVector<MonoConCommand*>* m_conCommands;

	uint64 m_conVarStringId;
	CUtlVector<MonoConVarString*>* m_conVarString;
};

class BaseAccessor : public IConCommandBaseAccessor
{
public:
	bool RegisterConCommandBase(ConCommandBase *pCommandBase);
	//{
	//	/* Always call META_REGCVAR instead of going through the engine. */
	//	return META_REGCVAR(pCommandBase);
	//}
};

class MonoConVarString : public ConVar
{
public:
	MonoConVarString(uint64 nativeId, char* name, char* description, int flags, char* defaultValue);
public: //properties
	uint64 NativeId() { return this->m_nativeId; };
private:
	uint64 m_nativeId;
};

//Native callbacks from Managed
static void Mono_Msg(MonoString* msg)
{
	META_CONPRINT(mono_string_to_utf8(msg));
};

static bool Mono_RegisterConCommand(MonoString* name, MonoString* description, MonoDelegate* code, int flags)
{
	META_CONPRINTF("Entering Mono_RegisterConCommand : %s: %s\n", mono_string_to_utf8(name), mono_string_to_utf8(description));

	MonoConCommand* com = new MonoConCommand(mono_string_to_utf8(name), mono_string_to_utf8(description), code, flags);
	//TODO : add MonoConCommand to native list for handle tracking
	g_MonoPlugPlugin.m_conCommands->AddToTail(com);
		
	//TODO : register MonoConCommand to engine
	g_SMAPI->RegisterConCommandBase(g_PLAPI, com);

	return true;
};

static bool Mono_UnregisterConCommand(MonoString* name)
{
	META_CONPRINTF("Entering Mono_UnregisterConCommand : %s\n", mono_string_to_utf8(name));
	MonoConCommand* com = NULL;
	const char* s_name = mono_string_to_utf8(name);

	//TODO : get MonoConCommand handle in list
	if(!g_MonoPlugPlugin.m_conCommands)
	{
		META_CONPRINT("g_MonoPlugin.m_ConCommands IS NULL\n");
		return false;
	}
	META_CONPRINTF("g_MonoPlugPlugin.m_conCommands->Count() = %d\n", g_MonoPlugPlugin.m_conCommands->Count());

	META_CONPRINTF("item count : %d\n", g_MonoPlugPlugin.m_conCommands->Count());
	for(int i = 0; i < 	g_MonoPlugPlugin.m_conCommands->Count(); i++)
	{
		MonoConCommand* item = 	g_MonoPlugPlugin.m_conCommands->Element(i);
		if(!item)
		{
			META_CONPRINT("item IS NULL\n");
			return false;
		}
		META_CONPRINTF("item[%d] item->GetName() => %s ; s_name = %s\n", i, item->GetName(), s_name);
		if(Q_strcmp(item->GetName(), s_name) == 0)
		{
			META_CONPRINTF("item[%d] Found\n", i);
			com = item;
			//break;
		}
	}

	META_CONPRINTF("com=[%d]\n", com);

	if(NULL == com)
	{
		META_CONPRINTF("Mono_UnregisterConCommand : Command NOT found\n");
		return false;
	}
	else
	{
		META_CONPRINTF("Mono_UnregisterConCommand : Command found -> Unregister\n");
		//TODO : unregister MonoConCommand to engine
		g_SMAPI->UnregisterConCommandBase(g_PLAPI, com);
	
		META_CONPRINTF("Mono_UnregisterConCommand : Command unregistered -> Removing from list\n");

		//TODO : Remove from list
		g_MonoPlugPlugin.m_conCommands->FindAndRemove(com);
		
		META_CONPRINTF("Mono_UnregisterConCommand : Command removed -> delete\n");

		//TODO : delete handle
		delete com;

		META_CONPRINTF("Mono_UnregisterConCommand : Command delete -> OK\n");
		return true;
	}
};

static uint64 Mono_RegisterConVarString(MonoString* name, MonoString* description, int flags, MonoString* defaultValue)
{
#ifdef _DEBUG
	META_CONPRINTF("Entering Mono_RegisterConVarString : %s: %s\n", mono_string_to_utf8(name), mono_string_to_utf8(description));
#endif

	MonoConVarString* var = new MonoConVarString(++g_MonoPlugPlugin.m_conVarStringId, mono_string_to_utf8(name), mono_string_to_utf8(description), flags, mono_string_to_utf8(defaultValue));
	if(g_SMAPI->RegisterConCommandBase(g_PLAPI, var))
	{
		g_MonoPlugPlugin.m_conVarString->AddToTail(var);
		return var->NativeId();
	}
	return true;
}

static MonoString* Mono_GetConVarStringValue(uint64 nativeId)
{
#ifdef _DEBUG
	META_CONPRINT("Entering Mono_GetConVarStringValue\n");
#endif
	MonoConVarString* var = NULL;
	for(int i=0;i<g_MonoPlugPlugin.m_conVarString->Count(); i++)
	{
		MonoConVarString* elem = g_MonoPlugPlugin.m_conVarString->Element(i);
		if(elem->NativeId() == nativeId)
		{
			var = elem;
			break;
		}
	}

	if(var)
	{
		return MONO_STRING(g_Domain, var->GetString());
	}
	else
	{
		return NULL;
	}
}

static void Mono_SetConVarStringValue(uint64 nativeId, MonoString* value)
{
#ifdef _DEBUG
	META_CONPRINT("Entering Mono_SetConVarStringValue\n");
#endif

	MonoConVarString* var = NULL;
	for(int i=0;i<g_MonoPlugPlugin.m_conVarString->Count(); i++)
	{
		MonoConVarString* elem = g_MonoPlugPlugin.m_conVarString->Element(i);
		if(elem->NativeId() == nativeId)
		{
			var = elem;
			break;
		}
	}

	if(var)
	{
		var->SetValue(mono_string_to_utf8(value));
	}
}

static void ConVarStringChangeCallback(IConVar *var, const char *pOldValue, float flOldValue)
{
#ifdef _DEBUG
	META_CONPRINT("Entering ConVarStringChangeCallback\n");
#endif

	MonoConVarString* strVar = (MonoConVarString*)var;
	gpointer args[1];
	uint64 val = strVar->NativeId(); 
	args[0] = &(val);
	MONO_CALL_ARGS(g_MonoPlugPlugin.m_ClsMain, g_MonoPlugPlugin.g_EVT_ConVarStringValueChanged, args);
}

#endif //_MONOPLUG_H

