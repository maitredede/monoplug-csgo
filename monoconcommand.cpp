#include "monoconcommand.h"

MonoConCommand::MonoConCommand(char* name, char* description, MonoDelegate* code, int flags)
: ConCommand(name, (FnCommandCallback_t)NULL, description, flags, (FnCommandCompletionCallback)NULL)
{
	//META_CONPRINT("Entering : MonoConCommand::MonoConCommand\n");
	this->m_code = code;
};

void MonoConCommand::Dispatch(const CCommand &command)
{
	META_CONPRINT("Entering : MonoConCommand::mono_callback\n");

	void* args[1];
	args[0] = MONO_STRING(g_Domain, command.ArgS());
	META_CONPRINTF("Calling delegate of command %s\n", this->GetName());
	MONO_DELEGATE_CALL(this->m_code, args);
	META_CONPRINTF("Called delegate of command %s\n", this->GetName());
};