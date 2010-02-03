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
    }
}
