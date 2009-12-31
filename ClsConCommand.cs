using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    public sealed class ClsConCommand : ClsConCommandBase
    {
        internal ClsConCommand(UInt64 nativeId, string name, string description, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate complete)
            : base(nativeId, name, description, flags)
        {
        }
    }
}
