using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Drawing;

namespace MonoPlug
{
    internal sealed partial class ClsPluginEvents : ObjectBase, IEvents
    {
        private readonly ReaderWriterLock _lck = new ReaderWriterLock();
        private readonly EventHandlerList _events = new EventHandlerList();
        private readonly Dictionary<object, int> _eventsCount = new Dictionary<object, int>();
        private readonly ClsPluginBase _owner;
        private readonly IEventsAnchor _anchor;

        internal ClsPluginEvents(ClsPluginBase owner, IEventsAnchor anchor)
        {
#if DEBUG
            Check.NonNull("owner", owner);
            Check.NonNull("anchor", anchor);
#endif

            this._owner = owner;
            this._anchor = anchor;
        }

        private void Attach(object eventToken, EventHandler handler, ThreadAction<ClsPluginBase> attachNative)
        {
            this._AttachInternal(eventToken, handler, attachNative);
        }

        private void Attach<T>(object eventToken, EventHandler<T> handler, ThreadAction<ClsPluginBase> attachNative) where T : EventArgs
        {
            this._AttachInternal(eventToken, handler, attachNative);
        }

        private void _AttachInternal(object eventToken, Delegate handler, ThreadAction<ClsPluginBase> attachNative)
        {
            this._lck.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (!this._eventsCount.ContainsKey(eventToken))
                {
                    if (attachNative != null)
                    {
                        attachNative(this._owner);
                    }
                    this._eventsCount.Add(eventToken, 1);
                }
                else
                {
                    int count = this._eventsCount[eventToken];
                    this._eventsCount.Remove(eventToken);
                    this._eventsCount.Add(eventToken, count);
                }
                this._events.AddHandler(eventToken, handler);
            }
            finally
            {
                this._lck.ReleaseWriterLock();
            }
        }

        private void Detach(object eventToken, EventHandler handler, ThreadAction<ClsPluginBase> detachNative)
        {
            this._DetachInternal(eventToken, handler, detachNative);
        }

        private void Detach<T>(object eventToken, EventHandler<T> handler, ThreadAction<ClsPluginBase> detachNative) where T : EventArgs
        {
            this._DetachInternal(eventToken, handler, detachNative);
        }

        private void _DetachInternal(object eventToken, Delegate handler, ThreadAction<ClsPluginBase> detachNative)
        {
            this._lck.AcquireWriterLock(Timeout.Infinite);
            try
            {
                this._events.RemoveHandler(eventToken, handler);
                if (this._eventsCount.ContainsKey(eventToken))
                {
                    int count = this._eventsCount[eventToken];
                    count--;
                    if (count == 0)
                    {
                        if (detachNative != null)
                        {
                            detachNative(this._owner);
                        }
                        this._eventsCount.Remove(eventToken);
                    }
                }

            }
            finally
            {
                this._lck.ReleaseWriterLock();
            }
        }

        private void RaiseEmpty(object eventToken, object sender)
        {
            EventHandler d;
            this._lck.AcquireReaderLock(Timeout.Infinite);
            try
            {
                d = (EventHandler)this._events[eventToken];
            }
            finally
            {
                this._lck.ReleaseReaderLock();
            }
            if (d != null)
            {
                d(sender, EventArgs.Empty);
            }
        }

        private void Raise<T>(object eventToken, object sender, T args) where T : EventArgs
        {
            EventHandler<T> d;
            this._lck.AcquireReaderLock(Timeout.Infinite);
            try
            {
                d = (EventHandler<T>)this._events[eventToken];
            }
            finally
            {
                this._lck.ReleaseReaderLock();
            }
            if (d != null)
            {
                d(sender, args);
            }
        }
    }
}
