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
            AppDomain dom = null;
            try
            {
                string path = this.GetAssemblyDirectory();
                this.Msg("CLR: Assembly path is : {0}\n", path);

                //Create another domain to gather plugin data
                ClsMain proxy;
                dom = this.CreateAppDomain(typeof(ClsMain).FullName, out proxy);
                this._pluginCache = proxy.Remote_GetPluginsFromDirectory(this, path);
            }
            catch (Exception ex)
            {
                this._pluginCache = new PluginDefinition[] { };
                while (ex != null)
                {
                    this.Msg("GetPlugins Error : {0}\n", ex.GetType().FullName);
                    this.Msg("GetPlugins Error : {0}\n", ex.Message);
                    this.Msg("GetPlugins Error : {0}\n", ex.StackTrace);
                    ex = ex.InnerException;
                }
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
        }
    }
}
