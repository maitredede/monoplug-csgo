using System;
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
        private readonly EventHandlerList _lstHandlers = new EventHandlerList();
        private readonly InternalConvar _convar;

        internal ClsConVar(InternalConvar convar, ClsPluginBase owner)
            : base(convar, owner)
        {
#if DEBUG
            Check.NonNull("convar", convar);
#endif
            this._convar = convar;
        }

        internal InternalConvar Internal { get { return this._convar; } }

        /// <summary>
        /// Get the default value
        /// </summary>
        public string DefaultValue { get { return this._convar.DefaultValue; } }

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
                this.Plugin.ThreadPool.QueueUserWorkItem<EventHandler>(this.RaiseValueChanged, d);
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
            get { return this._convar.GetValueBool(); }
            set { this._convar.SetValueBool(value); }
        }

        /// <summary>
        /// Get or set the ConVar value as a string
        /// </summary>
        public string ValueString
        {
            get { return this._convar.GetValueString(); }
            set { this._convar.SetValueString(value); }
        }
    }
}
