#ifndef _CBASEACCESSOR_H_
#define _CBASEACCESSOR_H_

#include "monoBase.h"
#include "CMonoPlugin.h"

namespace MonoPlugin
{
	class CBaseAccessor : public IConCommandBaseAccessor
	{
	public: 
		bool RegisterConCommandBase(ConCommandBase *pCommandBase);
	};
}

extern MonoPlugin::CBaseAccessor s_BaseAccessor;
#endif //_CBASEACCESSOR_H_
