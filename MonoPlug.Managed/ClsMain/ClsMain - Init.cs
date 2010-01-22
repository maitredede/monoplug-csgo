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
            //get current thread Id to check for interthread calls
            this._mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

#if DEBUG
            this.DevMsg("DBG: ClsMain::Init (enter)\n");
            try
            {
#endif
                //Register base commands and vars
                this._clr_mono_version = this.RegisterConvar(null, "clr_mono_version", "Get current Mono runtime version", FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_CHEAT | FCVAR.FCVAR_PRINTABLEONLY, this.MonoVersion);
                this._clr_plugin_directory = this.RegisterConvar(null, "clr_plugin_directory", "Assembly plugin search path", FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_CHEAT | FCVAR.FCVAR_PRINTABLEONLY, this.GetAssemblyDirectory());

                this._clr_plugin_list = this.RegisterConCommand(null, "clr_plugin_list", "List available plugins and loaded plugins", this.clr_plugin_list, FCVAR.FCVAR_NONE, null);
                this._clr_plugin_refresh = this.RegisterConCommand(null, "clr_plugin_refresh", "Refresh internal list of plugins", this.clr_plugin_refresh, FCVAR.FCVAR_NONE, null);
                this._clr_plugin_load = this.RegisterConCommand(null, "clr_plugin_load", "Load and start a CLR plugin", this.clr_plugin_load, FCVAR.FCVAR_NONE, this.clr_plugin_load_complete);
                this._clr_plugin_unload = this.RegisterConCommand(null, "clr_plugin_unload", "Unload a CLR plugin", this.clr_plugin_unload, FCVAR.FCVAR_NONE, this.clr_plugin_load_complete);
#if DEBUG
                this._clr_test = this.RegisterConCommand(null, "clr_test", "for developpement purposes only", this.clr_test, FCVAR.FCVAR_NONE, null);
#endif
                //Refresh plugin cache
                this.clr_plugin_refresh(string.Empty, null);

                this.DevMsg("DBG: ClsMain::Init (C)\n");
                try
                {
                    int wrkTh;
                    int compTh;
                    System.Threading.ThreadPool.GetAvailableThreads(out wrkTh, out compTh);
                    this.DevMsg("ThreadPool Available : wrk={0} cmp={0}\n");
                    System.Threading.ThreadPool.GetMinThreads(out wrkTh, out compTh);
                    this.DevMsg("ThreadPool Minimum  : wrk={0} cmp={0}\n");
                    System.Threading.ThreadPool.GetMaxThreads(out wrkTh, out compTh);
                    this.DevMsg("ThreadPool Maximum  : wrk={0} cmp={0}\n");
                }
                catch (Exception ex)
                {
                    this.Error(ex);
                }

                this.DevMsg("DBG: ClsMain::Init (D)\n");
                return true;
#if DEBUG
            }
            catch (Exception ex)
            {
                this.Error(ex);
                return false;
            }
            finally
            {
                this.DevMsg("DBG: ClsMain::Init (exit)\n");
            }
#endif
        }
    }
}
