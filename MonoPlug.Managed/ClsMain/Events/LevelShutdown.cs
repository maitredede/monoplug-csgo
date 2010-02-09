using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEventsAnchor.LevelShutdown_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.LevelShutdown, NativeMethods.Mono_EventAttach_LevelShutdown);
        }

        void IEventsAnchor.LevelShutdown_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.LevelShutdown, NativeMethods.Mono_EventDetach_LevelShutdown);
        }

        internal void Raise_LevelShutdown()
        {
            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.LevelShutdown))
            {
                plugin.PluginEvents.Raise_LevelShutdown();
            }
        }
    }
}
