#ifndef _CMONOCONVAR_H_
#define _CMONOCONVAR_H_

#include "Common.h"
#include "CMonoHelpers.h"
//#include "monoCallbacks.h"

class CMonoConvar: public ConVar
{
private:
	uint64 m_nativeId;
	MonoDelegate* m_changeDelegate;

public:
	CMonoConvar(uint64 nativeId, const char* name, const char* defaultValue, int flags, const char* help, MonoDelegate* changeCallback);
	void Changed(IConVar *var, const char *pOldValue, float flOldValue);
};

#endif //_CMONOCONVAR_H_
