using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void Event_server_shutdown(string reason)
        {
            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.ServerShutdown))
            {
                plugin.PluginEvents.Raise_ServerShutdown(reason);
            }
        }

        void IEventsAnchor.ServerShutdown_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.ServerShutdown, NativeMethods.Mono_EventAttach_server_shutdown);
        }

        void IEventsAnchor.ServerShutdown_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.ServerShutdown, NativeMethods.Mono_EventDetach_server_shutdown);
        }
    }
}
