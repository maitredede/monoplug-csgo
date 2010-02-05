using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void Event_player_connect(string name, int index, int userid, string networkid, string address)
        {
#if DEBUG
            this._msg.DevMsg("ClsMain::Event_player_connect: name={0} index={1} userid={2} networkid={3} address={4}\n", name, index, userid, networkid, address);
#endif
            ClsPlayer player = new ClsPlayer();
            player.Id = userid;
            player.Name = name;
            player.SteamID = networkid;
            player.IP = address;

            this.AddPlayer(player);

            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.PlayerConnect))
            {
                plugin.PluginEvents.Raise_PlayerConnect(player);
            }
        }

        void IEventsAnchor.PlayerConnect_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.PlayerConnect, NativeMethods.Mono_EventAttach_player_connect);
        }

        void IEventsAnchor.PlayerConnect_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.PlayerConnect, NativeMethods.Mono_EventDetach_player_connect);
        }
    }
}
