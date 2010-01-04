﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        //[ConCommand("clr_plugin_load", "Load a CLR plugin into memory", FCVAR.FCVAR_GAMEDLL)]
        private void clr_plugin_load(string args)
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
                            Msg("clr_plugin_load: A\n");
                            dom = AppDomain.CreateDomain(plug.Name);
                            //dom.AssemblyResolve += this._asmResolve;
                            //dom.AssemblyLoad += this._asmLoad;
                            //dom.TypeResolve += this._asmTypeResolve;

                            Msg("clr_plugin_load: B\n");
                            Msg("  Assembly location : {0}\n", Assembly.GetExecutingAssembly().Location);
                            Msg("  Type fullname location : {0}\n", typeof(ClsMain).FullName);
                            ClsMain main = (ClsMain)dom.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, typeof(ClsMain).FullName);
                            Msg("clr_plugin_load: C\n");
                            plugin = main.Remote_CreatePlugin(this, plug);
                            Msg("clr_plugin_load: D\n");
                            Msg("M: Plugin inited\n");
                            plugin.Init(this);
                            Msg("clr_plugin_load: E\n");
                            Msg("Plugin '{0}' loaded\n", plug.Name);
                            this._plugins.Add(dom, plugin);
                            Msg("clr_plugin_load: F\n");
                        }
                        catch (NullReferenceException ex)
                        {
                            Msg("Can't load plugin '{0}' : {1}\n", args, ex.Message);
                            Msg("{0}\n", ex.StackTrace);
                            if (dom != null)
                                AppDomain.Unload(dom);
                            plugin = null;
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

        private Assembly _asmResolve(object s, ResolveEventArgs a)
        {
            Msg("Assembly Resolve : asm='{0}'\n", a.Name);
            return Assembly.Load(a.Name);
        }

        private void _asmLoad(object s, AssemblyLoadEventArgs a)
        {
            Msg("Assembly Load : asm='{0}'\n", a.LoadedAssembly.FullName);
        }

        private Assembly _asmTypeResolve(object s, ResolveEventArgs a)
        {
            Msg("Assembly TypeResolve : asm='{0}'\n", a.Name);
            return Assembly.Load(a.Name);
        }
    }
}