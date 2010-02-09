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
#if DEBUG
            string p;
            if (player == null)
            {
                p = "<null>";
            }
            else
            {
                p = player.Dump();
            }

            this._msg.Msg("ClsMain::Raise_ClientConnect: player={0}\n", p);
#endif

            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.ClientConnect))
            {
                //plugin.Events.Raise_ClientConnect(player);
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
