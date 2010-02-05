﻿using System;
using System.Runtime.Remoting.Lifetime;

namespace MonoPlug
{
    /// <summary>
    /// Base class for plugins
    /// </summary>
    public abstract partial class ClsPluginBase : ObjectBase
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ClsPluginBase()
        {
        }

        #region Abstracts
        /// <summary>
        /// Plugin name
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Plugin description
        /// </summary>
        public abstract string Description { get; }
        /// <summary>
        /// Plugin load
        /// </summary>
        protected abstract void Load();
        /// <summary>
        /// Plugin unload
        /// </summary>
        protected abstract void Unload();
        #endregion

        private ClsProxy _proxy;
        private ClsPluginMessage _msg;
        private ClsPluginEvents _events;
        private ClsPluginEngine _entry;
        private ClsPluginThreadPool _pool;
        private ClsPluginDatabase _db;

        internal void Init(ClsProxy proxy, IMessage msg, IEventsAnchor anchor, IEngineWrapper entry, IThreadPool pool, IDatabaseConfig db)
        {
            Check.NonNull("proxy", proxy);
            Check.NonNull("msg", msg);
            Check.NonNull("anchor", anchor);
            Check.NonNull("entry", entry);
            Check.NonNull("pool", pool);
            Check.NonNull("db", db);

            this._proxy = proxy;
            this._msg = new ClsPluginMessage(this, msg);
            this._events = new ClsPluginEvents(this, anchor);
            this._entry = new ClsPluginEngine(this, entry);
            this._pool = new ClsPluginThreadPool(this, pool);
            this._db = new ClsPluginDatabase(this, db, this._msg);
            this.Load();
        }

        ///// <summary>
        ///// Lifetime overriding to avoid passive plugin removal
        ///// </summary>
        ///// <returns></returns>
        //public sealed override object InitializeLifetimeService()
        //{
        //    ILease lease = (ILease)base.InitializeLifetimeService();
        //    if (lease.CurrentState == LeaseState.Initial)
        //    {
        //        lease.InitialLeaseTime = TimeSpan.Zero;
        //    }
        //    return lease;
        //}

        /// <summary>
        /// Message to Console
        /// </summary>
        public IMessage Message { get { return this._msg; } }
        /// <summary>
        /// Events
        /// </summary>
        public IEvents Events { get { return this._events; } }
        /// <summary>
        /// Engine
        /// </summary>
        public IEngine Engine { get { return this._entry; } }
        /// <summary>
        /// ThreadPool (for async operations)
        /// </summary>
        public IThreadPool ThreadPool { get { return this._pool; } }
        /// <summary>
        /// Database IO
        /// </summary>
        public IDatabase Database { get { return this._db; } }
        internal ClsProxy Proxy { get { return this._proxy; } }
        internal ClsPluginEvents PluginEvents { get { return this._events; } }

        internal void Uninit()
        {
            this.Unload();
        }
    }
}
