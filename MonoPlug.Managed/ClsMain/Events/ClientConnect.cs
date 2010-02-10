using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void Raise_ClientConnect(ClsPlayer player)
        {
            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.ClientConnect))
            {
                plugin.PluginEvents.Raise_ClientConnect(player);
            }
        }

        void IEventsAnchor.ClientConnect_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.ClientConnect, null);
        }

        void IEventsAnchor.ClientConnect_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.ClientConnect, null);
        }
    }
}
