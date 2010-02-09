using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void Event_player_disconnect(int userid, string reason, string name, string networkid, ClsPlayer player)
        {
#if DEBUG
            this._msg.DevMsg("ClsMain::Event_player_disconnect: userid={0} reason={1} name={2} networkid={3}\n", userid, reason, name, networkid);
            if (player == null)
            {
                this._msg.DevMsg("ClsMain::Event_player_disconnect: player={0}\n", "<null>");
            }
            else
            {
                this._msg.DevMsg("ClsMain::Event_player_disconnect: player={0}\n", player.Dump());
            }
#endif
            if (player == null)
            {
                this._msg.Msg("ClsMain::Event_player_disconnect: player=<null>\n");
            }
            else
            {
                this._msg.Msg("ClsMain::Event_player_disconnect: player={0}\n", player.Dump());
            }
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
