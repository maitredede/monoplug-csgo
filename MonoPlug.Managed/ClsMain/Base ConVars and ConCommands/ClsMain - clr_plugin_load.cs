using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.IO;

namespace MonoPlug
{
    partial class ClsMain
    {
        private void clr_plugin_load(string line, string[] arguments)
        {
            ClsPluginBase plugin = null;
            //Search if plugin is not already loaded
            this._lckPlugins.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (AppDomain dom in this._plugins.Keys)
                {
                    if (this._plugins[dom].Name.Equals(line, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.Msg("Plugin already loaded\n");
                        return;
                    }
                }

                //Plugin not loaded, searching from cache
                foreach (PluginDefinition pluginDef in this._pluginCache)
                {
                    if (pluginDef.Name.Equals(line, StringComparison.InvariantCultureIgnoreCase))
                    {
                        AppDomain dom = null;
                        try
                        {
                            ClsMain remoteProxy;
                            dom = this.CreateAppDomain(pluginDef.Name, out remoteProxy);
                            plugin = remoteProxy.Remote_CreatePlugin(this, pluginDef);
                            this.Msg("Plugin init...\n");
                            plugin.Init(this);

                            this.Msg("Plugin '{0}' loaded\n", pluginDef.Name);
                            LockCookie cookie = this._lckPlugins.UpgradeToWriterLock(Timeout.Infinite);
                            try
                            {
                                this._plugins.Add(dom, plugin);
                            }
                            finally
                            {
                                this._lckPlugins.DowngradeFromWriterLock(ref cookie);
                            }
                        }
                        catch (NullReferenceException ex)
                        {
                            this.Msg("Can't load plugin (NullReferenceException) '{0}' : {1}\n", line, ex.Message);
                            this.Msg("{0}\n", ex.StackTrace);
                            this.UnloadPlugin(dom, plugin);
                            if (dom != null)
                            {
                                AppDomain.Unload(dom);
                            }
                            plugin = null;
                        }
                        catch (FileNotFoundException ex)
                        {
                            this.Msg("Can't load plugin (FileNotFoundException) '{0}' : {1}\n", line, ex.Message);
                            this.Msg("{0}\n", ex.StackTrace);
                            this.Msg("File was : {0}\n", ex.FileName);
                            this.UnloadPlugin(dom, plugin);
                            if (dom != null)
                            {
                                AppDomain.Unload(dom);
                            }
                            plugin = null;
                        }
                        catch (Exception ex)
                        {
                            while (ex != null)
                            {
                                this.Msg("Can't load plugin ({0}) '{1}' : {2}\n", ex.GetType().Name, line, ex.Message);
                                this.Msg("{0}\n", ex.StackTrace);
                                ex = ex.InnerException;
                            }
                            this.UnloadPlugin(dom, plugin);
                            if (dom != null)
                            {
                                AppDomain.Unload(dom);
                            }
                            plugin = null;
                        }
                        break;
                    }
                }
            }
            finally
            {
                this._lckPlugins.ReleaseReaderLock();
            }
        }

        private string[] clr_plugin_load_complete(string partial)
        {
            //Msg("clr_plugin_load_complete : {0}\n", partial);
            if (this._pluginCache == null)
                this._pluginCache = new PluginDefinition[] { };
            List<string> lst = new List<string>();

            string[] args = partial.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 2)
            {
                if (string.IsNullOrEmpty(args[1]))
                {
                    foreach (PluginDefinition desc in this._pluginCache)
                    {
                        lst.Add(string.Format("{0} {1}", args[0], desc.Name));
                    }
                }
                else
                {
                    foreach (PluginDefinition desc in this._pluginCache)
                    {
                        if (desc.Name.ToUpperInvariant().Contains(args[1].ToUpperInvariant()))
                        {
                            lst.Add(string.Format("{0} {1}", args[0], desc.Name));
                        }
                    }
                }
            }
            return lst.ToArray();
        }
    }
}
