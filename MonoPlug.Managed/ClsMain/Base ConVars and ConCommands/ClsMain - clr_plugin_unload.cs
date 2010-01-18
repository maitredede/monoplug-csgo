using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        private void clr_plugin_unload(string args)
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
                        if (plugin.Name.Equals(args, StringComparison.InvariantCultureIgnoreCase))
                        {
                            found = true;
                            NativeMethods.Mono_Msg("M: clr_plugin_unload D\n");
                            //Deinit plugin
                            plugin.UnInit();

                            NativeMethods.Mono_Msg("M: clr_plugin_unload E\n");

                            //TODO : Clean commands
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
                            Msg("M: clr_plugin_unload F {0}\n", lstCommands.Count);
                            foreach (ClsConCommand cmd in lstCommands)
                            {
                                this.UnregisterConCommand(plugin, cmd);
                            }

                            NativeMethods.Mono_Msg("M: clr_plugin_unload G\n");

                            //Clean vars
                            List<ClsConvar> lstVars = new List<ClsConvar>();
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
                            Msg("M: clr_plugin_unload H {0}\n", lstVars.Count);
                            foreach (ClsConvar v in lstVars)
                            {
                                this.UnregisterConvar(plugin, v);
                            }

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
                    Msg("Plugin '{0}' not found or loaded\n", args);
                }
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    Msg(ex.GetType().FullName + "\n");
                    Msg(ex.Message + "\n");
                    Msg(ex.StackTrace + "\n");
                    ex = ex.InnerException;
                }
            }
        }
    }
}
