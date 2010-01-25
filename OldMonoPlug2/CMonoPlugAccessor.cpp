#include "CMonoPlug.h"

CMonoPlugAccessor g_Accessor;

bool CMonoPlugAccessor::RegisterConCommandBase(ConCommandBase *pVar)
{
	//this will work on any type of concmd!
	return META_REGCVAR(pVar);
}
