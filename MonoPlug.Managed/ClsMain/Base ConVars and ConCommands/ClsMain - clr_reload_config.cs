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
                    this._configLoadedOK = false;
                    try
                    {
                        string path = Path.Combine(this.GetAssemblyDirectory(), "config.xml");
                        this._msg.Msg("Loading config file : {0}\n", path);
                        using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            XmlTextReader xr = new XmlTextReader(fs);
                            XmlSerializer xz = new XmlSerializer(typeof(ClsConfig));
                            if (!xz.CanDeserialize(xr))
                            {
                                this._msg.Warning("Can't deserialize file\n");
                            }
                            else
                            {
                                fs.Seek(0, SeekOrigin.Begin);
                                this._config = (ClsConfig)xz.Deserialize(fs);
                                this._configLoadedOK = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this._msg.Warning(ex);
                        this._configLoadedOK = false;
                        this._config = null;
                    }
                }
                finally
                {
                    this._lckConfig.DowngradeFromWriterLock(ref cookie);
                }

                this.clr_plugin_refresh(string.Empty, null);

                if (this._configLoadedOK && this._config != null && this._config.Plugin != null)
                {
                    //Restore plugin load state
                    this._lckPlugins.AcquireWriterLock(Timeout.Infinite);
                    try
                    {
                        //Try to un/load plugins from config
                        List<string> lstHandled = new List<string>();
                        foreach (ClsConfigPlugin configPlugin in this._config.Plugin)
                        {
                            //Find there is a definition
                            bool found = false;
                            PluginDefinition def = new PluginDefinition();
                            foreach (PluginDefinition definition in this._pluginCache)
                            {
                                if (definition.Name.Equals(configPlugin.Name, StringComparison.InvariantCultureIgnoreCase))
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

                                if (loaded && !configPlugin.Loaded)
                                {
                                    //Unload plugin
                                    this._thPool.QueueUserWorkItem<string, string[]>(this.clr_plugin_unload, configPlugin.Name, null);
                                }

                                if (!loaded && configPlugin.Loaded)
                                {
                                    //load plugin
                                    this._thPool.QueueUserWorkItem<string, string[]>(this.clr_plugin_load, configPlugin.Name, null);
                                }
                            }

                            lstHandled.Add(configPlugin.Name);
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
