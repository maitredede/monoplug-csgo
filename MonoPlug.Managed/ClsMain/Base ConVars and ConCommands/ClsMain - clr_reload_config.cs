using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace MonoPlug
{
    partial class ClsMain
    {
        private void clr_reload_config(string line, string[] args)
        {
            //Try to reload config file
            this._lckConfig.AcquireReaderLock(Timeout.Infinite);
            try
            {
                LockCookie cookie = this._lckConfig.UpgradeToWriterLock(Timeout.Infinite);
                try
                {
                    this._configLoadedOK = this.LoadConfigNoLock(out this._config);
                }
                finally
                {
                    this._lckConfig.DowngradeFromWriterLock(ref cookie);
                }

                this.clr_plugin_refresh(string.Empty, null);

                if (this._configLoadedOK && this._config != null && this._config.plugin != null)
                {
                    //Restore plugin load state
                    this._lckPlugins.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        //Try to un/load plugins from config
                        List<string> lstHandled = new List<string>();
                        foreach (TPlugin configPlugin in this._config.plugin)
                        {
                            //Find there is a definition
                            bool found = false;
                            PluginDefinition def = new PluginDefinition();
                            foreach (PluginDefinition definition in this._pluginCache)
                            {
                                if (definition.Name.Equals(configPlugin.name, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    found = true;
                                    def = definition;
                                    break;
                                }
                            }

                            if (found)
                            {
                                //Find if plugin is loaded
                                bool loaded = false;
                                foreach (AppDomain dom in this._plugins.Keys)
                                {
                                    if (dom.FriendlyName.Equals(def.Name, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        loaded = true;
                                        break;
                                    }
                                }

                                if (loaded && !configPlugin.loaded)
                                {
                                    //Unload plugin
                                    this._thPool.QueueUserWorkItem<string, string[]>(this.clr_plugin_unload, configPlugin.name, null);
                                }

                                if (!loaded && configPlugin.loaded)
                                {
                                    //load plugin
                                    this._thPool.QueueUserWorkItem<string, string[]>(this.clr_plugin_load, configPlugin.name, null);
                                }
                            }

                            lstHandled.Add(configPlugin.name);
                        }

                        //Unload loaded plugins not in config file
                        foreach (AppDomain dom in this._plugins.Keys)
                        {
                            bool found = false;
                            foreach (string item in lstHandled)
                            {
                                if (item.Equals(dom.FriendlyName, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    found = true;
                                    break;
                                }

                                //If plugin not in config
                                if (!found)
                                {
                                    //Unload plugin
                                    this._thPool.QueueUserWorkItem<string, string[]>(this.clr_plugin_unload, dom.FriendlyName, null);
                                }
                            }
                        }
                    }
                    finally
                    {
                        this._lckPlugins.ReleaseWriterLock();
                    }
                }
            }
            finally
            {
                this._lckConfig.ReleaseReaderLock();
            }
        }

        //private void SaveConfigNoLock()
        //{
        //    string path = Path.Combine(this.GetAssemblyDirectory(), "config.xml");
        //    using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
        //    {
        //        XmlSerializer xz = new XmlSerializer(typeof(ClsConfig));
        //        xz.Serialize(fs, this._config);
        //    }
        //}
    }
}
