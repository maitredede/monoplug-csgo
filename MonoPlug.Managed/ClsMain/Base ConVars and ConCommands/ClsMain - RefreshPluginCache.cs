using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;

namespace MonoPlug
{
    partial class ClsMain
    {
        //private void clr_plugin_refresh(ClsPlayer sender, string line, string[] arguments)
        //{
        //    this.RefreshPluginCache();
        //}

        private bool RefreshPluginCache()
        {
            bool ok = false;
            AppDomain dom = null;
            try
            {
                //Create another domain to gather plugin data
                ClsProxy proxy;
                dom = this.CreateAppDomain("MonoPlug_ScanPlugins", out proxy);
                this._pluginCache = proxy.GetPluginsFromDirectory(this._msg, this._assemblyPath);
                ok = true;
            }
            catch (Exception ex)
            {
                ok = false;
                this._pluginCache = new PluginDefinition[] { };
                this._msg.Warning(ex);
            }
            finally
            {
                if (dom != null)
                {
                    //Destroy remote domain
                    AppDomain.Unload(dom);
                }
            }
            return ok;
        }
    }
}
