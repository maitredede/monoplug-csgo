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
                        List<ClsConfigPlugin> lstToLoad = new List<ClsConfigPlugin>();

#if DEBUG
                        this._msg.DevMsg("APL: ({0})\n", "config loaded");
#endif
                        //Enlist plugins to load
                        if (this._config != null && this._config.Plugin != null && this._config.Plugin.Count > 0)
                        {
#if DEBUG
                            this._msg.DevMsg("APL: ({0})\n", "adding plugin to load");
#endif
                            foreach (ClsConfigPlugin confPlug in this._config.Plugin)
                            {
                                if (confPlug.Loaded)
                                {
                                    lstToLoad.Add(confPlug);
                                }
                            }
#if DEBUG
                            this._msg.DevMsg("APL: ({0})\n", "to load : " + lstToLoad.Count);
#endif
                        }

                        //Do plugin load
                        foreach (ClsConfigPlugin plug in lstToLoad)
                        {
                            ClsPluginBase plugin;
#if DEBUG
                            this._msg.DevMsg("APL: ({0})\n", plug.Name);
                            bool ok =
#endif
 this.LoadPlugin(plug.Name, out plugin);
#if DEBUG
                            this._msg.DevMsg("APL: ({0} : {1})\n", plug.Name, ok);
#endif
                        }
                    }
                    else
                    {
#if DEBUG
                        this._msg.DevMsg("APL: ({0})\n", "no config");
#endif
                        //Create default config
                        this._config = new ClsConfig();
                    }

#if DEBUG
                    this._msg.DevMsg("APL: ({0})\n", "add missing");
#endif
                    //Add file plugins missing from config
                    if (this._config.Plugin == null)
                    {
#if DEBUG
                        this._msg.DevMsg("APL: ({0})\n", "new list");
#endif
                        this._config.Plugin = new List<ClsConfigPlugin>();
                    }
                    foreach (PluginDefinition def in this._pluginCache)
                    {
                        bool exists = false;
                        foreach (ClsConfigPlugin conf in this._config.Plugin)
                        {
                            if (conf.Name.Equals(def.Name, StringComparison.InvariantCultureIgnoreCase))
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
                            ClsConfigPlugin conf = new ClsConfigPlugin();
                            conf.Name = def.Name;
                            conf.Loaded = false;
                            this._config.Plugin.Add(conf);
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
