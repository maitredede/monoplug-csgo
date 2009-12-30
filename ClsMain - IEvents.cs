using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain : IEvents
    {
        #region Hooked event management
        private readonly EventHandlerList _events = new EventHandlerList();

        private void AddHandler(object idx, Delegate handler, ref int counter, ThreadStart addHook)
        {
#if DEBUG
            Msg("ClsMain::AddHandler in AppDomain '{0}'\n", AppDomain.CurrentDomain.FriendlyName);
            try
            {
#endif
                lock (this._events)
                {
                    this._events.AddHandler(idx, handler);
                    if (counter++ == 0)
                    {
                        this.InterThreadCall(addHook.Invoke);
                    }
                }
#if DEBUG
            }
            finally
            {
                Msg("M: AddHandler Exit\n");
            }
#endif
        }

        private void RemoveHandler(object idx, Delegate handler, ref int counter, ThreadStart remHook)
        {
            Msg("ClsMain::RemoveHandler in AppDomain '{0}'\n", AppDomain.CurrentDomain.FriendlyName);
            lock (this._events)
            {
                this._events.AddHandler(idx, handler);
                if (--counter == 0)
                {
                    this.InterThreadCall(remHook.Invoke);
                }
            }
        }

        private void RaiseHandler(object idx, params object[] args)
        {
            Msg("ClsMain::RaiseHandler in AppDomain '{0}'\n", AppDomain.CurrentDomain.FriendlyName);
            lock (this._events)
            {
                Delegate d = this._events[idx];
                if (d != null)
                {
                    d.DynamicInvoke(args);
                }
            }
        }
        #endregion

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Attach_LevelShutdown();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern void Detach_LevelShutdown();

        private int _evtLevelShutdownCount = 0;
        private static readonly object _evtLevelShutdown = new object();

        void IEvents.LevelShutdown_Add(EventHandler action)
        {
            Msg("ClsMain::IEvents.LevelShutdown_Add in AppDomain '{0}'\n", AppDomain.CurrentDomain.FriendlyName);
            this.AddHandler(_evtLevelShutdown, action, ref this._evtLevelShutdownCount, Attach_LevelShutdown);
        }

        void IEvents.LevelShutdown_Remove(EventHandler action)
        {
            Msg("ClsMain::IEvents.LevelShutdown_Remove in AppDomain '{0}'\n", AppDomain.CurrentDomain.FriendlyName);
            this.RemoveHandler(_evtLevelShutdown, action, ref this._evtLevelShutdownCount, Attach_LevelShutdown);
        }

        private void Raise_LevelShutdown()
        {
            Msg("ClsMain::Raise_LevelShutdown in AppDomain '{0}'\n", AppDomain.CurrentDomain.FriendlyName);
            this.RaiseHandler(_evtLevelShutdown, this, EventArgs.Empty);
        }
    }
}
