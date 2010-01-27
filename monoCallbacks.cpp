#include "monoCallbacks.h"
#include "CMonoCommand.h"
#include "tools.h"

namespace MonoPlugin
{
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

	void Mono_AttachConsole()
	{
		if(!g_MonoPlugin.m_console)
		{
			g_MonoPlugin.m_console = new CMonoConsole(&g_MonoPlugin);
			g_pCVar->InstallConsoleDisplayFunc(g_MonoPlugin.m_console);
		}
	};

	void Mono_DetachConsole()
	{
		if(!g_MonoPlugin.m_console)
		{
			g_pCVar->RemoveConsoleDisplayFunc(g_MonoPlugin.m_console);
			delete g_MonoPlugin.m_console;
			g_MonoPlugin.m_console = NULL;
		}
	};

	uint64 Mono_RegisterConCommand(MonoString* name, MonoString* help, int flags, MonoDelegate* code, MonoDelegate* complete)
	{
		char* n_name = mono_string_to_utf8(name);
		ConCommandBase* command = g_pCVar->FindCommandBase(n_name);
		if(command)
			return false;

		uint64 nativeId = g_MonoPlugin.m_nextConbaseId++;
		CMonoCommand* cmd = new CMonoCommand(n_name, mono_string_to_utf8(help), code, flags, complete);
		unsigned int id = g_MonoPlugin.m_conbase->Insert(nativeId, cmd);
		if(id != g_MonoPlugin.m_conbase->InvalidIndex())
		{
			return nativeId;
		}
		else
		{
			delete cmd;
			return 0;
		}
	};

	void Mono_UnregisterConCommand(uint64 nativeId)
	{
		unsigned int i = g_MonoPlugin.m_conbase->Find(nativeId);
		if(g_MonoPlugin.m_conbase->IsValidIndex(i))
		{
			CMonoCommand* com = (CMonoCommand*)g_MonoPlugin.m_conbase->Element(i);
			META_UNREGCVAR(com);
			g_MonoPlugin.m_conbase->Remove(i);
		}
		else
		{
			META_CONPRINTF("Mono_UnregisterConCommand : Command NOT found\n");
		}
	};

	void Mono_RaiseConVarChange(IConVar *var, const char *pOldValue, float flOldValue)
	{
		unsigned int i = g_MonoPlugin.m_conbase->FirstInorder();
		while(g_MonoPlugin.m_conbase->IsValidIndex(i))
		{
			ConCommandBase* item = g_MonoPlugin.m_conbase->Element(i);
			if(!item->IsCommand())
			{
				IConVar* itemVar = (IConVar*)item;
				if(itemVar == var)
				{
					uint64 nativeId = g_MonoPlugin.m_conbase->Key(i);
					void* args[1];
					args[0] = &nativeId;
					CMonoHelpers::CallMethod(g_MonoPlugin.m_main, g_MonoPlugin.m_ClsMain_Raise_ConVarChange, args);
					break;
				}
			}

			i = g_MonoPlugin.m_conbase->NextInorder(i);
		};
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


	MonoString* Mono_Convar_GetString(uint64 nativeID)
	{
		unsigned short i = g_MonoPlugin.m_conbase->Find(nativeID);
		if(g_MonoPlugin.m_conbase->IsValidIndex(i))
		{
			ConVar* var = (ConVar*)g_MonoPlugin.m_conbase->Element(i);
			return CMonoHelpers::GetString(g_Domain, var->GetString());
		}
		else
		{
			return NULL;
		}
	};

	void Mono_Convar_SetString(uint64 nativeID, MonoString* value)
	{
		unsigned short i = g_MonoPlugin.m_conbase->Find(nativeID);
		if(g_MonoPlugin.m_conbase->IsValidIndex(i))
		{
			ConVar* var = (ConVar*)g_MonoPlugin.m_conbase->Element(i);
			char* c_value = mono_string_to_utf8(value);
			var->SetValue(c_value);
		}
	};

	bool Mono_Convar_GetBoolean(uint64 nativeID)
	{
		unsigned short i = g_MonoPlugin.m_conbase->Find(nativeID);
		if(g_MonoPlugin.m_conbase->IsValidIndex(i))
		{
			ConVar* var = (ConVar*)g_MonoPlugin.m_conbase->Element(i);
			return var->GetBool();
		}
		else
		{
			return NULL;
		}
	};

	void Mono_Convar_SetBoolean(uint64 nativeID, bool value)
	{
		unsigned short i = g_MonoPlugin.m_conbase->Find(nativeID);
		if(g_MonoPlugin.m_conbase->IsValidIndex(i))
		{
			ConVar* var = (ConVar*)g_MonoPlugin.m_conbase->Element(i);
			var->SetValue(value);
		}
	};

	uint64 Mono_RegisterConVar(MonoString* name, MonoString* help, int flags, MonoString* defaultValue)
	{
		char* n_name = mono_string_to_utf8(name);
		ConCommandBase* command = g_pCVar->FindCommandBase(n_name);
		if(command)
			return false;

		uint64 nativeId = g_MonoPlugin.m_nextConbaseId++;
		ConVar* var = new ConVar(n_name, mono_string_to_utf8(defaultValue), flags, mono_string_to_utf8(help));
		var->InstallChangeCallback(Mono_RaiseConVarChange);

		unsigned int id = g_MonoPlugin.m_conbase->Insert(nativeId, var);
		if(id != g_MonoPlugin.m_conbase->InvalidIndex())
		{
			return nativeId;
		}
		else
		{
			delete var;
			return 0;
		}
	};

	void Mono_UnregisterConVar(uint64 nativeId)
	{
		unsigned int i = g_MonoPlugin.m_conbase->Find(nativeId);
		if(g_MonoPlugin.m_conbase->IsValidIndex(i))
		{
			ConVar* var = (ConVar*)g_MonoPlugin.m_conbase->Element(i);
			META_UNREGCVAR(var);
			g_MonoPlugin.m_conbase->Remove(i);
		}
		else
		{
			META_CONPRINTF("Mono_UnregisterConVar : ConVar NOT found\n");
		}
	};



	void Mono_EventAttach_LevelShutdown()
	{
	};

	void Mono_EventDetach_LevelShutdown()
	{
	};
}
