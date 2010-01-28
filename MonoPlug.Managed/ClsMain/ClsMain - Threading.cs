using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        private int _mainThreadId = 0;

        private readonly ReaderWriterLock _lckThreadQueue = new ReaderWriterLock();
        private readonly Queue<IExecute> _threadQueue = new Queue<IExecute>();

        internal TRet InterThreadCall<TRet, TParam>(InterThreadCallDelegate<TRet, TParam> d, TParam parameter)
        {
            //Same thread, direct call
            if (Thread.CurrentThread.ManagedThreadId == this._mainThreadId)
            {
                return d.Invoke(parameter);
            }

            ClsThreadItem<TRet, TParam> item = new ClsThreadItem<TRet, TParam>(d, parameter);
            {
                this._lckThreadQueue.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    this._threadQueue.Enqueue(item);
                }
                finally
                {
                    this._lckThreadQueue.ReleaseWriterLock();
                }

                item.WaitOne();
                return (TRet)item.ReturnValue;
            }
        }

        internal void GameFrame()
        {
            //#if DEBUG
            //            NativeMethods.Mono_DevMsg("GameFrame Enter\n");
            //#endif
            this._lckThreadQueue.AcquireReaderLock(Timeout.Infinite);
            //#if DEBUG
            //            NativeMethods.Mono_DevMsg("  GF: Reader locked\n");
            //#endif
            try
            {
                if (this._threadQueue.Count > 0)
                {
                    NativeMethods.Mono_DevMsg(string.Format("ITH: GameFrame {0} jobs\n", this._threadQueue.Count));
                    LockCookie cookie = this._lckThreadQueue.UpgradeToWriterLock(Timeout.Infinite);
                    try
                    {
                        while (this._threadQueue.Count > 0)
                        {
                            IExecute item = this._threadQueue.Dequeue();
                            NativeMethods.Mono_DevMsg("ITH: Dequeue\n");
                            item.Execute();
                        }
                    }
                    finally
                    {
                        NativeMethods.Mono_DevMsg("ITH: loop exit\n");
                        this._lckThreadQueue.DowngradeFromWriterLock(ref cookie);
                    }
                }
            }
#if DEBUG
            catch (Exception ex)
            {
                this.Warning(ex);
            }
#endif
            finally
            {
                //#if DEBUG
                //                NativeMethods.Mono_DevMsg("GameFrame : Exit\n");
                //#endif
                this._lckThreadQueue.ReleaseReaderLock();
            }
        }

        internal void QueueUserWorkItem(WaitCallback callback, object state)
        {
            this._pool.QueueUserWorkItem(callback, state);
        }
    }
}
