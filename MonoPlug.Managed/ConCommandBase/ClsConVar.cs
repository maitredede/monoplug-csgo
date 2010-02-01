﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MonoPlug
{
    /// <summary>
    /// A Console Variable
    /// </summary>
    public sealed class ClsConVar : ClsConCommandBase
    {
        private readonly IConVarValue _val;
        private readonly string _defaultValue;

        internal ClsConVar(IMessage msg, IThreadPool pool, ClsPluginBase plugin, string name, string help, FCVAR flags, IConVarValue val, string defaultValue)
            : base(msg, pool, plugin, name, help, flags)
        {
            this._val = val;
            this._defaultValue = defaultValue;
        }

        /// <summary>
        /// Get the default value
        /// </summary>
        public string DefaultValue { get { return this._defaultValue; } }

        private readonly EventHandlerList _lstHandlers = new EventHandlerList();

        /// <summary>
        /// Raised when the convar value has changed
        /// </summary>
        public event EventHandler ValueChanged
        {
            add
            {
                lock (this._lstHandlers)
                {
                    this._lstHandlers.AddHandler(Events.ConvarValueChanged, value);
                }
            }
            remove
            {
                lock (this._lstHandlers)
                {
                    this._lstHandlers.RemoveHandler(Events.ConvarValueChanged, value);
                }
            }
        }

        internal void RaiseValueChanged()
        {
            EventHandler d;
            lock (this._lstHandlers)
            {
                d = (EventHandler)this._lstHandlers[Events.ConvarValueChanged];
            }
            if (d != null)
            {
                this.ThreadPool.QueueUserWorkItem<EventHandler>(this.RaiseValueChanged, d); //d(this, EventArgs.Empty);
            }
        }

        private void RaiseValueChanged(EventHandler d)
        {
            if (d != null)
            {
                d(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get or set the ConVar value as a boolean
        /// </summary>
        public bool ValueBoolean
        {
            get { return this._val.GetValueBool(this.NativeID); }
            set { this._val.SetValueBool(this.NativeID, value); }
        }

        /// <summary>
        /// Get or set the ConVar value as a string
        /// </summary>
        public string ValueString
        {
            get { return this._val.GetValueString(this.NativeID); }
            set { this._val.SetValueString(this.NativeID, value); }
        }
    }
}
