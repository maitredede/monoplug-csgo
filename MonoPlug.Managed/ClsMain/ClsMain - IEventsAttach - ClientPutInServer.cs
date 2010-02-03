using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEventsAttach.ClientPutInServer_Add(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.ClientPutInServer, NativeMethods.Mono_EventAttach_ClientPutInServer);
        }

        void IEventsAttach.ClientPutInServer_Remove(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.ClientPutInServer, NativeMethods.Mono_EventDetach_ClientPutInServer);
        }

        internal void Raise_ClientPutInServer(ClsPlayer player)
        {
            List<ClsPluginBase> lst = null;
            lock (this._events)
            {
                if (this._events.ContainsKey(Events.ClientPutInServer))
                {
                    lst = new List<ClsPluginBase>(this._events[Events.ClientPutInServer]);
                }
            }
            if (lst != null && lst.Count > 0)
            {
                foreach (ClsPluginBase plugin in lst)
                {
                    plugin.PluginEvents.Raise_ClientPutInServer(player);
                }
            }
        }
    }
}
