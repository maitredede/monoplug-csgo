#include "ManagedCommand.h"

ManagedCommand::ManagedCommand(int id, const char* cmd, const char* description, int flags)
{
	this->m_id = id;

	FnCommandCallback_t nativeCallback = &ManagedCommand::NativeCallback;

	this->m_nativeCommand = new ConCommand(cmd, nativeCallback, description, flags);
}

ManagedCommand::~ManagedCommand(){
	delete this->m_nativeCommand;
}

void ManagedCommand::NativeCallback(const CCommand &command){
	g_Managed.RaiseCommand(command.ArgC(), command.ArgV());
}

ConCommand* ManagedCommand::GetNativeCommand()
{
	return this->m_nativeCommand;
}

int ManagedCommand::GetId()
{
	return this->m_id;
}