#include "CMonoPlugin.h"

void MonoPlugin::CMonoPlugin::Hook_GameFrame(bool simulating)
{
	//don't log this, it just pumps stuff to the screen ;]
	//META_LOG(g_PLAPI, "GameFrame() called: simulating=%d", simulating);
	MonoPlugin::CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_GameFrame);
	//RETURN_META(MRES_IGNORED);
}
