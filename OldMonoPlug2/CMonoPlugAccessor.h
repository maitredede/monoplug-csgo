#ifndef _INCLUDE_CVARS_H
#define _INCLUDE_CVARS_H

#include "Common.h"

class CMonoPlugAccessor : public IConCommandBaseAccessor
{
public:
	virtual bool RegisterConCommandBase(ConCommandBase *pVar);
};

extern CMonoPlugAccessor g_Accessor;

#endif //_INCLUDE_CVARS_H
