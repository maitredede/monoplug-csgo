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
