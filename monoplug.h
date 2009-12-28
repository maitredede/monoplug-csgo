#ifndef _MONOPLUG_H
#define _MONOPLUG_H

#include "monoplug_common.h"
#include "monoconcommand.h"
#include "BaseAccessor.h"

class CMonoPlug: public ISmmPlugin
{
public:
	bool Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late);
	bool Unload(char *error, size_t maxlen);
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
	MonoMethod* m_EVT_ConVarStringValueChanged;

	CUtlVector<MonoConCommand*>* m_conCommands;

	uint64 m_convarStringIdValue;
	CUtlVector<ConVar*>* m_convarStringPtr;
	CUtlVector<uint64Container*>* m_convarStringId;
};

static void ConVarStringChangeCallback(IConVar *var, const char *pOldValue, float flOldValue)
{
	ConVar* v = (ConVar*)var;
	int i = g_MonoPlugPlugin.m_convarStringPtr->Find(v);
	if(i>=0)
	{
		uint64Container* value = g_MonoPlugPlugin.m_convarStringId->Element(i);

		gpointer args[1];
		uint64 val = value->Value();
		args[0] = &val;
		MONO_CALL_ARGS(g_MonoPlugPlugin.m_ClsMain, g_MonoPlugPlugin.m_EVT_ConVarStringValueChanged, args);
	}
};


static bool Mono_RegisterConCommand(MonoString* name, MonoString* description, MonoDelegate* code, int flags)
{
	MonoConCommand* com = new MonoConCommand(mono_string_to_utf8(name), mono_string_to_utf8(description), code, flags);
	if(g_SMAPI->RegisterConCommandBase(g_PLAPI, com))
	{
		g_MonoPlugPlugin.m_conCommands->AddToTail(com);
		return true;
	}
	else
	{
		delete com;
		return false;
	}
};

static bool Mono_UnregisterConCommand(MonoString* name)
{
	const char* s_name = mono_string_to_utf8(name);

	for(int i = 0; i < 	g_MonoPlugPlugin.m_conCommands->Count(); i++)
	{
		MonoConCommand* item = 	g_MonoPlugPlugin.m_conCommands->Element(i);

		if(Q_strcmp(item->GetName(), s_name) == 0)
		{
			g_SMAPI->UnregisterConCommandBase(g_PLAPI, item);
			g_MonoPlugPlugin.m_conCommands->Remove(i);
			delete item;
			return true;
		}
	}

	META_CONPRINTF("Mono_UnregisterConCommand : Command NOT found\n");
	return false;
};

static uint64 Mono_RegisterConVarString(MonoString* name, MonoString* description, int flags, MonoString* defaultValue)
{
	uint64 nativeID = ++g_MonoPlugPlugin.m_convarStringIdValue;
	ConVar* var = new ConVar(mono_string_to_utf8(name), mono_string_to_utf8(defaultValue), flags, mono_string_to_utf8(description), (FnChangeCallback_t)ConVarStringChangeCallback);
	if(g_SMAPI->RegisterConCommandBase(g_PLAPI, var))
	{
		g_MonoPlugPlugin.m_convarStringId->AddToTail(new uint64Container(nativeID));
		g_MonoPlugPlugin.m_convarStringPtr->AddToTail(var);
		return nativeID;
	}
	else
	{
		META_CONPRINTF("Mono_RegisterConVarString var '%s' NOT registered\n", mono_string_to_utf8(name));
		return 0;
	}
};

static void Mono_UnregisterConVarString(uint64 nativeID)
{
	for(int i=0;i<g_MonoPlugPlugin.m_convarStringId->Count(); i++)
	{
		uint64Container* cont = g_MonoPlugPlugin.m_convarStringId->Element(i);
		if(cont->Value() == nativeID)
		{
			ConVar* var = g_MonoPlugPlugin.m_convarStringPtr->Element(i);
			g_SMAPI->UnregisterConCommandBase(g_PLAPI, var);
			g_MonoPlugPlugin.m_convarStringId->Remove(i);
			g_MonoPlugPlugin.m_convarStringPtr->Remove(i);
			delete var;
			break;
		}
	}

};

static MonoString* Mono_GetConVarStringValue(uint64 nativeID)
{
#ifdef _DEBUG
	META_CONPRINTF("Entering Mono_GetConVarStringValue : %l\n", nativeID);
#endif
	for(int i=0;i<g_MonoPlugPlugin.m_convarStringId->Count(); i++)
	{
		uint64Container* cont = g_MonoPlugPlugin.m_convarStringId->Element(i);
		if(cont->Value() == nativeID)
		{
			ConVar* var = g_MonoPlugPlugin.m_convarStringPtr->Element(i);

			return MONO_STRING(g_Domain, var->GetString());
		}
	}
	return NULL;
};

static void Mono_SetConVarStringValue(uint64 nativeID, MonoString* value)
{
#ifdef _DEBUG
	META_CONPRINT("Entering Mono_SetConVarStringValue\n");
#endif

	for(int i=0;i<g_MonoPlugPlugin.m_convarStringId->Count(); i++)
	{
		uint64Container* cont = g_MonoPlugPlugin.m_convarStringId->Element(i);
		if(cont->Value() == nativeID)
		{
			ConVar* var = g_MonoPlugPlugin.m_convarStringPtr->Element(i);

			var->SetValue(mono_string_to_utf8(value));
			break;
		}
	}
};

#endif //_MONOPLUG_H
