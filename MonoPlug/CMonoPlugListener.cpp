#include "CMonoPlugListener.h"


void *CMonoPlugListener::OnMetamodQuery(const char *iface, int *ret)
{
	if (Q_strcmp(iface, "CMonoPlug")==0)
	{
		if (ret)
			*ret = IFACE_OK;
		return static_cast<void *>(&g_MonoPlug);
	}

	if (ret)
		*ret = IFACE_FAILED;

	return NULL;
}
