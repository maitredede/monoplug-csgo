using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MonoPlug
{
    public sealed class ClsConVar : ClsConCommandBase
    {
        private readonly ConVarData _data;

        internal ClsConVar(ConVarData data)
            : base(data)
        {
            this._data = data;
        }

        public string DefaultValue { get { return this._data.DefaultValue; } }

        private static readonly object EvtValueChanged = new object();
        private readonly EventHandlerList _lstHandlers = new EventHandlerList();

        public event EventHandler ValueChanged
        {
            add
            {
                lock (this._lstHandlers)
                {
                    this._lstHandlers.AddHandler(EvtValueChanged, value);
                }
            }
            remove
            {
                lock (this._lstHandlers)
                {
                    this._lstHandlers.RemoveHandler(EvtValueChanged, value);
                }
            }
        }

        internal void RaiseValueChanged()
        {
            EventHandler d;
            lock (this._lstHandlers)
            {
                d = this._lstHandlers[EvtValueChanged] as EventHandler;
            }
            if (d != null)
            {
                d(this, EventArgs.Empty);
            }
        }
    }
}
