using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    /// <summary>
    /// Semaphore class that accepts negative values for initial count
    /// </summary>
    public sealed class ParallelSemaphore : MarshalByRefObject, IDisposable
    {
        private readonly Mutex _mut;
        private int _count;
        private readonly int _max;
        private readonly ManualResetEvent _latch;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="count">Initial value</param>
        /// <param name="max">Maximum count</param>
        public ParallelSemaphore(int count, int max)
            : base()
        {
            this._mut = new Mutex();
            this._count = count;
            this._max = max;
            this._latch = new ManualResetEvent(this._count > 0);
        }

        /// <summary>
        /// Block until one is available
        /// </summary>
        public void WaitOne()
        {
            while (true)
            {
                this._latch.WaitOne();
                this._mut.WaitOne();
                if (this._count > 0)
                {
                    this._count--;
                    this._mut.ReleaseMutex();
                    return;
                }
                this._mut.ReleaseMutex();
            }
        }

        /// <summary>
        /// Release one
        /// </summary>
        public void Release()
        {
            this._mut.WaitOne();
            this._count++;
            if (this._count > this._max)
            {
                this._count = this._max;
            }
            if (this._count > 0)
            {
                this._latch.Set();
            }
            this._mut.ReleaseMutex();
        }

        void IDisposable.Dispose()
        {
            this._latch.Close();
            ((IDisposable)this._latch).Dispose();
            this._mut.Close();
            ((IDisposable)this._mut).Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
