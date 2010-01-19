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
        internal bool Init()
        {
#if DEBUG
            try
            {
#endif
                //get current thread Id to check for interthread calls
                this._mainThread = System.Threading.Thread.CurrentThread;

                //Refresh plugin cache
                this.clr_plugin_refresh(string.Empty, null);

                //Register base commands and vars
                this._clr_mono_version = this.RegisterConvar(null, "clr_mono_version", "Get current Mono runtime version", FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_CHEAT, ClsMain.MonoVersion);
                this._clr_plugin_list = this.RegisterConCommand(null, "clr_plugin_list", "List available plugins and loaded plugins", this.clr_plugin_list, FCVAR.FCVAR_NONE, null);
                this._clr_plugin_refresh = this.RegisterConCommand(null, "clr_plugin_refresh", "Refresh internal list of plugins", this.clr_plugin_refresh, FCVAR.FCVAR_NONE, null);
                this._clr_plugin_load = this.RegisterConCommand(null, "clr_plugin_load", "Load and start a CLR plugin", this.clr_plugin_load, FCVAR.FCVAR_NONE, this.clr_plugin_load_complete);
                this._clr_plugin_unload = this.RegisterConCommand(null, "clr_plugin_unload", "Unload a CLR plugin", this.clr_plugin_unload, FCVAR.FCVAR_NONE, this.clr_plugin_load_complete);
#if DEBUG
                this._clr_test = this.RegisterConCommand(null, "clr_test", "for developpement purposes only", this.clr_test, FCVAR.FCVAR_NONE, null);
#endif
                return true;
#if DEBUG
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
            finally
            {
            }
#endif
        }
    }
}
