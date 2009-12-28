#ifndef _BASEACCESSOR_H_
#define _BASEACCESSOR_H_

#include <ISmmPlugin.h>
#include <convar.h>
#include "engine_wrappers.h"
#include <filesystem.h>

class BaseAccessor : public IConCommandBaseAccessor
{
public:
	bool RegisterConCommandBase(ConCommandBase *pCommandBase);
};

#endif //_BASEACCESSOR_H_
