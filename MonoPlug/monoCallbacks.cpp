#include "monoCallbacks.h"

void Mono_Msg(MonoString* msg)
{
	META_CONPRINT(mono_string_to_utf8(msg));
};

void Mono_Log(MonoString* msg)
{
	META_LOG(g_PLAPI, mono_string_to_utf8(msg));
};

uint64 Mono_RegisterConvar(MonoString* name, MonoString* help, int flags, MonoString* defaultValue)
{
	char* n_name = mono_string_to_utf8(name);
	ConVar* var = g_MonoPlug.m_icvar->FindVar(n_name);
	if(var)
	{
		return 0;
	}
	else
	{
		uint64 nativeID = ++g_MonoPlug.m_convarNextId;
		var = new ConVar(
			n_name,
			mono_string_to_utf8(defaultValue),
			flags,
			mono_string_to_utf8(help),
			Mono_ConvarValueChanged
			);
		g_MonoPlug.m_convars->Insert(var, nativeID);
		META_REGCVAR(var);
		return nativeID;
	}
};

bool Mono_UnregisterConvar(uint64 nativeID)
{
	ConVar* var = NULL;

	unsigned short i = g_MonoPlug.m_convars->FirstInorder();
	while(i != g_MonoPlug.m_convars->InvalidIndex())
	{
		if(g_MonoPlug.m_convars->Element(i) == nativeID)
		{
			var = g_MonoPlug.m_convars->Key(i);
			break;
		}
	}

	if(var)
	{
		META_UNREGCVAR(var);
		return true;
	}
	else
	{
		return false;
	}

};

void Mono_ConvarValueChanged(IConVar *var, const char *pOldValue, float flOldValue)
{
	ConVar* v = (ConVar*)var;
	unsigned short i = g_MonoPlug.m_convars->Find(v);

	if(i!=g_MonoPlug.m_convars->InvalidIndex())
	{
		//uint64Container* value = g_MonoPlugPlugin.m_convarStringId->Element(i);
		uint64 nativeID = g_MonoPlug.m_convars->Element(i);

		gpointer args[1];
		args[0] = &nativeID;
		CMonoHelpers::MONO_CALL(g_MonoPlug.m_main, g_MonoPlug.m_ClsMain_ConvarChanged, args);
	}
};

MonoString* Mono_Convar_GetString(uint64 nativeID)
{
	ConVar* var = NULL;

	unsigned short i = g_MonoPlug.m_convars->FirstInorder();
	while(i != g_MonoPlug.m_convars->InvalidIndex())
	{
		if(g_MonoPlug.m_convars->Element(i) == nativeID)
		{
			var = g_MonoPlug.m_convars->Key(i);
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
};

void Mono_Convar_SetString(uint64 nativeID, MonoString* value)
{
	ConVar* var = NULL;

	unsigned short i = g_MonoPlug.m_convars->FirstInorder();
	while(i != g_MonoPlug.m_convars->InvalidIndex())
	{
		if(g_MonoPlug.m_convars->Element(i) == nativeID)
		{
			var = g_MonoPlug.m_convars->Key(i);
			break;
		}
	}

	if(var)
	{
		var->SetValue(mono_string_to_utf8(value));
	}
};

bool Mono_RegisterConCommand(MonoString* name, MonoString* help, MonoDelegate* code, int flags, MonoDelegate* complete)
{
	char* n_name = mono_string_to_utf8(name);
	ConCommand* command = g_MonoPlug.m_icvar->FindCommand(n_name);
	if(command)
	{
		return false;
	}
	else
	{
		CMonoCommand* CONCMD_VARNAME(com) = new CMonoCommand(n_name, mono_string_to_utf8(help), code, flags, complete);
		if(META_REGCMD(com))
		{
			g_MonoPlug.m_commands->AddToTail(CONCMD_VARNAME(com));
			return true;
		}
		else
		{
			delete CONCMD_VARNAME(com);
			return false;
		}
	}
};

bool Mono_UnregisterConCommand(MonoString* name)
{
	const char* s_name = mono_string_to_utf8(name);

	for(int i = 0; i < 	g_MonoPlug.m_commands->Count(); i++)
	{
		CMonoCommand* CONCMD_VARNAME(item) = g_MonoPlug.m_commands->Element(i);

		if(Q_strcmp(CONCMD_VARNAME(item)->GetName(), s_name) == 0)
		{
			META_UNREGCMD(item);
			g_MonoPlug.m_commands->Remove(i);
			delete CONCMD_VARNAME(item);
			return true;
		}
	}

	META_CONPRINTF("Mono_UnregisterConCommand : Command NOT found\n");
	return false;
};

MonoArray* Mono_GetPlayers()
{
	CUtlVector<MonoObject*>* lst = new CUtlVector<MonoObject*>();

	//Search for players
	for(mono_array_size_t i = 0; i < g_MonoPlug.m_EdictCount ; i++)
	{
		MonoObject* player = mono_array_get(g_MonoPlug.m_players, MonoObject*, i);
		if(player)
		{
			lst->AddToTail(player);
		}
	}

	//Convert native vector to managed list
	MonoArray* arr = mono_array_new(g_Domain, g_MonoPlug.m_Class_ClsPlayer, lst->Count());
	for(int i=0; i<lst->Count(); i++)
	{
		mono_array_set(arr, MonoObject*, i, lst->Element(i));
	}

	delete lst;

	return arr;
};

void Attach_ConMessage()
{
	g_MonoPlug.m_icvar->InstallConsoleDisplayFunc(g_MonoPlug.m_console);
};

void Detach_ConMessage()
{
	g_MonoPlug.m_icvar->RemoveConsoleDisplayFunc(g_MonoPlug.m_console);
};