using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
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
                Msg(ex.GetType().FullName + "\n");
                Msg(ex.Message + "\n");
                Msg(ex.StackTrace + "\n");
            }

            //Remove internals
            this.UnregisterConVarString(null, this._clr_mono_version);
            this.UnregisterCommand(null, this._clr_plugin_list);
            this.UnregisterCommand(null, this._clr_plugin_refresh);
            this.UnregisterCommand(null, this._clr_plugin_load);
            this.UnregisterCommand(null, this._clr_plugin_unload);
        }
    }
}
