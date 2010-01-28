using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MonoPlug
{
    public sealed class ClsConVar : ClsConCommandBase
    {
        private readonly ClsMain _main;
        private readonly ConVarData _data;

        internal ClsConVar(ClsMain main, ConVarData data)
            : base(data)
        {
            this._main = main;
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
                d = (EventHandler)this._lstHandlers[EvtValueChanged];
            }
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
            get { return this._main.InterThreadCall<bool, object>(this.ValueBoolean_Get, null); }
            set { this._main.InterThreadCall<object, bool>(this.ValueBoolean_Set, value); }
        }

        private bool ValueBoolean_Get(object unused)
        {
            return NativeMethods.Mono_Convar_GetBoolean(this._data.NativeID);
        }
        private object ValueBoolean_Set(bool value)
        {
            NativeMethods.Mono_Convar_SetBoolean(this._data.NativeID, value);
            return null;
        }

        /// <summary>
        /// Get or set the ConVar value as a string
        /// </summary>
        public string ValueString
        {
            get { return this._main.InterThreadCall<string, object>(this.ValueString_Get, null); }
            set { this._main.InterThreadCall<object, string>(this.ValueString_Set, value); }
        }

        private string ValueString_Get(object unused)
        {
            return NativeMethods.Mono_Convar_GetString(this._data.NativeID);
        }
        private object ValueString_Set(string value)
        {
#if DEBUG
            NativeMethods.Mono_DevMsg("ValueString_Set (enter)\n");
            try
            {
#endif
                NativeMethods.Mono_Convar_SetString(this._data.NativeID, value);
                return null;
#if DEBUG
            }
            finally
            {
                NativeMethods.Mono_DevMsg("ValueString_Set (exit)\n");
            }
#endif
        }
    }
}
