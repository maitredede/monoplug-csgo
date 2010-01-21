using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        public void DevMsg(string format, params object[] elements)
        {
            string msg;
            if (elements != null && elements.Length > 0)
            {
                msg = string.Format(format, elements);
            }
            else
            {
                msg = format;
            }
            this.InterThreadCall<object, string>(this.DevMsg_CALL, msg);
        }

        private object DevMsg_CALL(string msg)
        {
            NativeMethods.Mono_Msg(msg);
            return null;
        }
    }
}
