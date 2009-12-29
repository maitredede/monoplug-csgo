using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        [ConCommand("clr_plugin_list", "List available plugins", FCVAR.FCVAR_GAMEDLL)]
        public void clr_plugin_list(string args)
        {
            if (this._pluginCache == null)
                this._pluginCache = new PluginDefinition[] { };
            Msg("Found {0} plugins\n", this._pluginCache.Length);
            foreach (PluginDefinition desc in this._pluginCache)
            {
                Msg(desc.ToString() + "\n");
            }
        }
    }
}
