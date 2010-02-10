﻿using System;
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
        private readonly IThreadPool _thPool;

        internal ConCommandDelegate Code { get { return this._code; } }
        internal ConCommandCompleteDelegate Complete { get { return this._complete; } }
        internal bool Async { get { return this._async; } }
        internal ConCommandDelegate Execute { get { return this._exec; } }
        internal override bool IsCommand { get { return true; } }

        internal InternalConCommand(string name, string help, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate complete, bool async, IThreadPool pool)
            : base(name, help, flags)
        {
#if DEBUG
            Check.NonNull("code", code);
            Check.NonNull("pool", pool);
#endif

            this._code = code;
            this._complete = complete;
            this._async = async;
            this._exec = new ConCommandDelegate(this.DoExec);
            this._thPool = pool;
        }

        private void DoExec(ClsPlayer sender, string line, string[] args)
        {
            if (this.Public != null)
            {
                //ClsPlugin
                this.Public.Execute(sender, line, args);
            }
            else
            {
                //ClsMain
                if (this._async)
                {
                    this._thPool.QueueUserWorkItem(this.DoExecMain, sender, line, args);
                }
                else
                {
                    this.DoExecMain(sender, line, args);
                }
            }
        }

        private void DoExecMain(ClsPlayer sender, string line, string[] args)
        {
            this._code(sender, line, args);
        }
    }
}
