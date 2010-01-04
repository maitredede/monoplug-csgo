using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
#if DEBUG
    [System.Diagnostics.DebuggerDisplay("ConVarString : {Name} = {Value}")]
#endif
    public sealed class ClsConVarStrings : ClsConCommandBase
    {
        internal ClsConVarStrings(UInt64 nativeId, string name, string description, FCVAR flags)
            : base(nativeId, name, description, flags)
        {
        }

        public string Value
        {
            get
            {
                //Get from native var
                return ClsMain.Mono_GetConVarStringValue(base.NativeID);
            }
            set
            {
                //Set native var
                ClsMain.Mono_SetConVarStringValue(base.NativeID, value);
            }
        }

        public event EventHandler ValueChanged;

        internal void RaiseValueChanged()
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
            }
        }
    }
}
