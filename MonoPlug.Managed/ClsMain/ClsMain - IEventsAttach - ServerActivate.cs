using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain : IEventsAttach
    {
        void IEventsAttach.ServerActivate_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.ServerActivate, NativeMethods.Mono_EventAttach_ServerActivate);
        }

        void IEventsAttach.ServerActivate_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.ServerActivate, NativeMethods.Mono_EventDetach_ServerActivate);
        }

        internal void Raise_ServerActivate(int maxclient)
        {
            List<ClsPluginBase> lst = null;
            lock (this._events)
            {
                if (this._events.ContainsKey(Events.ServerActivate))
                {
                    lst = new List<ClsPluginBase>(this._events[Events.ServerActivate]);
                }
            }
            if (lst != null && lst.Count > 0)
            {
                foreach (ClsPluginBase plugin in lst)
                {
                    plugin.PluginEvents.Raise_ServerActivate(maxclient);
                }
            }
        }
    }
}
