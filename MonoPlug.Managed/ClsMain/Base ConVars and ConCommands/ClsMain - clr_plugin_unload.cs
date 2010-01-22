using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        private void UnloadPlugin(AppDomain dom, ClsPluginBase plugin)
        {
            if (plugin == null)
            {
                return;
            }
            //Deinit plugin
            try
            {
                plugin.UnInit();
            }
            catch (Exception ex)
            {
                this.Warning("Plugin UnInit() error\n");
                this.Warning(ex);
            }

            //Clean commands
            try
            {
                List<ClsConCommand> lstCommands = new List<ClsConCommand>();
                lock (this._concommands)
                {
                    foreach (string name in this._concommands.Keys)
                    {
                        if (this._concommands[name].Plugin == plugin)
                        {
                            lstCommands.Add(this._concommands[name].Command);
                        }
                    }
                }
                foreach (ClsConCommand cmd in lstCommands)
                {
                    this.UnregisterConCommand(plugin, cmd);
                }
            }
            catch (Exception ex)
            {
                this.Warning("Plugin ConCommandClean error\n");
                this.Warning(ex);
            }

            //Clean convars
            //List<ClsConvarMain> lstVars = new List<ClsConvarMain>();
            this._lckConvars.AcquireWriterLock(Timeout.Infinite);
            try
            {
                List<ulong> lst = new List<ulong>();
                foreach (ulong id in this._convarsList.Keys)
                {
                    ConVarEntry entry = this._convarsList[id];
                    if (entry.Plugin == plugin)
                    {
                        this.InterThreadCall<bool, ulong>(this.UnregisterConvar_Call, entry.Var.NativeID);
                        lst.Add(id);
                    }
                }
                foreach (ulong id in lst)
                {
                    this._convarsList.Remove(id);
                }
            }
            finally
            {
                this._lckConvars.ReleaseWriterLock();
            }
            //lock (this._convars)
            //{
            //    foreach (ulong id in this._convarsList.Keys)
            //    {
            //        if (this._convarsList[id].Plugin == plugin)
            //        {
            //            lstVars.Add(this._convarsList[id].Var);
            //        }
            //    }
            //}
            //foreach (ClsConvarMain convar in lstVars)
            //{
            //    this.UnregisterConvar(plugin, convar);
            //}
        }

        private void clr_plugin_unload(string line, string[] arguments)
        {
            NativeMethods.Mono_DevMsg("M: clr_plugin_unload 0\n");
            try
            {
                this.DevMsg("M: clr_plugin_unload A\n");
                //Find if plugin is loaded
                bool found = false;
                this._lckPlugins.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    foreach (AppDomain dom in this._plugins.Keys)
                    {
                        this.DevMsg("M: clr_plugin_unload B\n");
                        ClsPluginBase plugin = this._plugins[dom];
                        this.DevMsg("M: clr_plugin_unload C {0}\n", plugin.Name);
                        if (plugin.Name.Equals(line, StringComparison.InvariantCultureIgnoreCase))
                        {
                            found = true;
                            this.DevMsg("M: clr_plugin_unload D\n");

                            this.UnloadPlugin(dom, plugin);

                            this.DevMsg("M: clr_plugin_unload I\n");
                            //Clean plugin
                            string plugname = plugin.Name;
                            this._plugins.Remove(dom);
                            this.DevMsg("M: clr_plugin_unload J\n");
                            this._plugins.Remove(dom);
                            AppDomain.Unload(dom);

                            this.Msg("Plugin '{0}' unloaded\n", plugname);
                            break;
                        }
                    }
                }
                finally
                {
                    this._lckPlugins.ReleaseWriterLock();
                }
                if (!found)
                {
                    this.Msg("Plugin '{0}' not found or loaded\n", line);
                }
            }
            catch (Exception ex)
            {
                this.Warning(ex);
            }
        }
    }
}
