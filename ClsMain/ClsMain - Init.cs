using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        /// <summary>
        /// Init function for main instance 
        /// </summary>
        internal void _Init()
        {
#if DEBUG
            Mono_Msg("CB: INIT Enter\n");
            try
            {
#endif
                //Create lists
                this._lstMsg = new List<MessageEntry>();
                this._plugins = new Dictionary<AppDomain, ClsPluginBase>();

                this._ConCommands = new Dictionary<string, ConCommandEntry>();
                this._ConVarString = new Dictionary<UInt64, ConVarEntry>();

                //get current thread Id to check for interthread calls
                this._mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                this._waitIn = new System.Threading.ManualResetEvent(false);
                this._waitOut = new System.Threading.ManualResetEvent(false);

                Mono_Msg("CB: INIT A\n");
                //Refresh plugin cache
                this.clr_plugin_refresh(string.Empty);
                Mono_Msg("CB: INIT B\n");

                //Register internals
                //this.LoadAllCommands(this);
                Mono_Msg("CB: INIT C\n");
                //this.RegisterCommand(null, "clr_plugin_list", "List available plugins", this.clr_plugin_list, FCVAR.FCVAR_GAMEDLL);
                //this.RegisterCommand(null, "clr_plugin_refresh", "Refresh plugin list", this.clr_plugin_refresh, FCVAR.FCVAR_GAMEDLL);
                this._clr_mono_version = this.RegisterConVarString(null, "clr_mono_version", "Get current Mono runtime version", FCVAR.FCVAR_GAMEDLL | FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_CHEAT, ClsMain.MonoVersion);
                this._clr_plugin_list = this.RegisterCommand(null, "clr_plugin_list", "List available plugins and loaded plugins", this.clr_plugin_list, FCVAR.FCVAR_GAMEDLL, /* this.clr_plugin_list_complete */ null);
                this._clr_plugin_refresh = this.RegisterCommand(null, "clr_plugin_refresh", "Refresh internal list of plugins", this.clr_plugin_refresh, FCVAR.FCVAR_GAMEDLL, null);
                this._clr_plugin_load = this.RegisterCommand(null, "clr_plugin_load", "Load and start a CLR plugin", this.clr_plugin_load, FCVAR.FCVAR_GAMEDLL, this.clr_plugin_load_complete);
                this._clr_plugin_unload = this.RegisterCommand(null, "clr_plugin_unload", "Unload a CLR plugin", this.clr_plugin_unload, FCVAR.FCVAR_GAMEDLL, null);
#if DEBUG
                Mono_Msg("CB: INIT D\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Mono_Msg("CB: INIT Exit\n");
            }
#endif
        }
    }
}
