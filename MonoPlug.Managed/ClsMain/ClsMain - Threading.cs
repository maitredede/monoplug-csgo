using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        //private int _mainThreadId = 0;
        private Thread _mainThread = null;

        private sealed class ClsThreadItem : IDisposable
        {
            private readonly ManualResetEvent _latch;
            private readonly Delegate _code;
            private readonly object _param;
            private object _return = null;

            public ClsThreadItem(Delegate code, object param)
            {
                this._latch = new ManualResetEvent(false);
                this._code = code;
                this._param = param;
            }

            public void WaitOne()
            {
                this._latch.WaitOne();
            }

            public object ReturnValue { get { return this._return; } }

            public void Execute()
            {
                try
                {
                    this._return = this._code.DynamicInvoke(this._param);
                }
                finally
                {
                    this._latch.Set();
                }
            }

            void IDisposable.Dispose()
            {
                ((IDisposable)this._latch).Dispose();
            }
        }

        private readonly ReaderWriterLock _lckThreadQueue = new ReaderWriterLock();
        private readonly Queue<ClsThreadItem> _threadQueue = new Queue<ClsThreadItem>();

        internal TRet InterThreadCall<TRet, TParam>(InterThreadCallDelegate<TRet, TParam> d, TParam parameter)
        {
            //Same thread, direct call
            if (Thread.CurrentThread == this._mainThread)
            {
                return d.Invoke(parameter);
            }

            using (ClsThreadItem item = new ClsThreadItem(d, parameter))
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
                while (this._threadQueue.Count > 0)
                {
                    ClsThreadItem item = this._threadQueue.Dequeue();
                    item.Execute();
                }
            }
            finally
            {
                this._lckThreadQueue.ReleaseReaderLock();
            }
        }

        //private long _queueLength = 0;

        //private readonly object _lckThreadSync = new object();
        //private readonly ManualResetEvent _waitEngineSyncEnter = new ManualResetEvent(false);
        //private readonly ManualResetEvent _waitEngineSyncExit = new ManualResetEvent(false);

        //private long _recursiveDepth = 0;

        ///// <summary>
        ///// Make call to the engine in sync with its main thread
        ///// </summary>
        ///// <param name="d">Code to call</param>
        ///// <param name="parameter">Argument of call</param>
        //internal TRet InterThreadCall<TRet, TParam>(InterThreadCallDelegate<TRet, TParam> d, TParam parameter)
        //{
        //    //Same thread, direct call
        //    if (Thread.CurrentThread == this._mainThread)
        //    {
        //        return d.Invoke(parameter);
        //    }

        //    //recursive action, direct call
        //    if (Interlocked.Read(ref this._recursiveDepth) > 0)
        //    {
        //        return d.Invoke(parameter);
        //    }

        //    //Increment waiting queue counter
        //    Interlocked.Increment(ref this._queueLength);

        //    //wait for engine to let thread access it
        //    this._waitEngineSyncEnter.WaitOne();


        //    Thread.

        //    //Allow only one thread to access to engine at a time...
        //    lock (this._lckThreadSync)
        //    {
        //        Console.WriteLine("Access allowed to thread " + Thread.CurrentThread.ManagedThreadId);
        //        try
        //        {
        //            //Increment recursive counter
        //            Interlocked.Increment(ref this._recursiveDepth);
        //            try
        //            {
        //                return d.Invoke(parameter);
        //            }
        //            finally
        //            {
        //                Interlocked.Decrement(ref this._recursiveDepth);
        //            }
        //        }
        //        finally
        //        {
        //            //If waiting queue is empty, block new threads and allow engine to continue
        //            if (Interlocked.Decrement(ref this._queueLength) == 0)
        //            {
        //                //Reset latches state
        //                this._waitEngineSyncEnter.Reset();
        //                this._waitEngineSyncExit.Set();
        //            }
        //        }
        //    }
        //}

        //internal void GameFrame()
        //{
        //    //If there is at least one thread waiting access to the engine
        //    if (Interlocked.Read(ref this._queueLength) > 0)
        //    {
        //        //Let threads access the engine
        //        this._waitEngineSyncEnter.Set();
        //        //Wait for queue to end
        //        this._waitEngineSyncExit.WaitOne();
        //        //Reset the queue end latch
        //        this._waitEngineSyncExit.Reset();
        //    }
        //}
    }
}
