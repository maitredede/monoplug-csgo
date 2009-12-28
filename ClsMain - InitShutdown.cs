﻿using System;
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

            Mono_Msg("M: Called ClsMain::_Init()\n");

            //Refresh plugin cache
            this.clr_plugin_refresh(string.Empty);

            this.RegisterCommand(null, "clr_test", "Test clr", this.clr_test, FCVAR.FCVAR_GAMEDLL);
            this.RegisterCommand(null, "clr_plugin_list", "List available plugins", this.clr_plugin_list, FCVAR.FCVAR_GAMEDLL);
            this.RegisterCommand(null, "clr_plugin_refresh", "Refresh plugin list", this.clr_plugin_refresh, FCVAR.FCVAR_GAMEDLL);

            this.RegisterConVarString(null, "clr_mono_version", "Get current Mono runtime version", FCVAR.FCVAR_GAMEDLL | FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_CHEAT, ClsMain.MonoVersion);
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
        }
    }
}