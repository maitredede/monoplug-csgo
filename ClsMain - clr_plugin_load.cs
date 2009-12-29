using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MonoPlug
{
    partial class ClsMain
    {
        [ConCommand("clr_plugin_load", "Load a CLR plugin into memory", FCVAR.FCVAR_GAMEDLL)]
        private void clr_plugin_load(string args)
        {
            InternalActionDelegate d = () =>
            {
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
                                ClsPluginBase plugin = main.Remote_CreatePlugin(plug);
                                this._plugins.Add(dom, plugin);
                                plugin.Init(this);
                                Msg("Plugin '{0}' loaded\n", plug.Name);
                            }
                            catch (Exception ex)
                            {
                                if (dom != null)
                                    AppDomain.Unload(dom);
                                Msg("Can't load plugin : {0} : {1}\n", args, ex.Message);
                            }
                            break;
                        }
                    }
                }

                Msg("Can't find plugin type : {0}\n", args);
            };

            this.InterthreadCall(d);
        }
    }
}
