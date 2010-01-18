#ifndef _MONOCALLBACKS_H_
#define _MONOCALLBACKS_H_

#include "Common.h"
#include "CMonoPlug.h"
#include "CMonoCommand.h"

void Mono_Msg(MonoString*);
void Mono_Log(MonoString*);
uint64 Mono_RegisterConvar(MonoString* name, MonoString* help, int flags, MonoString* defaultValue);
bool Mono_UnregisterConvar(uint64 nativeID);

void Mono_ConvarValueChanged(IConVar *var, const char *pOldValue, float flOldValue);

MonoString* Mono_Convar_GetString(uint64 nativeID);
void Mono_Convar_SetString(uint64 nativeID, MonoString* value);

bool Mono_RegisterConCommand(MonoString* name, MonoString* help, MonoDelegate* code, int flags, MonoDelegate* complete);
bool Mono_UnregisterConCommand(MonoString* name);

MonoArray* Mono_GetPlayers();

void Attach_ConMessage();
void Detach_ConMessage();

#endif
