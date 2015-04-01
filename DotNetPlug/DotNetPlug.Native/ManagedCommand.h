#ifndef _DOTNETPLUG_MANAGED_COMMAND_H_
#define _DOTNETPLUG_MANAGED_COMMAND_H_

#include <convar.h>
#include "Plugin.h"
#include "Managed.h"
#include "Types.h"

#ifdef MANAGED_WIN32
#include <comutil.h>
#include <stdio.h>
#endif


class ManagedCommand
{
public:
	ManagedCommand(int id, const char* cmd, const char* description, int flags);
	ConCommand* GetNativeCommand();
	int GetId();
private:
	int m_id;
	ConCommand* m_nativeCommand;
	//void InvokeCallback(int argc, const char** argv);

	static void NativeCallback(const CCommand &command);
};

#endif //_DOTNETPLUG_MANAGED_COMMAND_H_