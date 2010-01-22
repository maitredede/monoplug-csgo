using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace MonoPlug
{
    partial class ClsMain
    {
        public void clr_plugin_refresh(string line, string[] arguments)
        {
#if DEBUG
            this.DevMsg("Entering clr_plugin_refresh...\n");
            try
            {
#endif
                AppDomain dom = null;
                try
                {
                    string path = this.GetAssemblyDirectory();
                    //Create another domain to gather plugin data
                    ClsRemote proxy;
                    dom = this.CreateAppDomain("MonoPlug_ScanPlugins", out proxy);
                    this._pluginCache = proxy.GetPluginsFromDirectory(this, path);
                }
                catch (Exception ex)
                {
                    this._pluginCache = new PluginDefinition[] { };
                    this.Warning(ex);
                }
                finally
                {
                    if (dom != null)
                    {
                        //Destroy remote domain
                        AppDomain.Unload(dom);
                    }
                }
                this.Msg("Refreshed {0} plugins\n", this._pluginCache.Length);
#if DEBUG
            }
            finally
            {
                this.Msg("Refreshed {0} plugins\n", this._pluginCache.Length);
                this.DevMsg("Exiting clr_plugin_refresh...");
            }
#endif
        }
    }
}
