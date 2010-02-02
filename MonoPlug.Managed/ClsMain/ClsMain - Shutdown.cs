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
            ((IConItemEntry)this).UnregisterConCommand(this._clr_versions);
            ((IConItemEntry)this).UnregisterConvar(this._clr_plugin_directory);
#if DEBUG
            ((IConItemEntry)this).UnregisterConCommand(this._clr_test);
#endif
            ((IConItemEntry)this).UnregisterConCommand(this._clr_plugin_list);
            ((IConItemEntry)this).UnregisterConCommand(this._clr_plugin_refresh);
            ((IConItemEntry)this).UnregisterConCommand(this._clr_plugin_load);
            ((IConItemEntry)this).UnregisterConCommand(this._clr_plugin_unload);
        }
    }
}
