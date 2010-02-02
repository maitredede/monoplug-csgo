using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void AllPluginsLoaded()
        {
            try
            {
                //No need to lock, called synchronously from engine, without threaded managed plugin

#if DEBUG
                this._msg.DevMsg("APL: ({0})\n", "enter");
#endif
                //Get plugins from disk
                if (this.RefreshPluginCache())
                {
                    //this._clr_plugin_refresh.Execute(string.Empty, null, false);
                    //Load config file
#if DEBUG
                    this._msg.DevMsg("APL: ({0})\n", "load config");
#endif
                    this._configLoadedOK = this.LoadConfigNoLock(out this._config);
                    if (this._configLoadedOK)
                    {
                        List<TPlugin> lstToLoad = new List<TPlugin>();

#if DEBUG
                        this._msg.DevMsg("APL: ({0})\n", "config loaded");
#endif
                        //Enlist plugins to load
                        if (this._config != null && this._config.plugin != null && this._config.plugin.Count > 0)
                        {
#if DEBUG
                            this._msg.DevMsg("APL: ({0})\n", "adding plugin to load");
#endif
                            foreach (TPlugin confPlug in this._config.plugin)
                            {
                                if (confPlug.loaded)
                                {
                                    lstToLoad.Add(confPlug);
                                }
                            }
#if DEBUG
                            this._msg.DevMsg("APL: ({0})\n", "to load : " + lstToLoad.Count);
#endif
                        }

                        //Do plugin load
                        foreach (TPlugin plug in lstToLoad)
                        {
                            ClsPluginBase plugin;
#if DEBUG
                            this._msg.DevMsg("APL: ({0})\n", plug.name);
                            bool ok =
#endif
 this.LoadPlugin(plug.name, out plugin);
#if DEBUG
                            this._msg.DevMsg("APL: ({0} : {1})\n", plug.name, ok);
#endif
                        }
                    }
                    else
                    {
#if DEBUG
                        this._msg.DevMsg("APL: ({0})\n", "no config");
#endif
                        //Create default config
                        this._config = new TConfig();
                    }

#if DEBUG
                    this._msg.DevMsg("APL: ({0})\n", "add missing");
#endif
                    //Add file plugins missing from config
                    if (this._config.plugin == null)
                    {
#if DEBUG
                        this._msg.DevMsg("APL: ({0})\n", "new list");
#endif
                        this._config.plugin = new List<TPlugin>();
                    }
                    foreach (PluginDefinition def in this._pluginCache)
                    {
                        bool exists = false;
                        foreach (TPlugin conf in this._config.plugin)
                        {
                            if (conf.name.Equals(def.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists)
                        {
#if DEBUG
                            this._msg.DevMsg("APL: ({0} : {1})\n", "missing", def.Name);
#endif
                            TPlugin conf = new TPlugin();
                            conf.name = def.Name;
                            conf.loaded = false;
                            this._config.plugin.Add(conf);
                        }
                    }
#if DEBUG
                    this._msg.DevMsg("APL: ({0})\n", "saving config");
#endif
                    this.SaveConfigNoLock(this._config);
#if DEBUG
                    this._msg.DevMsg("APL: ({0})\n", "config saved");
#endif
                }
#if DEBUG
                else
                {
                    this._msg.DevMsg("APL: Can't refresh plugin list\n");
                }
#endif
            }
            catch (Exception ex)
            {
                this._msg.Warning(ex);
            }
#if DEBUG
            finally
            {
                this._msg.DevMsg("APL: ({0})\n", "exit");
            }
#endif
        }
    }
}
