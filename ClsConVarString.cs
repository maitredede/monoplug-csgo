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
        private readonly string _name;
        private readonly string _desc;
        private readonly UInt64 _nativeId;
        private readonly FCVAR _flags;

        internal ClsConVarStrings(UInt64 nativeId, string name, string description, FCVAR flags)
            : base(nativeId, name, description, flags)
        {
        }

        public string Value
        {
            get
            {
                //Get from native var
                return ClsMain.Mono_GetConVarStringValue(this._nativeId);
            }
            set
            {
                //Set native var
                ClsMain.Mono_SetConVarStringValue(this._nativeId, value);
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
