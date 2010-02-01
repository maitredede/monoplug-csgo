using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;

namespace MonoPlug
{
    /// <summary>
    /// Base class for plugins
    /// </summary>
    public abstract partial class ClsPluginBase : MarshalByRefObject
    {
        private readonly EventHandlerList _events = new EventHandlerList();
        //private ClsMain _main;
        private ClsRemote _proxy;
        private IMessage _msg;
        private IThreadPool _thPool;
        private IDatabase _db;
        private IConItem _conItem;
        private IEngine _engine;

        internal ClsRemote Proxy { get { return this._proxy; } }
        public IMessage Message { get { return this._msg; } }
        public IThreadPool ThreadPool { get { return this._thPool; } }
        public IDatabase Database { get { return this._db; } }
        public IConItem ConItem { get { return this._conItem; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ClsPluginBase()
        {
            this._proxy = null;
            this._msg = null;
            this._thPool = null;
        }

        internal void Init(ClsRemote remoteProxy, IMessage msg, IThreadPool thPool, IDatabase database, IConItem conItem, IEngine engine)
        {
#if DEBUG
            msg.DevMsg("ClsPluginBase:: Loading init...\n");
#endif
            this._proxy = remoteProxy;
            this._msg = msg;
            this._thPool = thPool;
            this._db = database;
            this._conItem = conItem;
            this._engine = engine;
#if DEBUG
            this._msg.DevMsg("ClsPluginBase:: Loading code...\n");
#endif
            this.Load();
#if DEBUG
            this._msg.DevMsg("ClsPluginBase:: Loaded...\n");
#endif
        }

        internal void UnInit()
        {
            this.Unload();
        }

        /// <summary>
        /// Called by engine when plugin is loading
        /// </summary>
        protected abstract void Load();
        /// <summary>
        /// Called by engine when plugin is unloading
        /// </summary>
        protected abstract void Unload();
        /// <summary>
        /// Called by engine when plugin may pause
        /// </summary>
        protected virtual void Pause() { }
        /// <summary>
        /// Called by engine when plugin may unpause
        /// </summary>
        protected virtual void Unpause() { }

        /// <summary>
        /// Get plugin name
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Get plugin description
        /// </summary>
        public abstract string Description { get; }

        ///// <summary>
        ///// Get players on server
        ///// </summary>
        ///// <returns>Players array</returns>
        //public IList<ClsPlayer> GetPlayers()
        //{
        //    return this._main.GetPlayers();
        //}

        /// <summary>
        /// Get the current mono runtime version
        /// </summary>
        public string MonoVersion { get { return ClsMain.GetMonoVersion(); } }
    }
}
