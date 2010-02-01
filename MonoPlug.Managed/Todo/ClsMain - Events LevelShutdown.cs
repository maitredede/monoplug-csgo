using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEngine.LevelShutdown_Add(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.LevelShutdown, NativeMethods.Mono_EventAttach_LevelShutdown);
        }

        void IEngine.LevelShutdown_Remove(ClsPluginBase plugin)
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
                    plugin.Raise_LevelShutdown(this, EventArgs.Empty);
                }
            }
        }
    }
}
