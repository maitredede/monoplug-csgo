using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void Msg(string format, params object[] vals)
        {
            string msg;
            if (vals == null || vals.Length == 0)
            {
                msg = format;
            }
            else
            {
                msg = string.Format(format, vals);
            }
            this.InterThreadCall<object, string>(this.MsgCall, msg);
        }

        private object MsgCall(string msg)
        {
            NativeMethods.Mono_Msg(msg);
            return null;
        }
    }
}
