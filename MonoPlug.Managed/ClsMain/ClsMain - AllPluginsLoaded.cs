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
                //Get plugins from disk
                if (this.RefreshPluginCache())
                {
                    //Load config file
                    this._config = null;
                    this._configLoadedOK = this.LoadConfigNoLock(out this._config);
                    if (this._configLoadedOK)
                    {
                        List<TPlugin> lstToLoad = new List<TPlugin>();

                        //Enlist plugins to load
                        if (this._config != null && this._config.plugin != null && this._config.plugin.Count > 0)
                        {
                            foreach (TPlugin confPlug in this._config.plugin)
                            {
                                if (confPlug.loaded)
                                {
                                    lstToLoad.Add(confPlug);
                                }
                            }
                        }

                        //Do plugin load
                        foreach (TPlugin plug in lstToLoad)
                        {
                            ClsPluginBase plugin;
                            this.LoadPlugin(plug.name, out plugin);
                        }
                    }
                    else
                    {
                        //Create default config
                        this._config = new TConfig();
                    }

                    //Add file plugins missing from config
                    if (this._config.plugin == null)
                    {
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
                            TPlugin conf = new TPlugin();
                            conf.name = def.Name;
                            conf.loaded = false;
                            this._config.plugin.Add(conf);
                        }
                    }
                    this.SaveConfigNoLock(this._config);
                }
            }
            catch (Exception ex)
            {
                this._msg.Warning(ex);
            }
        }
    }
}
