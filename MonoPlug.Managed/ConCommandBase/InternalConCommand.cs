using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class InternalConCommand : InternalConbase<ClsConCommand>
    {
        private readonly ConCommandDelegate _code;
        private readonly ConCommandCompleteDelegate _complete;
        private readonly bool _async;
        private readonly ConCommandDelegate _exec;

        internal ConCommandDelegate Code { get { return this._code; } }
        internal ConCommandCompleteDelegate Complete { get { return this._complete; } }
        internal bool Async { get { return this._async; } }
        internal ConCommandDelegate Execute { get { return this._exec; } }

        internal InternalConCommand(string name, string help, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate complete, bool async)
            : base(name, help, flags)
        {
#if DEBUG
            Check.NonNull("code", code);
#endif

            this._code = code;
            this._complete = complete;
            this._async = async;
            this._exec = new ConCommandDelegate(this.DoExec);
        }

        private void DoExec(string line, string[] args)
        {
            NativeMethods.Mono_DevMsg("DoExec enter\n");
            if (this.Public != null)
            {
                //ClsPlugin
                this.Public.Execute(line, args);
                NativeMethods.Mono_DevMsg("DoExec public\n");
            }
            else
            {
                //ClsMain
                //TODO : check the Async flag
                NativeMethods.Mono_DevMsg("DoExec main\n");
                this.DoExecMain(line, args);
            }
        }

        private void DoExecMain(string line, string[] args)
        {
            NativeMethods.Mono_DevMsg("DoExecMain enter\n");
            this._code(line, args);
        }
    }
}
