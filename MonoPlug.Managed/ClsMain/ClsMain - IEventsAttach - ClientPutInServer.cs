using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEventsAnchor.ClientPutInServer_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.ClientPutInServer, NativeMethods.Mono_EventAttach_ClientPutInServer);
        }

        void IEventsAnchor.ClientPutInServer_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.ClientPutInServer, NativeMethods.Mono_EventDetach_ClientPutInServer);
        }

        internal void Raise_ClientPutInServer(ClsPlayer player)
        {
            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.ClientPutInServer))
            {
                plugin.PluginEvents.Raise_ClientPutInServer(player);
            }
        }
    }
}
