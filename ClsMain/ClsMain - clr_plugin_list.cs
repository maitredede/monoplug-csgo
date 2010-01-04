using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        //[ConCommand("clr_plugin_list", "List available plugins", FCVAR.FCVAR_GAMEDLL)]
        public void clr_plugin_list(string args)
        {
            if (this._pluginCache == null)
                this._pluginCache = new PluginDefinition[] { };
            Msg("Available : {0} plugins\n", this._pluginCache.Length);
            foreach (PluginDefinition desc in this._pluginCache)
            {
                Msg("  {0}\n", desc);
            }
            lock (this._plugins)
            {
                Msg("Loaded : {0} plugins\n", this._plugins.Count);
                foreach (AppDomain dom in this._plugins.Keys)
                {
                    Msg("  {0}\n", this._plugins[dom].Name);
                }
            }
        }

        //public string[] clr_plugin_list_complete(string partial)
        //{
        //    if (this._pluginCache == null)
        //        this._pluginCache = new PluginDefinition[] { };
        //    List<string> lst = new List<string>();

        //    if (!string.IsNullOrEmpty(partial))
        //    {
        //        foreach (PluginDefinition desc in this._pluginCache)
        //        {
        //            if (desc.Name.ToUpperInvariant().Contains(partial.ToUpperInvariant()))
        //            {
        //                lst.Add(desc.Name);
        //            }
        //        }
        //    }
        //    return lst.ToArray();
        //}
    }
}
