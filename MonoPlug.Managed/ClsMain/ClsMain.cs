using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

namespace MonoPlug
{
    /// <summary>
    /// Main class that handle everything
    /// </summary>
    internal sealed partial class ClsMain : ObjectBase
    {
        #region Private fields
        /// <summary>
        /// Plugin instanciated and running 
        /// </summary>
        private readonly Dictionary<AppDomain, ClsPluginBase> _plugins = new Dictionary<AppDomain, ClsPluginBase>();
        /// <summary>
        /// Plugin dictionnary lock
        /// </summary>
        private readonly ReaderWriterLock _lckPlugins = new ReaderWriterLock();

        /// <summary>
        /// ConCommandBase lock
        /// </summary>
        private readonly ReaderWriterLock _lckConCommandBase = new ReaderWriterLock();
        /// <summary>
        /// Convars and concommands
        /// </summary>
        private readonly Dictionary<UInt64, InternalConbase> _conCommandBase = new Dictionary<ulong, InternalConbase>();


        /// <summary>
        /// Available plugin cache list 
        /// </summary>
        private PluginDefinition[] _pluginCache = null;

        private readonly ReaderWriterLock _lckConfig = new ReaderWriterLock();
        private TConfig _config;
        private bool _configLoadedOK;

        private readonly string _assemblyPath;

        //Internal commands and vars
        private InternalConCommand _clr = null;
        private InternalConvar _clr_plugin_directory = null;
        #endregion

        /// <summary>
        /// Thread pool engine
        /// </summary>
        private readonly ClsThreadPool _thPool;
        private readonly IMessage _msg;
        private readonly ClsMainConvarValue _cvarValue;

        public ClsMain()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            }
            catch (Exception ex)
            {
                NativeMethods.Mono_Msg(ex.GetType().FullName + "\n");
                NativeMethods.Mono_Msg(ex.Message + "\n");
                NativeMethods.Mono_Msg(ex.StackTrace + "\n");
            }

            this._assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            this._msg = (IMessage)this;
            this._thPool = new ClsThreadPool(this._msg);
            this._cvarValue = new ClsMainConvarValue(this);
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName an = new AssemblyName(args.Name);
            string codebase = this.GetType().Assembly.Location;
            string asm = this.GetType().Assembly.GetName(false).Name;
            int pos = codebase.LastIndexOf(asm);
            string newCB = codebase.Remove(pos, asm.Length).Insert(pos, an.Name);
            Assembly assembly = Assembly.LoadFrom(newCB);
            return assembly;
        }
    }
}