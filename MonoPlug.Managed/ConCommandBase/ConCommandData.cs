using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class ConCommandData : ConCommandBaseData
    {
        private readonly ConCommandDelegate _code;
        private readonly ConCommandCompleteDelegate _complete;

        public ConCommandData(ClsPluginBase plugin, string name, string help, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate complete)
            : base(plugin, name, help, flags)
        {
            this._code = code;
            this._complete = complete;
        }

        public ConCommandDelegate Code { get { return this._code; } }
        public ConCommandCompleteDelegate Complete { get { return this._complete; } }
    }
}
