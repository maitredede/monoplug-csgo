using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        [ConCommand("clr_plugin_refresh", "Refresh plugin list", FCVAR.FCVAR_GAMEDLL)]
        public void clr_plugin_refresh(string args)
        {
            this._pluginCache = this.GetPlugins();
            Msg("Refreshed {0} plugins\n", this._pluginCache.Length);
        }
    }
}
