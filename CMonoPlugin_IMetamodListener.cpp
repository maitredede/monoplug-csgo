#include "CMonoPlugin.h"
#include "pluginterfaces.h"

void MonoPlugin::CMonoPlugin::OnVSPListening(IServerPluginCallbacks *iface)
{
	g_vsp_callbacks = iface;
}
