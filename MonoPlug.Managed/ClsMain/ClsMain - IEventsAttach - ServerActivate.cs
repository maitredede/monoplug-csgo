using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain : IEventsAnchor
    {
        void IEventsAnchor.ServerActivate_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.ServerActivate, NativeMethods.Mono_EventAttach_ServerActivate);
        }

        void IEventsAnchor.ServerActivate_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.ServerActivate, NativeMethods.Mono_EventDetach_ServerActivate);
        }

        internal void Raise_ServerActivate(int maxclient)
        {
            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.ServerActivate))
            {
                plugin.PluginEvents.Raise_ServerActivate(maxclient);
            }
        }
    }
}
