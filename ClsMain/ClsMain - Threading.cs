using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void InterThreadCall(ThreadStart d)
        {
            if (Thread.CurrentThread.ManagedThreadId == this._mainThreadId || Interlocked.Exchange(ref this._isInITCall, this._isInITCall) > 1)
            {
                d.Invoke();
            }
            else
            {
                Interlocked.Increment(ref this._queueLength);
                this._waitIn.WaitOne();
                lock (this._lckThreadSync)
                {
                    Interlocked.Increment(ref this._isInITCall);
                    d.Invoke();
                    Interlocked.Decrement(ref this._isInITCall);

                    if (Interlocked.Decrement(ref this._queueLength) == 0)
                    {
                        this._waitIn.Reset();
                        this._waitOut.Set();
                    }
                }
            }
        }

        internal void EVT_GameFrame()
        {
            if (Interlocked.Exchange(ref this._queueLength, this._queueLength) > 0)
            {
                this._waitIn.Set();
                this._waitOut.WaitOne();
                this._waitOut.Reset();
            }
        }
    }
}
