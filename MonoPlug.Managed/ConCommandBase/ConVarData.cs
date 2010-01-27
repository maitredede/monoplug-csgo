using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class ConVarData : ConCommandBaseData
    {
        private readonly string _defValue;

        internal ConVarData(ClsPluginBase plugin, string name, string help, FCVAR flags, string defaultValue)
            : base(plugin, name, help, flags)
        {
            this._defValue = defaultValue;
        }

        public string DefaultValue { get { return this._defValue; } }
    }
}
