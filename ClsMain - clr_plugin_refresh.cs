using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        private void clr_plugin_refresh(string args)
        {
            this._pluginCache = this.GetPlugins();
            Mono_Msg(string.Format("Refreshed {0} plugins\n", this._pluginCache.Length));
        }
    }
}
