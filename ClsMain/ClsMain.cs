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
        /// List of awaiting Messages 
        /// </summary>
        private List<MessageEntry> _lstMsg = null;
        /// <summary>
        /// Plugin instanciated and running 
        /// </summary>
        private Dictionary<AppDomain, ClsPluginBase> _plugins = null;
        /// <summary>
        /// Available plugin cache list 
        /// </summary>
        private PluginDefinition[] _pluginCache = null;

        private Dictionary<string, ConCommandEntry> _ConCommands = null;

        private Dictionary<UInt64, ConVarEntry> _ConVarString = null;
        private ClsConVarStrings _clr_mono_version = null;
        private ClsConCommand _clr_plugin_list = null;
        private ClsConCommand _clr_plugin_refresh = null;
        private ClsConCommand _clr_plugin_load = null;
        private ClsConCommand _clr_plugin_unload = null;

        private int _mainThreadId = 0;
        private int _queueLength = 0;
        private readonly object _lckThreadSync = new object();
        private ManualResetEvent _waitIn;
        private ManualResetEvent _waitOut;

        private int _isInITCall = 0;
        #endregion

        public ClsMain()
        {
        }
    }
}