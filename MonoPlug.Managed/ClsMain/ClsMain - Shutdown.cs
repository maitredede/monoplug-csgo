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
                        plugin.Uninit();
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
                this._msg.Warning(ex);
            }

            //Remove internals
            ((IEngineWrapper)this).UnregisterConvar(this._clr_plugin_directory);
#if DEBUG
            ((IEngineWrapper)this).UnregisterConCommand(this._clr_test);
#endif
            ((IEngineWrapper)this).UnregisterConCommand(this._clr_plugin_list);
            ((IEngineWrapper)this).UnregisterConCommand(this._clr_plugin_refresh);
            ((IEngineWrapper)this).UnregisterConCommand(this._clr_plugin_load);
            ((IEngineWrapper)this).UnregisterConCommand(this._clr_plugin_unload);
            ((IEngineWrapper)this).UnregisterConCommand(this._clr_reload_config);
        }
    }
}
