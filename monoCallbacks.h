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

	//Convars
	//uint64 Mono_RegisterConvar(MonoString* name, MonoString* help, int flags, MonoString* defaultValue, MonoDelegate* changeCallback);
	//bool Mono_UnregisterConvar(uint64 nativeID);
	//void Mono_CMonoConvar_ValueChanged(IConVar *var, const char *pOldValue, float flOldValue);

	//MonoString* Mono_Convar_GetString(uint64 nativeID);
	//void Mono_Convar_SetString(uint64 nativeID, MonoString* value);

	//bool Mono_Convar_GetBoolean(uint64 nativeID);
	//void Mono_Convar_SetBoolean(uint64 nativeID, bool value);

	//bool Mono_RegisterConCommand(MonoString* name, MonoString* help, MonoDelegate* code, int flags, MonoDelegate* complete);
	//bool Mono_UnregisterConCommand(MonoString* name);

	//void Mono_ClientDialogMessage(int client, MonoString* title, MonoString* message, int a, int r, int g, int b, int level, int time);
	//void Mono_ShowMOTD(int index, char *title, char *msg, int type, char *cmd);
}

#endif //_MONOCALLBACKS_H_
