#include "monoCallbacks.h"

using namespace MonoPlugin;

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