using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class InternalConvar : InternalConbase<ClsConVar>
    {
        private readonly string _defaultValue;
        private readonly IConVarValue _accessor;

        internal InternalConvar(string name, string help, FCVAR flags, string defaultvalue, IConVarValue accessor)
            : base(name, help, flags)
        {
            this._defaultValue = defaultvalue;
            this._accessor = accessor;
        }

        internal override bool IsCommand { get { return false; } }
        internal string DefaultValue { get { return this._defaultValue; } }

        internal void RaiseValueChanged()
        {
            if (this.Public != null)
            {
                this.Public.RaiseValueChanged();
            }
        }

        internal string GetValueString()
        {
            return this._accessor.GetValueString(this.NativeID);
        }

        internal void SetValueString(string value)
        {
            this._accessor.SetValueString(this.NativeID, value);
        }

        internal bool GetValueBool()
        {
            return this._accessor.GetValueBool(this.NativeID);
        }

        internal void SetValueBool(bool value)
        {
            this._accessor.SetValueBool(this.NativeID, value);
        }
    }
}
