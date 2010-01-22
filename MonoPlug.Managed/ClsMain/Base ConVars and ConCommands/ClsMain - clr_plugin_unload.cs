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
                this.Error("Plugin UnInit() error\n");
                this.Error(ex);
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
                this.Error("Plugin ConCommandClean error\n");
                this.Error(ex);
            }

            //Clean convars
            List<ClsConvarMain> lstVars = new List<ClsConvarMain>();
            lock (this._convars)
            {
                foreach (ulong id in this._convars.Keys)
                {
                    if (this._convars[id].Plugin == plugin)
                    {
                        lstVars.Add(this._convars[id].Var);
                    }
                }
            }
            foreach (ClsConvarMain convar in lstVars)
            {
                this.UnregisterConvar(plugin, convar);
            }
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
                this.Error(ex);
            }
        }
    }
}
