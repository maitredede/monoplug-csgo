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
	MonoMethod* m_EVT_ConVarStringValueChanged;

	CUtlVector<MonoConCommand*>* m_conCommands;

	uint64 m_convarStringIdValue;
	CUtlVector<ConVar*>* m_convarStringPtr;
	CUtlVector<uint64Container*>* m_convarStringId;
	//CUtlVector<MonoConVarString*>* m_conVarString;
};

//class MonoConVarString : public ConVar
//{
//public:
//	MonoConVarString(uint64 nativeId, char* name, char* description, int flags, char* defaultValue);
//public: //properties
//	uint64 NativeId() { return this->m_nativeId; };
//	const char* GetValueString() { return this->GetString(); } ;
//private:
//	uint64 m_nativeId;
//};

static void ConVarStringChangeCallback(IConVar *var, const char *pOldValue, float flOldValue)
{
	//MonoConVarString* strVar = (MonoConVarString*)var;
	//gpointer args[1];
	//uint64 val = strVar->NativeId(); 
	//args[0] = &(val);
	//MONO_CALL_ARGS(g_MonoPlugPlugin.m_ClsMain, g_MonoPlugPlugin.m_EVT_ConVarStringValueChanged, args);

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

#endif //_MONOPLUG_H

