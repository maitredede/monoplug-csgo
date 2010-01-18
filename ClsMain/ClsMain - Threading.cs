using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal delegate TRet InterThreadCallDelegate<TRet, TParam>(TParam param);

        /// <summary>
        /// Make call to the engine in sync with its main thread
        /// </summary>
        /// <param name="d">Code to call</param>
        internal TRet InterThreadCall<TRet, TParam>(InterThreadCallDelegate<TRet, TParam> d, TParam parameter)
        {
            TRet returnValue;
            if (Thread.CurrentThread.ManagedThreadId == this._mainThreadId || Interlocked.Exchange(ref this._isInITCall, this._isInITCall) > 1)
            {
                returnValue = d.Invoke(parameter);
            }
            else
            {
                Interlocked.Increment(ref this._queueLength);
                this._waitIn.WaitOne();
                lock (this._lckThreadSync)
                {
                    Interlocked.Increment(ref this._isInITCall);
                    returnValue = d.Invoke(parameter);
                    Interlocked.Decrement(ref this._isInITCall);

                    if (Interlocked.Decrement(ref this._queueLength) == 0)
                    {
                        this._waitIn.Reset();
                        this._waitOut.Set();
                    }
                }
            }
            return returnValue;
        }

        internal void GameFrame()
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
