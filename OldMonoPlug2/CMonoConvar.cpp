#include "CMonoConvar.h"

CMonoConvar::CMonoConvar(uint64 nativeId, const char* name, const char* defaultValue, int flags, const char* help, MonoDelegate* changeCallback)
:ConVar(name, defaultValue, flags, help)
{
	this->m_nativeId = nativeId;
	this->m_changeDelegate = changeCallback;
}

void CMonoConvar::Changed(IConVar *var, const char *pOldValue, float flOldValue)
{
	CMonoHelpers::MONO_DELEGATE_CALL(this->m_changeDelegate, NULL);
}
