﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.IO;

namespace MonoPlug
{
    partial class ClsMain
    {
        private void LoadPlugin(string name)
        {
            ClsPluginBase plugin;
            bool loaded = this.LoadPlugin(name, out plugin);
            this._lckConfig.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (this._configLoadedOK)
                {
                    bool found = false;
                    if (this._config != null && this._config.plugin != null)
                    {
                        foreach (TPlugin conf in this._config.plugin)
                        {
                            if (conf.name.Equals(plugin.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                conf.loaded = loaded;
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found && loaded)
                    {
                        if (this._config == null) this._config = new TConfig();
                        if (this._config.plugin == null) this._config.plugin = new List<TPlugin>();
                        TPlugin conf = new TPlugin();
                        conf.name = plugin.Name;
                        conf.loaded = true;
                        this._config.plugin.Add(conf);
                    }
                    this.SaveConfigNoLock(this._config);
                }
            }
            finally
            {
                this._lckConfig.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Load a plugin
        /// </summary>
        /// <param name="name">plugin name</param>
        /// <param name="plugin">plugin instance</param>
        /// <returns>True if plugin is already loaded, or if loaded successfully</returns>
        private bool LoadPlugin(string name, out ClsPluginBase plugin)
        {
            //Search if plugin is not already loaded
            this._lckPlugins.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (AppDomain dom in this._plugins.Keys)
                {
                    if (this._plugins[dom].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this._msg.Warning("Plugin already loaded\n");
                        plugin = this._plugins[dom];
                        return true;
                    }
                }

                //Plugin not loaded, searching from cache
                bool found = false;
                PluginDefinition def = new PluginDefinition();
                foreach (PluginDefinition pluginDef in this._pluginCache)
                {
                    if (pluginDef.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        def = pluginDef;
                        break;
                    }
                }

                if (found)
                {
                    bool ok = false;
                    AppDomain dom = null;
                    plugin = null;
                    try
                    {
                        ClsProxy remoteProxy;
                        dom = this.CreateAppDomain(def.Name, out remoteProxy);
                        plugin = remoteProxy.CreatePluginClass(this._msg, this._assemblyPath, def);
                        plugin.Init(remoteProxy, this._msg, this, this, this._thPool, this);

                        this._msg.Msg("Plugin '{0}' loaded\n", plugin.Name);
                        LockCookie cookie = this._lckPlugins.UpgradeToWriterLock(Timeout.Infinite);
                        try
                        {
                            this._plugins.Add(dom, plugin);
                            ok = true;
                        }
                        finally
                        {
                            this._lckPlugins.DowngradeFromWriterLock(ref cookie);
                        }
                    }
                    catch (MissingMethodException ex)
                    {
                        this._msg.Warning("Can't load plugin (MissingMethodException) '{0}'\n", name);
                        this._msg.Warning(ex);
                        if (ex.TargetSite == null)
                        {
                            this._msg.Warning("TargetSite is null\n");
                        }
                        else
                        {
                            this._msg.Warning("TargetSite: '{0}' in '{1}'\n", ex.TargetSite.Name, ex.TargetSite.DeclaringType.AssemblyQualifiedName);
                        }
                        this.UnloadPlugin(dom, plugin);
                        if (dom != null)
                        {
                            AppDomain.Unload(dom);
                        }
                        plugin = null;
                        ok = false;
                    }
                    catch (NullReferenceException ex)
                    {
                        this._msg.Warning("Can't load plugin (NullReferenceException) '{0}'\n", name);
                        this._msg.Warning(ex);
                        this.UnloadPlugin(dom, plugin);
                        if (dom != null)
                        {
                            AppDomain.Unload(dom);
                        }
                        plugin = null;
                        ok = false;
                    }
                    catch (FileNotFoundException ex)
                    {
                        this._msg.Warning("Can't load plugin (FileNotFoundException) '{0}'\n", name);
                        this._msg.Warning("File was : {0}\n", ex.FileName);
                        this._msg.Warning(ex);
                        this.UnloadPlugin(dom, plugin);
                        if (dom != null)
                        {
                            AppDomain.Unload(dom);
                        }
                        plugin = null;
                        ok = false;
                    }
                    catch (Exception ex)
                    {
                        this._msg.Warning(ex);
                        this.UnloadPlugin(dom, plugin);
                        if (dom != null)
                        {
                            AppDomain.Unload(dom);
                        }
                        plugin = null;
                        ok = false;
                    }
                    return ok;
                }
                else
                {
                    plugin = null;
                    return false;
                }
            }
            finally
            {
                this._lckPlugins.ReleaseReaderLock();
            }
        }
    }
}