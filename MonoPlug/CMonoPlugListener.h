#ifndef _CMONOPLUGLISTENER_H_
#define _CMONOPLUGLISTENER_H_

#include "Common.h"

class CMonoPlugListener : public IMetamodListener
{
public:
	virtual void *OnMetamodQuery(const char *iface, int *ret);
};

#endif //_CMONOPLUGLISTENER_H_
