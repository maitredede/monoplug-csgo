#ifndef _MONOCALLBACKS_H_
#define _MONOCALLBACKS_H_

#include "monoBase.h"
#include "pluginterfaces.h"
#include "CMonoPlugin.h"

namespace MonoPlugin
{
	//Messages
	void Mono_Msg(MonoString*);
	void Mono_Log(MonoString*);
	void Mono_DevMsg(MonoString* msg);
	void Mono_Warning(MonoString* msg);
	void Mono_Error(MonoString* msg);

	//Console
	void Mono_AttachConsole();
	void Mono_DetachConsole();

	//ConCommands
	uint64 Mono_RegisterConCommand(MonoString* name, MonoString* help, int flags, MonoDelegate* code, MonoDelegate* complete);
	void Mono_UnregisterConCommand(uint64 nativeId);

	//ConVars
	uint64 Mono_RegisterConVar(MonoString* name, MonoString* help, int flags, MonoString* defaultValue);
	void Mono_UnregisterConVar(uint64 nativeId);
	void Mono_RaiseConVarChange(IConVar *var, const char *pOldValue, float flOldValue);

	MonoString* Mono_Convar_GetString(uint64 nativeID);
	void Mono_Convar_SetString(uint64 nativeID, MonoString* value);
	bool Mono_Convar_GetBoolean(uint64 nativeID);
	void Mono_Convar_SetBoolean(uint64 nativeID, bool value);

	//Various
	void Mono_ClientDialogMessage(int client, MonoString* title, MonoString* message, int a, int r, int g, int b, int level, int time);
	void Mono_ClientDialogText(int client, MonoString* title, MonoString* message, int level, int time);
	void Mono_ServerCommand(MonoString* command);
	void Mono_ClientMessage(int userId, MonoString* message);

	//LevelShutdown
	void Mono_EventAttach_LevelShutdown();
	void Mono_EventDetach_LevelShutdown();

	//ServerActivate
	void Mono_EventAttach_ServerActivate();
	void Mono_EventDetach_ServerActivate();

	//ClientDisconnect
	void Mono_EventAttach_ClientDisconnect();
	void Mono_EventDetach_ClientDisconnect();

	//ClientPutInServer
	void Mono_EventAttach_ClientPutInServer();
	void Mono_EventDetach_ClientPutInServer();

	//ClientConnect
	void Mono_EventAttach_ClientConnect();
	void Mono_EventDetach_ClientConnect();
}

#endif //_MONOCALLBACKS_H_
