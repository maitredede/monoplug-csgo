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
            this._lckPlugins.AcquireWriterLock(Timeout.Infinite);
            try
            {
                foreach (AppDomain dom in this._plugins.Keys)
                {
                    if (this._plugins[dom].Name.Equals(line, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Msg("Plugin already loaded\n");
                        return;
                    }
                }

                //Plugin not loaded, searching from cache
                foreach (PluginDefinition plug in this._pluginCache)
                {
                    if (plug.Name.Equals(line, StringComparison.InvariantCultureIgnoreCase))
                    {
                        AppDomain dom = null;
                        try
                        {
                            dom = AppDomain.CreateDomain(plug.Name);
                            Msg("  Assembly location : {0}\n", Assembly.GetExecutingAssembly().Location);
                            Msg("  Type fullname location : {0}\n", typeof(ClsMain).FullName);
                            ClsMain main = (ClsMain)dom.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, typeof(ClsMain).FullName);
                            plugin = main.Remote_CreatePlugin(this, plug);
                            this.Msg("  Remote .ctor OK\n");
                            plugin.Init(this);
                            Msg("Plugin '{0}' loaded\n", plug.Name);
                            this._plugins.Add(dom, plugin);
                        }
                        catch (NullReferenceException ex)
                        {
                            Msg("Can't load plugin (NullReferenceException) '{0}' : {1}\n", line, ex.Message);
                            Msg("{0}\n", ex.StackTrace);
                            if (dom != null)
                                AppDomain.Unload(dom);
                            plugin = null;
                        }
                        catch (FileNotFoundException ex)
                        {
                            this.Msg("Can't load plugin (FileNotFoundException) '{0}' : {1}\n", line, ex.Message);
                            this.Msg("{0}\n", ex.StackTrace);
                            this.Msg("File was : {0}\n", ex.FileName);
                            if (dom != null)
                                AppDomain.Unload(dom);
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
                            if (dom != null)
                                AppDomain.Unload(dom);
                            plugin = null;
                        }
                        break;
                    }
                }
            }
            finally
            {
                this._lckPlugins.ReleaseWriterLock();
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
