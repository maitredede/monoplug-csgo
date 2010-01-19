using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// A Convar used by managed code
    /// </summary>
    public class ClsConvar : ClsConCommandBase
    {
        internal readonly ClsMain _main;

        internal ClsConvar(ClsMain main, UInt64 nativeId, string name, string description, FCVAR flags)
            : base(nativeId, name, description, flags)
        {
            this._main = main;
        }

        /// <summary>
        /// Raised when the value has changed
        /// </summary>
        public event EventHandler ValueChanged;

        internal void RaiseValueChanged()
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get the Convar value as string
        /// </summary>
        /// <returns>Convar value as string</returns>
        public string GetString()
        {
            return this._main.InterThreadCall<string, object>(this.GetStringCall, null);
        }

        private string GetStringCall(object state)
        {
            return NativeMethods.Mono_Convar_GetString(this.NativeID);
        }

        /// <summary>
        /// Set Convar value as string
        /// </summary>
        /// <param name="value">Value</param>
        public void SetValue(string value)
        {
            this._main.InterThreadCall<object, string>(this.SetStringCall, value);
        }

        private object SetStringCall(string value)
        {
            NativeMethods.Mono_Convar_SetString(this.NativeID, value);
            return null;
        }

        public bool GetBoolean()
        {
            return this._main.InterThreadCall<bool, object>(this.GetBooleanCall, null);
        }

        private bool GetBooleanCall(object state)
        {
            return NativeMethods.Mono_Convar_GetBoolean(this.NativeID);
        }

        public void SetValue(bool value)
        {
            this._main.InterThreadCall<object, bool>(this.SetBooleanCall, value);
        }

        private object SetBooleanCall(bool value)
        {
            NativeMethods.Mono_Convar_SetBoolean(this.NativeID, value);
            return null;
        }
    }
}
