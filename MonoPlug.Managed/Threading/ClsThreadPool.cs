using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    internal sealed class ClsThreadPool : ObjectBase, IThreadPool
    {
        internal sealed class ThreadPoolItem
        {
            private WaitCallback _dlg;
            private object _state;

            public ThreadPoolItem(WaitCallback dlg, object state)
            {
                this._dlg = dlg;
                this._state = state;
            }

            public WaitCallback Delegate { get { return this._dlg; } }
            public object State { get { return this._state; } }
        }

        private readonly Queue<ThreadPoolItem> _queue;
        private readonly int _size;
        private int _running;
        private IMessage _msg;

        public ClsThreadPool(IMessage message)
            : this(message, Environment.ProcessorCount * 10)
        {
        }

        public ClsThreadPool(IMessage message, int size)
        {
            Check.InRange("size", size, 1, int.MaxValue);

            this._queue = new Queue<ThreadPoolItem>();
            this._size = size;
            this._running = 0;
            this._msg = message;
        }

        public void QueueUserWorkItem<T>(ThreadAction<T> work, T item)
        {
            WaitCallback wcb = delegate(object state)
            {
                work(item);
            };
            this.QueueUserWorkItem(wcb, null);
        }

        public void QueueUserWorkItem<T1, T2>(ThreadAction<T1, T2> work, T1 item1, T2 item2)
        {
            WaitCallback wcb = delegate(object state)
            {
                work(item1, item2);
            };
            this.QueueUserWorkItem(wcb, null);
        }

        public void QueueUserWorkItem<T1, T2, T3>(ThreadAction<T1, T2, T3> work, T1 item1, T2 item2, T3 item3)
        {
            WaitCallback wcb = delegate(object state)
            {
                work(item1, item2, item3);
            };
            this.QueueUserWorkItem(wcb, null);
        }

        public int GetMaxWorkerThread()
        {
            return this._size;
        }

        public void QueueUserWorkItem(WaitCallback cb, object state)
        {
            try
            {
                lock (this._queue)
                {
                    ThreadPoolItem item = new ThreadPoolItem(cb, state);
                    if (this._running < this._size)
                    {
                        Thread th = new Thread(new ParameterizedThreadStart(this.Run));
                        //th.Name = "Pool";
                        this._running++;
                        th.Start(item);
                    }
                    else
                    {
                        this._queue.Enqueue(item);
                    }
                }
            }
            catch (Exception ex)
            {
                this._msg.Warning(ex);
                throw ex;
            }
        }

        private void Run(object state)
        {
            ThreadPoolItem item = (ThreadPoolItem)state;
            while (true)
            {
                item.Delegate(item.State);
                lock (this._queue)
                {
                    if (this._queue.Count == 0)
                    {
                        this._running--;
                        break;
                    }
                    else
                    {
                        item = this._queue.Dequeue();
                    }
                }
            }
        }
    }
}
