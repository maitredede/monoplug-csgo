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
            NativeMethods.Mono_Msg("CB: clr_plugin_refresh Enter\n");
            AppDomain dom = null;
            try
            {
                //Create another domain to gather plugin data
                dom = AppDomain.CreateDomain("GetPlugins");
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                this.Msg("CLR: Assembly path is : {0}\n", path);

                //Instanciate the remote wrapper
                ClsMain main = (ClsMain)dom.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().CodeBase, typeof(ClsMain).FullName);
                this._pluginCache = main.Remote_GetPluginsFromDirectory(this, path);
            }
            catch (Exception ex)
            {
                this._pluginCache = new PluginDefinition[] { };
                this.Msg("GetPlugins Error : {0}\n", ex.Message);
                this.Msg("GetPlugins Error : {0}\n", ex.StackTrace);
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
