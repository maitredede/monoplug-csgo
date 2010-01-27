using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace MonoPlug
{
    partial class ClsMain
    {
        private static readonly object _evtLevelShutdown = new object();

        internal void LevelShutdown_Add(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, _evtLevelShutdown, NativeMethods.Mono_EventAttach_LevelShutdown);
        }

        internal void LevelShutdown_Remove(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, _evtLevelShutdown, NativeMethods.Mono_EventDetach_LevelShutdown);
        }

        internal void Raise_LevelShutdown()
        {
            List<ClsPluginBase> lst = null;
            lock (this._events)
            {
                if (this._events.ContainsKey(_evtLevelShutdown))
                {
                    lst = new List<ClsPluginBase>(this._events[_evtLevelShutdown]);
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
