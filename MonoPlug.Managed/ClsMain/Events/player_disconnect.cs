using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void Event_player_disconnect(ClsPlayer player, string reason)
        {
            this.RemovePlayer(player);
            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.PlayerDisconnect))
            {
                plugin.PluginEvents.Raise_PlayerDisconnect(player, reason);
            }
        }

        void IEventsAnchor.PlayerDisconnect_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.PlayerDisconnect, null);
        }

        void IEventsAnchor.PlayerDisconnect_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.PlayerDisconnect, null);
        }
    }
}
