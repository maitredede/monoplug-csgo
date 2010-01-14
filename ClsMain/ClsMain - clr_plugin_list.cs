using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        public void clr_plugin_list(string args)
        {
            if (this._pluginCache == null)
                this._pluginCache = new PluginDefinition[] { };
            this.Msg("Available : {0} plugins\n", this._pluginCache.Length);
            foreach (PluginDefinition desc in this._pluginCache)
            {
                this.Msg("  {0}\n", desc);
            }
            lock (this._plugins)
            {
                this.Msg("Loaded : {0} plugins\n", this._plugins.Count);
                foreach (AppDomain dom in this._plugins.Keys)
                {
                    this.Msg("  {0}\n", this._plugins[dom].Name);
                }
            }
        }
    }
}
