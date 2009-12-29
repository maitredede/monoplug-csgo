using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void Msg(string message)
        {
            InternalActionDelegate d = () =>
            {
                Mono_Msg(message);
            };

            this.InterthreadCall(d);
        }

        internal void Msg(string format, params object[] vals)
        {
            this.Msg(string.Format(format, vals));
        }
    }
}
