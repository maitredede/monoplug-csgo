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
            //Create lists
            this._lstMsg = new List<MessageEntry>();
            this._plugins = new Dictionary<AppDomain, ClsPluginBase>();

            this._commands = new Dictionary<string, ConCommandEntry>();
            this._varString = new Dictionary<UInt64, ConVarEntry>();

            //get current thread Id to check for interthread calls
            this._mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

            //Refresh plugin cache
            this.clr_plugin_refresh(string.Empty);

            //Register internals
            this.LoadAllCommands(this);
            //this.RegisterCommand(null, "clr_plugin_list", "List available plugins", this.clr_plugin_list, FCVAR.FCVAR_GAMEDLL);
            //this.RegisterCommand(null, "clr_plugin_refresh", "Refresh plugin list", this.clr_plugin_refresh, FCVAR.FCVAR_GAMEDLL);
            this._clr_mono_version = this.RegisterConVarString(null, "clr_mono_version", "Get current Mono runtime version", FCVAR.FCVAR_GAMEDLL | FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_CHEAT, ClsMain.MonoVersion);
        }

        static void DumpThreadId()
        {
            string msg = string.Format("M: Thread Id is {0}\n", System.Threading.Thread.CurrentThread.ManagedThreadId);
            Console.Write(msg);
            //try
            //{
            //    Mono_Msg(string.Format("M: Thread Id is {0}\n", System.Threading.Thread.CurrentThread.ManagedThreadId));
            //}
            //catch (Exception ex)
            //{
            //    Mono_Msg("M: Thread Id Dump Error\n");
            //    Mono_Msg(ex.GetType().FullName + "\n");
            //    Mono_Msg(ex.Message + "\n");
            //    Mono_Msg(ex.StackTrace + "\n");
            //}
        }

        /// <summary>
        /// Callback for plugin shutdown 
        /// </summary>
        internal void _Shutdown()
        {
            try
            {
                lock (this._plugins)
                {
                    foreach (AppDomain dom in this._plugins.Keys)
                    {
                        ClsPluginBase plugin = this._plugins[dom];
                        plugin.UnInit();
                        AppDomain.Unload(dom);
                    }
                    this._plugins.Clear();
                }
            }
            catch (Exception ex)
            {
                Mono_Msg(ex.GetType().FullName + "\n");
                Mono_Msg(ex.Message + "\n");
                Mono_Msg(ex.StackTrace + "\n");
            }

            //Remove internals
            this.UnregisterConVarString(null, this._clr_mono_version);
        }
    }
}
