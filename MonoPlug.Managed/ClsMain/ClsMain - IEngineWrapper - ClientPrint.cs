using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEngineWrapper.ClientPrint(ClsPlayer client, string message)
        {
            this.InterThreadCall<object, CTuple<int, string>>(this.ClientMessage_CALL, new CTuple<int, string>(client.UserId, message));
        }

        private object ClientMessage_CALL(CTuple<int, string> data)
        {
            NativeMethods.Mono_ClientMessage(data.Item1, data.Item2);
            return null;
        }
    }
}
