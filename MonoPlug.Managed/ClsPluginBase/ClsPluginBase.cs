
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
            main.Msg("ClsPluginBase:: Init in AppDomain '{0}'\n", AppDomain.CurrentDomain.FriendlyName);
#endif

            ClsMain.DumpCurrentDomainAssemblies(main);
            this._main = main;
            main.Msg("ClsPluginBase:: Loading...\n");
            this.Load();
            main.Msg("ClsPluginBase:: Loaded...\n");
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
        protected void Msg(string format, params object[] args)
        {
            this._main.Msg(format, args);
        }

        /// <summary>
        /// Register a convar
        /// </summary>
        /// <param name="name">Name of convar</param>
        /// <param name="help">Help text of convar</param>
        /// <param name="flags">Flags of convar</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Convar instance if success, else null.</returns>
        protected ClsConvar RegisterConvar(string name, string help, FCVAR flags, string defaultValue)
        {
            return this._main.RegisterConvar(this, name, help, flags, defaultValue);
        }

        /// <summary>
        /// Unregister a convar
        /// </summary>
        /// <param name="var">Convar instance</param>
        protected void UnregisterConvar(ClsConvar var)
        {
            this._main.UnregisterConvar(this, var);
        }

        /// <summary>
        /// Register a ConCommand
        /// </summary>
        /// <param name="name">Name of command</param>
        /// <param name="help">Help text of command</param>
        /// <param name="flags">Flags of command</param>
        /// <param name="code">Code to invoke</param>
        /// <param name="completion">Auto-completion of command</param>
        /// <returns>ConCommand instance if success, else null</returns>
        protected ClsConCommand RegisterConCommand(string name, string help, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate completion)
        {
            return this._main.RegisterConCommand(this, name, help, code, flags, completion);
        }

        /// <summary>
        /// Unregister a ConCommand
        /// </summary>
        /// <param name="command">ConCommand instance to unregister</param>
        /// <returns>True if unregister is successfull</returns>
        protected bool UnregisterConCommand(ClsConCommand command)
        {
            return this._main.UnregisterConCommand(this, command);
        }

        /// <summary>
        /// Get players on server
        /// </summary>
        /// <returns>Players array</returns>
        protected IList<ClsPlayer> GetPlayers()
        {
            return this._main.GetPlayers();
        }
    }
}
