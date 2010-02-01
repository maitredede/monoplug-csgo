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
    internal sealed partial class ClsMain : MarshalByRefObject
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
        private readonly Dictionary<UInt64, ClsConCommandBase> _conCommandBase = new Dictionary<ulong, ClsConCommandBase>();


        /// <summary>
        /// Available plugin cache list 
        /// </summary>
        private PluginDefinition[] _pluginCache = null;

        private readonly ReaderWriterLock _lckConfig = new ReaderWriterLock();
        private ClsConfig _config;
        private bool _configLoadedOK;

        //Internal commands and vars
        private ClsConVar _clr_mono_version = null;
        private ClsConVar _clr_plugin_directory = null;
        private ClsConCommand _clr_plugin_list = null;
        private ClsConCommand _clr_plugin_refresh = null;
        private ClsConCommand _clr_plugin_load = null;
        private ClsConCommand _clr_plugin_unload = null;
        private ClsConCommand _clr_reload_config = null;
        #endregion

        /// <summary>
        /// Thread pool engine
        /// </summary>
        private readonly ClsThreadPool _thPool;
        private readonly ClsMessage _msg;
        private readonly ClsConvarValue _cvarValue;

        public ClsMain()
        {
            this._msg = new ClsMessage(this);
            this._thPool = new ClsThreadPool(this._msg);
            this._cvarValue = new ClsConvarValue(this);
        }
    }
}