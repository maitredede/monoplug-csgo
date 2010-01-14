using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    public sealed class ClsConCommand : ClsConCommandBase
    {
        private readonly ConCommandDelegate _code;
        private readonly ConCommandCompleteDelegate _complete;

        internal ConCommandDelegate Code { get { return this._code; } }
        internal ConCommandCompleteDelegate Complete { get { return this._complete; } }

        internal ClsConCommand(UInt64 nativeId, string name, string description, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate complete)
            : base(nativeId, name, description, flags)
        {
            this._code = code;
            this._complete = complete;
        }
    }
}
