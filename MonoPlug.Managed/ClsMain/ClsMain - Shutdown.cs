using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        /// <summary>
        /// Callback for plugin shutdown 
        /// </summary>
        internal void Shutdown()
        {
            try
            {
                this._lckPlugins.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    foreach (AppDomain dom in this._plugins.Keys)
                    {
                        ClsPluginBase plugin = this._plugins[dom];
                        plugin.UnInit();
                        AppDomain.Unload(dom);
                    }
                    this._plugins.Clear();
                }
                finally
                {
                    this._lckPlugins.ReleaseWriterLock();
                }
            }
            catch (Exception ex)
            {
                this.Warning(ex);
            }

            //Remove internals
            this.UnregisterConvar(null, this._clr_mono_version);
            this.UnregisterConvar(null, this._clr_plugin_directory);
            this.UnregisterConCommand(null, this._clr_test);
            this.UnregisterConCommand(null, this._clr_plugin_list);
            this.UnregisterConCommand(null, this._clr_plugin_refresh);
            this.UnregisterConCommand(null, this._clr_plugin_load);
            this.UnregisterConCommand(null, this._clr_plugin_unload);
        }
    }
}
