
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MonoPlug
{
    /// <summary>
    /// Base class for plugins
    /// </summary>
    public abstract partial class ClsPluginBase : MarshalByRefObject
    {
        private ClsMain _main;

        internal void Init(ClsMain main)
        {
#if DEBUG
            main.DevMsg("ClsPluginBase:: Init in AppDomain '{0}'\n", AppDomain.CurrentDomain.FriendlyName);
            ClsMain.DumpCurrentDomainAssemblies(main);
#endif
            this._main = main;
#if DEBUG
            main.DevMsg("ClsPluginBase:: Loading...\n");
#endif
            this.Load();
#if DEBUG
            ClsMain.DumpCurrentDomainAssemblies(main);
            main.DevMsg("ClsPluginBase:: Loaded...\n");
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

        /// <summary>
        /// Default constructor
        /// </summary>
        public ClsPluginBase()
        {
        }

        /// <summary>
        /// Write a message to the console
        /// </summary>
        /// <param name="format">Message format</param>
        /// <param name="args">Arguments of format</param>
        public void Msg(string format, params object[] args)
        {
            this._main.Msg(format, args);
        }

        /// <summary>
        /// Write a message to the developper console
        /// </summary>
        /// <param name="format">Message format</param>
        /// <param name="args">Arguments of format</param>
        public void DevMsg(string format, params object[] args)
        {
            this._main.DevMsg(format, args);
        }

        /// <summary>
        /// Get players on server
        /// </summary>
        /// <returns>Players array</returns>
        public IList<ClsPlayer> GetPlayers()
        {
            return this._main.GetPlayers();
        }

        public string MonoVersion { get { return this._main.MonoVersion; } }
    }
}
