#include "CMonoPlugin.h"

void MonoPlugin::CMonoPlugin::OnVSPListening(IServerPluginCallbacks *iface)
{
	vsp_callbacks = iface;
}