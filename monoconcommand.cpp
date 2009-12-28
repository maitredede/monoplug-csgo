#include "monoconcommand.h"

MonoConCommand::MonoConCommand(char* name, char* description, MonoDelegate* code, int flags)
: ConCommand(name, (FnCommandCallback_t)NULL, description, flags, (FnCommandCompletionCallback)NULL)
{
	this->m_code = code;
};

void MonoConCommand::Dispatch(const CCommand &command)
{
	void* args[1];
	args[0] = MONO_STRING(g_Domain, command.ArgS());
	MONO_DELEGATE_CALL(this->m_code, args);
};

