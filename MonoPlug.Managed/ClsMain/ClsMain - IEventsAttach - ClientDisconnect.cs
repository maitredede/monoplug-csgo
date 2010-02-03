using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEventsAttach.ClientDisconnect_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.ClientDisconnect, NativeMethods.Mono_EventAttach_ClientDisconnect);
        }

        void IEventsAttach.ClientDisconnect_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.ClientDisconnect, NativeMethods.Mono_EventDetach_ClientDisconnect);
        }

        internal void Raise_ClientDisconnect(ClsPlayer player)
        {
            List<ClsPluginBase> lst = null;
            lock (this._events)
            {
                if (this._events.ContainsKey(Events.ClientDisconnect))
                {
                    lst = new List<ClsPluginBase>(this._events[Events.ClientDisconnect]);
                }
            }
            if (lst != null && lst.Count > 0)
            {
                foreach (ClsPluginBase plugin in lst)
                {
                    plugin.PluginEvents.Raise_ClientDisconnect(player);
                }
            }
        }
    }
}
