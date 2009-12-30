using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        [ConCommand("clr_plugin_load", "Load a CLR plugin into memory", FCVAR.FCVAR_GAMEDLL)]
        private void clr_plugin_load(string args)
        {
            ThreadStart d = () =>
            {
                ClsPluginBase plugin = null;
                //Search if plugin is not already loaded
                lock (this._plugins)
                {
                    foreach (AppDomain dom in this._plugins.Keys)
                    {
                        if (this._plugins[dom].Name.Equals(args, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Msg("Plugin already loaded\n");
                            return;
                        }
                    }

                    //Plugin not loaded, searching from cache
                    foreach (PluginDefinition plug in this._pluginCache)
                    {
                        if (plug.Name.Equals(args, StringComparison.InvariantCultureIgnoreCase))
                        {
                            AppDomain dom = null;
                            try
                            {
                                dom = AppDomain.CreateDomain(plug.Name);
                                ClsMain main = (ClsMain)dom.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().CodeBase, typeof(ClsMain).FullName);
                                plugin = main.Remote_CreatePlugin(this, plug);
                                Msg("M: Plugin inited\n");
                                plugin.Init(this);
                                Msg("Plugin '{0}' loaded\n", plug.Name);
                                this._plugins.Add(dom, plugin);
                            }
                            catch (Exception ex)
                            {
                                Msg("Can't load plugin '{0}' : {1}\n", args, ex.Message);
                                Msg("{0}\n", ex.StackTrace);
                                if (dom != null)
                                    AppDomain.Unload(dom);
                                plugin = null;
                            }
                            break;
                        }
                    }
                }

                if (plugin == null)
                {
                    Msg("Can't find or load plugin type : {0}\n", args);
                }
            };

            this.InterThreadCall(d);
        }
    }
}
