#ifndef _MONOPLUG_H
#define _MONOPLUG_H

#include "monoplug_common.h"

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
	MonoObject *m_ClsMain;

	MonoMethod* m_ListPlugins;
	MonoMethod* m_PluginLoad;
public:
	CUtlVector<MonoConCommand*>* m_conCommands;
};

/** 
 * Something like this is needed to register cvars/CON_COMMANDs.
 */
class BaseAccessor : public IConCommandBaseAccessor
{
public:
	bool RegisterConCommandBase(ConCommandBase *pCommandBase)
	{
		/* Always call META_REGCVAR instead of going through the engine. */
		return META_REGCVAR(pCommandBase);
	}
} s_BaseAccessor;

//Native callbacks from Managed
static void Mono_Msg(MonoString* msg)
{
	META_CONPRINT(mono_string_to_utf8(msg));
};

typedef MonoObject CCode;

class MonoConCommand : public ConCommand
{
public:
	MonoConCommand(char* name, char* description, CCode* code);
private:
	void Dispatch( const CCommand &command );
	CCode* m_code;
};

extern CMonoPlug g_MonoPlugPlugin;

static bool Mono_RegisterConCommand(MonoString* name, MonoString* description, CCode* code)
{
	META_CONPRINTF("Entering Mono_RegisterConCommand : %s: %s\n", mono_string_to_utf8(name), mono_string_to_utf8(description));

	MonoConCommand* com = new MonoConCommand(mono_string_to_utf8(name), mono_string_to_utf8(description), code);
	//TODO : add MonoConCommand to native list for handle tracking
	g_MonoPlugPlugin.m_conCommands->AddToTail(com);
		
	//TODO : register MonoConCommand to engine
	g_SMAPI->RegisterConCommandBase(g_PLAPI, com);

	return true;
};

static bool Mono_UnregisterConCommand(MonoString* name)
{
	MonoConCommand* com = NULL;
	const char* s_name = mono_string_to_utf8(name);
	int pos = -1;

	//TODO : get MonoConCommand handle in list
	for(int i = 0; i < 	g_MonoPlugPlugin.m_conCommands->Count(); i++)
	{
		MonoConCommand* item = 	g_MonoPlugPlugin.m_conCommands->Element(i);
		if(strcmp(item->GetName(), s_name) == 0)
		{
			pos = i;
			com = item;
			break;
		}
	}
	
	if(com)
	{
		//TODO : unregister MonoConCommand to engine
		g_SMAPI->UnregisterConCommandBase(g_PLAPI, com);
	
		//TODO : Remove from list
		g_MonoPlugPlugin.m_conCommands->Remove(pos);
		
		//TODO : delete handle
		delete com;

		return true;
	}
	else
	{
		return false;
	}
};

#endif //_MONOPLUG_H

