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
    }
}
