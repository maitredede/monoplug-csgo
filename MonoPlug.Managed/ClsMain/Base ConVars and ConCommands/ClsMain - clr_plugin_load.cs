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
#if DEBUG
            try
            {
                this.DevMsg("DBG: Entering '{0}' in domain [{1}]...\n", "clr_plugin_load", AppDomain.CurrentDomain.FriendlyName);
#endif
                ClsPluginBase plugin = null;
                //Search if plugin is not already loaded
                this._lckPlugins.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    foreach (AppDomain dom in this._plugins.Keys)
                    {
                        if (this._plugins[dom].Name.Equals(line, StringComparison.InvariantCultureIgnoreCase))
                        {
                            this.Warning("Plugin already loaded\n");
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
                                ClsRemote remoteProxy;
                                dom = this.CreateAppDomain(pluginDef.Name, out remoteProxy);
#if DEBUG
                                this.DevMsg("DBG: Calling Remote_CreatePlugin() in [{0}] for [{1}]...\n", AppDomain.CurrentDomain, dom.FriendlyName);
#endif
                                plugin = remoteProxy.CreatePluginClass(this, this.GetAssemblyDirectory(), pluginDef);
#if DEBUG
                                this.DevMsg("DBG: Calling plugin.init() ...\n");
#endif
                                plugin.Init(this);

                                this.Msg("Plugin '{0}' loaded\n", plugin.Name);
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
                                this.Warning("Can't load plugin (NullReferenceException) '{0}'\n", line);
                                this.Warning(ex);
                                this.UnloadPlugin(dom, plugin);
                                if (dom != null)
                                {
                                    AppDomain.Unload(dom);
                                }
                                plugin = null;
                            }
                            catch (FileNotFoundException ex)
                            {
                                this.Warning("Can't load plugin (FileNotFoundException) '{0}'\n", line);
                                this.Warning("File was : {0}\n", ex.FileName);
                                this.Warning(ex);
                                this.UnloadPlugin(dom, plugin);
                                if (dom != null)
                                {
                                    AppDomain.Unload(dom);
                                }
                                plugin = null;
                            }
                            catch (Exception ex)
                            {
                                this.Warning(ex);
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
#if DEBUG
            }
            finally
            {
                this.DevMsg("DBG: Exiting '{0}'...\n", "clr_plugin_load");
            }
#endif
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
