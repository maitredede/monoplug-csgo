using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal interface IConItemEntry
    {
        InternalConvar RegisterConvar(string name, string help, FCVAR flags, string defaultValue);
        void UnregisterConvar(InternalConvar var);

        InternalConCommand RegisterConCommand(string name, string help, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate complete, bool async);
        void UnregisterConCommand(InternalConCommand var);
    }
}
