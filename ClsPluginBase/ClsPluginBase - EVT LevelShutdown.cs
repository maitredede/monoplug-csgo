using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace MonoPlug
{
    partial class ClsPluginBase
    {
        private readonly object _lckEventCounters = new object();
        private readonly EventHandlerList _events = new EventHandlerList();

        private int _EventCounter_LevelShutdown = 0;
        private static readonly object _EventToken_LevelShutdown = new object();

        protected event EventHandler LevelShutdown
        {
            add
            {
                Msg("ClsPluginBase::LevelShutdown_add A\n");
                lock (this._events)
                {
                    Msg("ClsPluginBase::LevelShutdown_add B\n");
                    this._events.AddHandler(_EventToken_LevelShutdown, value);
                    Msg("ClsPluginBase::LevelShutdown_add C\n");
                    if (this._EventCounter_LevelShutdown++ == 0)
                    {
                        Msg("ClsPluginBase::LevelShutdown_add D {0}\n", this._EventCounter_LevelShutdown);
                        this._main.LevelShutdown_Add(this);
                        Msg("ClsPluginBase::LevelShutdown_add E\n");
                    }
                    Msg("ClsPluginBase::LevelShutdown_add F\n");
                }
                Msg("ClsPluginBase::LevelShutdown_add G\n");
            }
            remove
            {
                Msg("ClsPluginBase::LevelShutdown_remove A\n");
                lock (this._events)
                {
                    Msg("ClsPluginBase::LevelShutdown_remove B\n");
                    this._events.RemoveHandler(_EventToken_LevelShutdown, value);
                    Msg("ClsPluginBase::LevelShutdown_remove C {0}\n", this._EventCounter_LevelShutdown);
                    if (this._EventCounter_LevelShutdown++ == 0)
                    {
                        Msg("ClsPluginBase::LevelShutdown_remove D\n");
                        this._main.LevelShutdown_Remove(this);
                    }
                    Msg("ClsPluginBase::LevelShutdown_remove E\n");
                }
                Msg("ClsPluginBase::LevelShutdown_remove F\n");
            }
        }

        internal void Raise_LevelShutdown(object sender, EventArgs e)
        {
            Msg("ClsPluginBase::LevelShutdown_raise A\n");
            EventHandler d;
            lock (this._events)
            {
                d = (EventHandler)this._events[_EventToken_LevelShutdown];
            }
            if (d != null)
            {
                d.Invoke(sender, e);
            }
            Msg("ClsPluginBase::LevelShutdown_raise B\n");
        }
    }
}
