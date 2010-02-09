using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEventsAnchor.ClientPutInServer_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.ClientPutInServer, null);
        }

        void IEventsAnchor.ClientPutInServer_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.ClientPutInServer, null);
        }

        internal void Raise_ClientPutInServer(ClsPlayer player)
        {
#if DEBUG
            this._msg.DevMsg("ClsMain::Raise_ClientPutInServer: player={0}\n", player.Dump());
#endif
            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.ClientPutInServer))
            {
                plugin.PluginEvents.Raise_ClientPutInServer(player);
            }
        }
    }
}
