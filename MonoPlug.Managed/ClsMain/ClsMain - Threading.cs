using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
#if DEBUG
        private bool _Verbose = false;
        public bool Verbose { get { return this._Verbose; } set { this._Verbose = value; } }
#endif
        private int _mainThreadId = 0;

        private readonly ReaderWriterLock _lckThreadQueue = new ReaderWriterLock();
        private readonly Queue<IExecute> _threadQueue = new Queue<IExecute>();

        internal TRet InterThreadCall<TRet, TParam>(InterThreadCallDelegate<TRet, TParam> d, TParam parameter)
        {
#if DEBUG
            if (_Verbose) Console.WriteLine("ITC: th={0}, main={1}", Thread.CurrentThread.ManagedThreadId == this._mainThreadId);
#endif
            //Same thread, direct call
            if (Thread.CurrentThread.ManagedThreadId == this._mainThreadId)
            {
#if DEBUG
                if (_Verbose) Console.WriteLine("ITC: direct call");
#endif
                return d.Invoke(parameter);
            }

#if DEBUG
            if (_Verbose) Console.WriteLine("ITC: new item");
#endif
            ClsThreadItem<TRet, TParam> item = new ClsThreadItem<TRet, TParam>(this, d, parameter);

#if DEBUG
            if (_Verbose) Console.WriteLine("ITC: writer lock");
#endif
            this._lckThreadQueue.AcquireWriterLock(Timeout.Infinite);
            try
            {
#if DEBUG
                if (_Verbose) Console.WriteLine("ITC: enqueue");
#endif
                this._threadQueue.Enqueue(item);
#if DEBUG
                if (_Verbose) Console.WriteLine("ITC: enqueue OK");
#endif
            }
            finally
            {
#if DEBUG
                if (_Verbose) Console.WriteLine("ITC: release writer");
#endif
                this._lckThreadQueue.ReleaseWriterLock();
            }

#if DEBUG
            if (_Verbose) Console.WriteLine("ITC: item wait");
#endif
            item.WaitOne();
#if DEBUG
            if (_Verbose) Console.WriteLine("ITC: item return");
#endif
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
                    this.Warning(ex);
                }
                catch
                {
                    Exception err = ex;
                    do
                    {
                        Console.WriteLine("GameFrame : {0}", ex.GetType().FullName);
                        Console.WriteLine("GameFrame : {0}", ex.Message);
                        Console.WriteLine("GameFrame : {0}", ex.StackTrace);
                    }
                    while ((err = ex.InnerException) != null);
                }
                throw ex;
            }
        }

        internal void GameFrame_Old()
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
