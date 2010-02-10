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
        void IEngineWrapper.ServerCommand(string command)
        {
            this.InterThreadCall<object, string>(this.ServerCommand_CALL, command);
        }

        private object ServerCommand_CALL(string command)
        {
            NativeMethods.Mono_ServerCommand(command);
            return null;
        }
    }
}
