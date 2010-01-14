using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    public class ClsConvar : ClsConCommandBase
    {
        internal readonly ClsMain _main;

        internal ClsConvar(ClsMain main, UInt64 nativeId, string name, string description, FCVAR flags)
            : base(nativeId, name, description, flags)
        {
            this._main = main;
        }

        public event EventHandler ValueChanged;

        internal void RaiseValueChanged()
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
            }
        }

        public string GetString()
        {
            return this._main.InterThreadCall<string, object>(this.GetStringCall, null);
        }

        private string GetStringCall(object state)
        {
            return NativeMethods.Mono_Convar_GetString(this.NativeID);
        }

        public void SetValue(string value)
        {
            this._main.InterThreadCall<object, string>(this.SetStringCall, value);
        }

        private object SetStringCall(string value)
        {
            NativeMethods.Mono_Convar_SetString(this.NativeID, value);
            return null;
        }
    }
}
