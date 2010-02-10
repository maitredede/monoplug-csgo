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

            ClsThreadItem<TRet, TParam> item = new ClsThreadItem<TRet, TParam>(this._msg, d, parameter);

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

        internal void GameFrame()
        {
            try
            {
                this._lckThreadQueue.AcquireReaderLock(Timeout.Infinite);
                int count;
                List<IExecute> lst = null;
                try
                {
                    count = this._threadQueue.Count;
                    if (count > 0)
                    {
                        lst = new List<IExecute>();
                        LockCookie cookie = this._lckThreadQueue.UpgradeToWriterLock(Timeout.Infinite);
                        try
                        {
                            while (this._threadQueue.Count > 0)
                            {
                                lst.Add(this._threadQueue.Dequeue());
                            }
                        }
                        finally
                        {
                            this._lckThreadQueue.DowngradeFromWriterLock(ref cookie);
                        }
                    }
                }
                finally
                {
                    this._lckThreadQueue.ReleaseReaderLock();
                }

                if (count > 0)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        try
                        {
                            lst[i].Execute();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    this._msg.Warning(ex);
                }
                catch
                {
                    Exception err = ex;
                    do
                    {
                        Console.WriteLine("GameFrame : {0}", err.GetType().FullName);
                        Console.WriteLine("GameFrame : {0}", err.Message);
                        Console.WriteLine("GameFrame : {0}", err.StackTrace);
                    }
                    while ((err = ex.InnerException) != null);
                }
                throw ex;
            }
        }
    }
}
