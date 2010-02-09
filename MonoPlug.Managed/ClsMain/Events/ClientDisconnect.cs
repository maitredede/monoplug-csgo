using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEventsAnchor.ClientDisconnect_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.ClientDisconnect, null);
        }

        void IEventsAnchor.ClientDisconnect_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.ClientDisconnect, null);
        }

        internal void Raise_ClientDisconnect(ClsPlayer player)
        {
            if (player == null)
            {
                this._msg.Msg("ClsMain::Raise_ClientDisconnect: player=<null>\n");
            }
            else
            {
                this._msg.Msg("ClsMain::Raise_ClientDisconnect: player={0}\n", player.Dump());
            }
            this.RemovePlayer(player);
            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.ClientDisconnect))
            {
                plugin.PluginEvents.Raise_ClientDisconnect(player);
            }
        }
    }
}
