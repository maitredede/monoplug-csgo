using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class ClsPluginThreadPool : ObjectBase, IThreadPool
    {
        private readonly IThreadPool _pool;
        private readonly ClsPluginBase _owner;

        internal ClsPluginThreadPool(ClsPluginBase owner, IThreadPool pool)
        {
#if DEBUG
            Check.NonNull("owner", owner);
            Check.NonNull("pool", pool);
#endif
            this._owner = owner;
            this._pool = pool;
        }

        #region IThreadPool Membres

        void IThreadPool.QueueUserWorkItem<T>(ThreadAction<T> work, T item)
        {
            this._pool.QueueUserWorkItem<T>(work, item);
        }

        void IThreadPool.QueueUserWorkItem<T1, T2>(ThreadAction<T1, T2> work, T1 item1, T2 item2)
        {
            this._pool.QueueUserWorkItem<T1, T2>(work, item1, item2);
        }

        void IThreadPool.QueueUserWorkItem(System.Threading.WaitCallback callback, object state)
        {
            this._pool.QueueUserWorkItem(callback, state);
        }

        #endregion
    }
}
