using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEventsAttach.LevelShutdown_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.LevelShutdown, NativeMethods.Mono_EventAttach_LevelShutdown);
        }

        void IEventsAttach.LevelShutdown_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.LevelShutdown, NativeMethods.Mono_EventDetach_LevelShutdown);
        }

        internal void Raise_LevelShutdown()
        {
            List<ClsPluginBase> lst = null;
            lock (this._events)
            {
                if (this._events.ContainsKey(Events.LevelShutdown))
                {
                    lst = new List<ClsPluginBase>(this._events[Events.LevelShutdown]);
                }
            }
            if (lst != null && lst.Count > 0)
            {
                foreach (ClsPluginBase plugin in lst)
                {
                    plugin.PluginEvents.Raise_LevelShutdown();
                }
            }
        }
    }
}
