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

            ClsThreadItem<TRet, TParam> item = new ClsThreadItem<TRet, TParam>(this, d, parameter);
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
            this._lckThreadQueue.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (this._threadQueue.Count > 0)
                {
#if DEBUG
                    NativeMethods.Mono_DevMsg(string.Format("ITH: GameFrame {0} jobs\n", this._threadQueue.Count));
#endif
                    List<IExecute> lstToExec = new List<IExecute>();

                    LockCookie cookie = this._lckThreadQueue.UpgradeToWriterLock(Timeout.Infinite);
                    try
                    {
                        while (this._threadQueue.Count > 0)
                        {
                            lstToExec.Add(this._threadQueue.Dequeue());
                            NativeMethods.Mono_DevMsg("ITH: Dequeue\n");
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
                        this._lckThreadQueue.DowngradeFromWriterLock(ref cookie);
                        NativeMethods.Mono_DevMsg("ITH: loop exit\n");
                    }

#if DEBUG
                    try
                    {
#endif
                        for (int i = 0; i < lstToExec.Count; i++)
                        {
                            IExecute item = lstToExec[i];
                            try
                            {
                                item.Execute();
                            }
                            catch (Exception ex)
                            {
                                this.Warning(ex);
                            }
                        }
#if DEBUG
                    }
                    finally
                    {
                        NativeMethods.Mono_DevMsg("ITH: Done\n");
                    }
#endif
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
                this._lckThreadQueue.ReleaseReaderLock();
            }
        }

        internal void QueueUserWorkItem(WaitCallback callback, object state)
        {
            this._pool.QueueUserWorkItem(callback, state);
        }
    }
}
