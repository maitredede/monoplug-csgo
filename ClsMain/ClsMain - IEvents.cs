using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        private readonly Dictionary<object, List<ClsPluginBase>> _events = new Dictionary<object, List<ClsPluginBase>>();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Attach_LevelShutdown();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Detach_LevelShutdown();

        private static readonly object _evtLevelShutdown = new object();
        private static readonly object _evtClientConnect = new object();

        private void Event_Add(ClsPluginBase plugin, object evt, ThreadStart attach)
        {
            lock (this._events)
            {
                if (!this._events.ContainsKey(evt))
                {
                    this._events.Add(evt, new List<ClsPluginBase>());
                    this.InterThreadCall(attach);
                }
                this._events[evt].Add(plugin);
            }
        }

        private void Event_Remove(ClsPluginBase plugin, object evt, ThreadStart detach)
        {
            lock (this._events)
            {
                if (this._events.ContainsKey(evt))
                {
                    List<ClsPluginBase> lst = this._events[evt];
                    if (lst.Count > 0)
                    {
                        if (lst.Contains(plugin))
                        {
                            lst.Remove(plugin);
                        }
                    }

                    if (lst.Count == 0)
                    {
                        this._events.Remove(evt);
                        this.InterThreadCall(detach);
                    }
                }
            }
        }

        internal void LevelShutdown_Add(ClsPluginBase plugin)
        {
            Msg("ClsMain::LevelShutdown_Add A\n");
            this.Event_Add(plugin, _evtLevelShutdown, Attach_LevelShutdown);
            Msg("ClsMain::LevelShutdown_Add B\n");
        }

        internal void LevelShutdown_Remove(ClsPluginBase plugin)
        {
            Msg("ClsMain::LevelShutdown_Remove A\n");
            this.Event_Remove(plugin, _evtLevelShutdown, Detach_LevelShutdown);
            Msg("ClsMain::LevelShutdown_Remove B\n");
        }

        private void Raise_LevelShutdown()
        {
            Msg("ClsMain::Raise_LevelShutdown\n");
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
