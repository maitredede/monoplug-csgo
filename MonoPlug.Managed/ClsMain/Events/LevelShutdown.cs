using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEventsAnchor.LevelShutdown_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.LevelShutdown, null);
        }

        void IEventsAnchor.LevelShutdown_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.LevelShutdown, null);
        }

        internal void Raise_LevelShutdown()
        {
            this.ClearPlayer();
            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.LevelShutdown))
            {
                plugin.PluginEvents.Raise_LevelShutdown();
            }
        }
    }
}
