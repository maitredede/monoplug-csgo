#ifndef _DOTNETPLUG_TYPES_H_
#define _DOTNETPLUG_TYPES_H_

#ifdef MANAGED_WIN32
#include <comutil.h>
#include <stdio.h>

typedef void(*MANAGED_COMMAND_CALLBACK)(int argc, SAFEARRAY* stringArr);
#endif //MANAGED_WIN32

#ifdef MANAGED_MONO
#error TODO : Types.h
typedef MonoMethod* MANAGED_COMMAND_CALLBACK;
#endif

#endif // _DOTNETPLUG_TYPES_H_