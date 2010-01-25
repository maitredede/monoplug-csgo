#include "funcs.h"

bool LessFunc_uint64(uint64 const& k1, uint64 const& k2)
{
	return k1 < k2;
}

void Mono_Msg(MonoString* msg)
{
	META_CONPRINT(mono_string_to_utf8(msg));
};

void Mono_Log(MonoString* msg)
{
	META_LOG(g_PLAPI, mono_string_to_utf8(msg));
};

void Mono_DevMsg(MonoString* msg)
{
	DevMsg(mono_string_to_utf8(msg));
};

void Mono_Warning(MonoString* msg)
{
	Warning(mono_string_to_utf8(msg));
};

void Mono_Error(MonoString* msg)
{
	Error(mono_string_to_utf8(msg));
};

uint64 Mono_RegisterConvar(MonoString* name, MonoString* help, int flags, MonoString* defaultValue, MonoDelegate* changeCallback)
{
	char* n_name = mono_string_to_utf8(name);
	{
		ConVar* var = g_pCVar->FindVar(n_name);
		if(var)
		{
			return 0;
		}
	}
	{
		uint64 nativeID = ++g_MonoPlug.m_convarNextId;
		CMonoConvar* mvar = new CMonoConvar(nativeID, n_name, mono_string_to_utf8(defaultValue), flags, mono_string_to_utf8(help), changeCallback);
		//var = new ConVar(n_name, mono_string_to_utf8(defaultValue), flags, mono_string_to_utf8(help));
		mvar->InstallChangeCallback(Mono_CMonoConvar_ValueChanged);
		if(META_REGCVAR(mvar))
		{
			g_MonoPlug.m_convars->Insert(nativeID, mvar);
			return nativeID;
		}
		else
		{
			delete mvar;
			return 0;
		}
	}
};

bool Mono_UnregisterConvar(uint64 nativeID)
{
	//ConVar* var = NULL;

	unsigned short i = g_MonoPlug.m_convars->Find(nativeID);
	if(i != g_MonoPlug.m_convars->InvalidIndex())
	{
		CMonoConvar* mvar = g_MonoPlug.m_convars->Element(i);
		g_MonoPlug.m_convars->Remove(i);
		META_UNREGCVAR(mvar);
		delete mvar;
		return true;
	}
	return false;

	//while(i != g_MonoPlug.m_convars->InvalidIndex())
	//{
	//	if(g_MonoPlug.m_convars->Element(i) == nativeID)
	//	{
	//		var = g_MonoPlug.m_convars->Key(i);
	//		break;
	//	}
	//}

	//if(var)
	//{
	//	META_UNREGCVAR(var);
	//	return true;
	//}
	//else
	//{
	//	return false;
	//}

};

void Mono_CMonoConvar_ValueChanged(IConVar *var, const char *pOldValue, float flOldValue)
{
	DevMsg("Native: Mono_ConvarValueChanged %s\n", "Enter...");
	CMonoConvar* v = (CMonoConvar*)var;
	v->Changed(var, pOldValue, flOldValue);
	DevMsg("Native: Mono_ConvarValueChanged %s\n", "Exit...");
};

MonoString* Mono_Convar_GetString(uint64 nativeID)
{
	unsigned short i = g_MonoPlug.m_convars->Find(nativeID);
	if(i != g_MonoPlug.m_convars->InvalidIndex())
	{
		CMonoConvar* var = g_MonoPlug.m_convars->Element(i);
		return CMonoHelpers::MONO_STRING(g_Domain, var->GetString());
	}
	else
	{
		return NULL;
	}

	//do
	//{
	//	if(g_MonoPlug.m_convars->Element(i) == nativeID)
	//	{
	//		ConVar* var = g_MonoPlug.m_convars->Key(i);
	//		return CMonoHelpers::MONO_STRING(g_Domain, var->GetString());
	//	}
	//}
	//while((i=g_MonoPlug.m_convars->NextInorder(i))!=g_MonoPlug.m_convars->InvalidIndex());
	//return NULL;
};

void Mono_Convar_SetString(uint64 nativeID, MonoString* value)
{
	unsigned short i = g_MonoPlug.m_convars->Find(nativeID);
	if(i != g_MonoPlug.m_convars->InvalidIndex())
	{
		CMonoConvar* var = g_MonoPlug.m_convars->Element(i);
		char* c_value = mono_string_to_utf8(value);
		var->SetValue(c_value);
	}
};

bool Mono_Convar_GetBoolean(uint64 nativeID)
{
	unsigned short i = g_MonoPlug.m_convars->Find(nativeID);
	if(i != g_MonoPlug.m_convars->InvalidIndex())
	{
			CMonoConvar* var = g_MonoPlug.m_convars->Element(i);
			bool ret = var->GetBool();
			return ret;
	}
	return FALSE;
};

void Mono_Convar_SetBoolean(uint64 nativeID, bool value)
{
	unsigned short i = g_MonoPlug.m_convars->Find(nativeID);
	if(i != g_MonoPlug.m_convars->InvalidIndex())
	{
			CMonoConvar* var = g_MonoPlug.m_convars->Element(i);
			return var->SetValue(value);
	}
};

bool Mono_RegisterConCommand(MonoString* name, MonoString* help, MonoDelegate* code, int flags, MonoDelegate* complete)
{
	char* n_name = mono_string_to_utf8(name);
	ConCommand* command = g_pCVar->FindCommand(n_name);
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

void Attach_ConMessage()
{
	g_pCVar->InstallConsoleDisplayFunc(g_MonoPlug.m_console);
};

void Detach_ConMessage()
{
	g_pCVar->RemoveConsoleDisplayFunc(g_MonoPlug.m_console);
};

void Mono_ClientDialogMessage(int client, MonoString* title, MonoString* message, int a, int r, int g, int b, int level, int time)
{
	edict_t* pEntity = EdictOfUserId(client);
	if(pEntity)
	{
		KeyValues *kv = new KeyValues( "msg" );
		kv->SetString( "title", mono_string_to_utf8(title) );
		kv->SetString( "msg", mono_string_to_utf8(message) );
		kv->SetColor( "color", Color( r, g, b, a ));
		kv->SetInt( "level", level);
		kv->SetInt( "time", time);

		g_helpers->CreateMessage(pEntity, DIALOG_MSG, kv, g_vsp_callbacks);
		kv->deleteThis();
	}
};
