using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;

namespace MonoPlug
{
    internal sealed class ParallelForEach<T>
    {
        private readonly ICollection<T> _col;
        private readonly IEnumerator<T> _enum;
        private readonly int _thcount;
        private ParallelSemaphore _sem_startlatch;
        private ParallelSemaphore _sem_endlatch;
        private Dictionary<T, Exception> _lstEx;
        private readonly Action<T> _action;

        internal ParallelForEach(ICollection<T> collection, Action<T> action, int threadcount)
        {
            this._col = collection;
            this._action = action;
            this._thcount = threadcount;

            this._enum = this._col.GetEnumerator();
        }

        public Dictionary<T, Exception> Execute()
        {
            this._lstEx = new Dictionary<T, Exception>();
            if (this._thcount == 1)
            {
                while (this._enum.MoveNext())
                {
                    T item = this._enum.Current;
                    try
                    {
                        this._action.Invoke(item);
                    }
                    catch (TargetInvocationException ex)
                    {
                        this._lstEx.Add(item, ex.InnerException);
                    }
                }
            }
            else
            {
                using (this._sem_startlatch = new ParallelSemaphore(1 - this._thcount, 1))
                {
                    using (this._sem_endlatch = new ParallelSemaphore(1 - this._thcount, 1))
                    {
                        for (int i = 0; i < this._thcount; i++)
                        {
                            Thread th = new Thread(new ThreadStart(this.Run));
                            //th.Name = "Parallel.ForEach[" + i + "]";
                            th.Start();
                        }
                        this._sem_startlatch.WaitOne();
                        this._sem_endlatch.WaitOne();
                    }
                }
            }
            return this._lstEx;
        }

        private void Run()
        {
            this._sem_startlatch.Release();
            T item = default(T);
            bool read = true;
            while (read)
            {
                lock (this._enum)
                {
                    read = this._enum.MoveNext();
                    if (read)
                    {
                        item = this._enum.Current;
                    }
                }

                if (!read)
                {
                    break;
                }

                try
                {
                    this._action.Invoke(item);
                }
                catch (TargetInvocationException ex)
                {
                    lock (this._lstEx)
                    {
                        this._lstEx.Add(item, ex.InnerException);
                    }
                }
            }
            this._sem_endlatch.Release();
        }
    }
}
