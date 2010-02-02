using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain : IEventsAttach
    {
        private readonly Dictionary<object, List<ClsPluginBase>> _events = new Dictionary<object, List<ClsPluginBase>>();

        #region internal funcs
        private void Event_Add(ClsPluginBase plugin, object evt, ThreadStart attach)
        {
            lock (this._events)
            {
                if (!this._events.ContainsKey(evt))
                {
                    this._events.Add(evt, new List<ClsPluginBase>());
                    if (attach != null)
                    {
                        this.InterThreadCall<object, ThreadStart>(this.Event_Add_Call, attach);
                    }
                }
                this._events[evt].Add(plugin);
            }
        }

        private object Event_Add_Call(ThreadStart attach)
        {
            attach.Invoke();
            return null;
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
                        if (detach != null)
                        {
                            this.InterThreadCall<object, ThreadStart>(this.Event_Remove_Call, detach);
                        }
                    }
                }
            }
        }

        private object Event_Remove_Call(ThreadStart detach)
        {
            detach.Invoke();
            return null;
        }
        #endregion

        #region IEventsAttach Membres

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
        #endregion
    }
}
