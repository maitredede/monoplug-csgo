using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    public interface IThreadPool
    {
        void QueueUserWorkItem<T>(ThreadAction<T> work, T item);
        void QueueUserWorkItem<T1, T2>(ThreadAction<T1, T2> work, T1 item1, T2 item2);
        void QueueUserWorkItem(WaitCallback callback, object state);
    }
}
