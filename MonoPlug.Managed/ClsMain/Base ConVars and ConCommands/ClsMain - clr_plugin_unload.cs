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
                while (ex != null)
                {
                    this.Msg("Plugin unload error : {0}\n", ex.GetType().FullName);
                    this.Msg("Plugin unload error : {0}\n", ex.Message);
                    this.Msg("Plugin unload error : {0}\n", ex.StackTrace);
                    ex = ex.InnerException;
                }
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
                while (ex != null)
                {
                    this.Msg("Plugin unload error : {0}\n", ex.GetType().FullName);
                    this.Msg("Plugin unload error : {0}\n", ex.Message);
                    this.Msg("Plugin unload error : {0}\n", ex.StackTrace);
                    ex = ex.InnerException;
                }
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
            NativeMethods.Mono_Msg("M: clr_plugin_unload 0\n");
            try
            {
                NativeMethods.Mono_Msg("M: clr_plugin_unload A\n");
                //Find if plugin is loaded
                bool found = false;
                this._lckPlugins.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    foreach (AppDomain dom in this._plugins.Keys)
                    {
                        NativeMethods.Mono_Msg("M: clr_plugin_unload B\n");
                        ClsPluginBase plugin = this._plugins[dom];
                        Msg("M: clr_plugin_unload C {0}\n", plugin.Name);
                        if (plugin.Name.Equals(line, StringComparison.InvariantCultureIgnoreCase))
                        {
                            found = true;
                            NativeMethods.Mono_Msg("M: clr_plugin_unload D\n");

                            this.UnloadPlugin(dom, plugin);

                            NativeMethods.Mono_Msg("M: clr_plugin_unload I\n");
                            //Clean plugin
                            string plugname = plugin.Name;
                            this._plugins.Remove(dom);
                            NativeMethods.Mono_Msg("M: clr_plugin_unload J\n");
                            this._plugins.Remove(dom);
                            AppDomain.Unload(dom);

                            Msg("Plugin '{0}' unloaded\n", plugname);
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
                    Msg("Plugin '{0}' not found or loaded\n", line);
                }
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    this.Msg(ex.GetType().FullName + "\n");
                    this.Msg(ex.Message + "\n");
                    this.Msg(ex.StackTrace + "\n");
                    ex = ex.InnerException;
                }
            }
        }
    }
}
