#include "CMonoCommand.h"

bool LessFunc_CMonoCommand(ConVar* const& key1, ConVar* const& key2)
{
	return (strcmpi(key1->GetName(), key2->GetName()) < 0);
};

CMonoCommand::CMonoCommand(char* name, char* description, MonoDelegate* code, int flags,MonoDelegate* complete)
: ConCommand(name, (FnCommandCallback_t)NULL, description, flags, (FnCommandCompletionCallback)NULL)
{
	this->m_code = code;
	this->m_complete = complete;
};

void CMonoCommand::Dispatch(const CCommand &command)
{
	MonoArray* arr = mono_array_new(g_Domain, mono_get_string_class(), command.ArgC());
	mono_array_size_t max = (mono_array_size_t)command.ArgC();
	for(mono_array_size_t i = 0; i<max; i++)
	{
		MonoString* str = CMonoHelpers::MONO_STRING(g_Domain, command.ArgV()[i]);
		mono_array_set(arr, MonoString*, i, str);
	}

	void* args[2]; 
	args[0] = CMonoHelpers::MONO_STRING(g_Domain, command.ArgS());
	args[1] = arr;

	//META_CONPRINTF("Dispatch command BEGIN : %s\n", command.GetCommandString());
	CMonoHelpers::MONO_DELEGATE_CALL(this->m_code, args);
	//META_CONPRINTF("Dispatch command END : %s\n", command.GetCommandString());
};

int CMonoCommand::AutoCompleteSuggest( const char *partial, CUtlVector< CUtlString > &commands )
{
	void* args[1];
	args[0] = CMonoHelpers::MONO_STRING(g_Domain, partial);
	MonoObject* ret = CMonoHelpers::MONO_DELEGATE_CALL(this->m_complete, args);
	if(ret)
	{
		MonoArray* arr = (MonoArray*)ret;
		for(uint i=0;i<mono_array_length(arr);i++)
		{
			MonoString* str = mono_array_get(arr, MonoString*, i);
			commands.AddToTail(CUtlString(mono_string_to_utf8(str)));
		}
		return mono_array_length(arr);
	}
	else
	{
		return 0;
	}
};

bool CMonoCommand::CanAutoComplete()
{
	if(this->m_complete)
	{
		return true;
	}
	else
	{
		return false;
	}
};
