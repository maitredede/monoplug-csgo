/* ======== sample_mm ========
* Copyright (C) 2004-2005 Metamod:Source Development Team
* No warranties of any kind
*
* License: zlib/libpng
*
* Author(s): David "BAILOPAN" Anderson
* ============================
*/

#include "SamplePlugin.h"
#include "cvars.h"

SampleAccessor g_Accessor;

extern SamplePlugin g_SamplePlugin;

ConVar gAdVersion("sample_version", SAMPLE_VERSION, FCVAR_SPONLY, "Sample Plugin version");

bool SampleAccessor::RegisterConCommandBase(ConCommandBase *pVar)
{
	//this will work on any type of concmd!
	return META_REGCVAR(pVar);
}

