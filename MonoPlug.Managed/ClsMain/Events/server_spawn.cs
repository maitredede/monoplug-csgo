using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void Event_server_spawn(string hostname, string address, string port, string game, string mapname, int maxplayers, string os, bool dedicated, bool password)
        {
#if DEBUG
            this._msg.DevMsg("ClsMain::Event_server_spawn: hostname={0} address={1} port={2} game={3} mapname={4} maxplayers={5} os={6} dedicated={7} password={8}\n", hostname, address, port, game, mapname, maxplayers, os, dedicated, password);
#endif
            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.ServerSpawn))
            {
                plugin.PluginEvents.Raise_ServerSpawn(hostname, address, port, game, mapname, maxplayers, os, dedicated, password);
            }
        }

        void IEventsAnchor.ServerSpawn_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.ServerSpawn, NativeMethods.Mono_EventAttach_server_spawn);
        }

        void IEventsAnchor.ServerSpawn_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.ServerSpawn, NativeMethods.Mono_EventDetach_server_spawn);
        }
    }
}
