using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        public void clr_plugin_list(string line, string[] arguments)
        {
            if (this._pluginCache == null)
                this._pluginCache = new PluginDefinition[] { };
            this.Msg("Assembly path : {0}\n", this.GetAssemblyDirectory());
            this.Msg("Available     : {0} plugins\n", this._pluginCache.Length);
            foreach (PluginDefinition desc in this._pluginCache)
            {
                this.Msg("  {0}\n", desc);
            }

            this._lckPlugins.AcquireReaderLock(Timeout.Infinite);
            try
            {
                this.Msg("Loaded : {0} plugins\n", this._plugins.Count);
                foreach (AppDomain dom in this._plugins.Keys)
                {
                    this.Msg("  {0}\n", this._plugins[dom].Name);
                }
            }
            finally
            {
                this._lckPlugins.ReleaseReaderLock();
            }
        }
    }
}
