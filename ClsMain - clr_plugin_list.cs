using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        private void clr_plugin_list(string args)
        {
            Mono_Msg(string.Format("Found {0} plugins\n", this._pluginCache.Length));
            foreach (PluginDefinition desc in this._pluginCache)
            {
                Mono_Msg(desc.ToString() + "\n");
            }
        }
    }
}
