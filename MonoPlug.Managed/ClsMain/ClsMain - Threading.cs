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
    }
}
