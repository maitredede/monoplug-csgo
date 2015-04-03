#ifndef _V8PLUG_HELPERS_H_
#define _V8PLUG_HELPERS_H_
#ifdef _WIN32
#pragma once
#endif

#include <stdlib.h>
#include <map>

#if !defined ZeroMemory
#define ZeroMemory(ptr, size) memset(ptr, 0, size);
#endif

#if defined WIN32 && !defined snprintf
#define snprintf _snprintf
#endif

#endif //_V8PLUG_HELPERS_H_