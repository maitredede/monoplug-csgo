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
            //Msg("Not implemented yet :o)\n");
            try
            {
                lock (this._plugins)
                {
                    foreach (PluginDefinition plug in this._pluginCache)
                    {
                        if (plug.Type == args)
                        {
                            try
                            {
                                AppDomain dom = AppDomain.CreateDomain(plug.Name);
                                ClsMain main = (ClsMain)dom.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().CodeBase, typeof(ClsMain).FullName);
                                ClsPluginBase plugin = main.Remote_CreatePlugin(plug);
                                this._plugins.Add(dom, plugin); ;
                                plugin.Init(this);
                            }
                            catch (Exception ex)
                            {
                                Msg("Can't load plugin : {0} : {1}\n", args, ex.Message);
                            }
                            break;
                        }
                    }

                    Msg("Can't find plugin type : {0}\n", args);
                }
            }
            catch (Exception ex)
            {
                Msg("PluginLoad Exception : {0}\n", ex.GetType().FullName);
                Msg("{0}\n", ex.Message);
                Msg("{0}\n", ex.StackTrace);
            }
        }
    }
}
