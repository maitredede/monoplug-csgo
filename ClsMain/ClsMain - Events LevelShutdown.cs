using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace MonoPlug
{
    partial class ClsMain
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Attach_LevelShutdown();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Detach_LevelShutdown();

        private static readonly object _evtLevelShutdown = new object();

        internal void LevelShutdown_Add(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, _evtLevelShutdown, Attach_LevelShutdown);
        }

        internal void LevelShutdown_Remove(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, _evtLevelShutdown, Detach_LevelShutdown);
        }

        private void Raise_LevelShutdown()
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
