using System;

namespace MonoPlug
{
    partial class ClsMain
    {
        /// <summary>
        /// Init function for main instance 
        /// </summary>
        internal bool Init()
        {
            //get current thread Id to check for interthread calls
            this._mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

            //Register base commands and vars
            //this._clr_mono_version = ((IConItemEntry)this).RegisterConvar(null, "clr_mono_version", "Get current Mono runtime version", FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_CHEAT | FCVAR.FCVAR_PRINTABLEONLY, ClsMain.GetMonoVersion());
            this._msg.DevMsg("DBG: ClsMain::Init (Begin)\n");
            this._clr_versions = ((IConItemEntry)this).RegisterConCommand("clr_versions", "Print versions of Mono and libs used for core", FCVAR.FCVAR_NONE, this.clr_versions, null, false);
            this._msg.DevMsg("DBG: ClsMain::Init (A)\n");

            this._clr_plugin_list = ((IConItemEntry)this).RegisterConCommand("clr_plugin_list", "List available plugins and loaded plugins", FCVAR.FCVAR_NONE, this.clr_plugin_list, null, true);
            this._clr_plugin_refresh = ((IConItemEntry)this).RegisterConCommand("clr_plugin_refresh", "Refresh internal list of plugins", FCVAR.FCVAR_NONE, this.clr_plugin_refresh, null, true);
            this._clr_plugin_load = ((IConItemEntry)this).RegisterConCommand("clr_plugin_load", "Load and start a CLR plugin", FCVAR.FCVAR_NONE, this.clr_plugin_load, this.clr_plugin_load_complete, true);
            this._clr_plugin_unload = ((IConItemEntry)this).RegisterConCommand("clr_plugin_unload", "Unload a CLR plugin", FCVAR.FCVAR_NONE, this.clr_plugin_unload, this.clr_plugin_load_complete, true);
            this._clr_reload_config = ((IConItemEntry)this).RegisterConCommand("clr_reload_config", "Reload config file from disk, load and unload plugins from file, and save it", FCVAR.FCVAR_NONE, this.clr_reload_config, null, true);
            this._msg.DevMsg("DBG: ClsMain::Init (B)\n");
            this._clr_plugin_directory = ((IConItemEntry)this).RegisterConvar("clr_plugin_directory", "Assembly plugin search path", FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_CHEAT | FCVAR.FCVAR_PRINTABLEONLY, this._assemblyPath);
#if DEBUG
            this._clr_test = ((IConItemEntry)this).RegisterConCommand("clr_test", "for developpement purposes only", FCVAR.FCVAR_NONE, this.clr_test, null, false);
            this._msg.DevMsg("DBG: ClsMain::Init (OK)\n");
#endif
            return true;
        }
    }
}
