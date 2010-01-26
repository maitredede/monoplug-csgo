#include "CBaseAccessor.h"

MonoPlugin::CBaseAccessor s_BaseAccessor;

bool MonoPlugin::CBaseAccessor::RegisterConCommandBase(ConCommandBase *pCommandBase)
{
	/* Always call META_REGCVAR instead of going through the engine. */
	return META_REGCVAR(pCommandBase);
}
