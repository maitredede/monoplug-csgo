using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void Event_player_connect(ClsPlayer player)
        {
            this.AddPlayer(player);

            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.PlayerConnect))
            {
                plugin.PluginEvents.Raise_PlayerConnect(player);
            }
        }

        void IEventsAnchor.PlayerConnect_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.PlayerConnect, null);
        }

        void IEventsAnchor.PlayerConnect_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.PlayerConnect, null);
        }
    }
}
