using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        private readonly object _lckMsg = new object();

        internal void Msg(string message)
        {
            if (Thread.CurrentThread.ManagedThreadId == this._mainThreadId)
            {
                lock (this._lckMsg)
                {
                    Mono_Msg(message);
                }
            }
            else
            {
                Console.WriteLine("MONO_MSG INTERTHREAD CALL");
            }
        }

        internal void Msg(string format, params object[] vals)
        {
            this.Msg(string.Format(format, vals));
        }
    }
}
