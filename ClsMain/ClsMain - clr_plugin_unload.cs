using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        //[ConCommand("clr_plugin_unload", "Unload a loaded plugin", FCVAR.FCVAR_GAMEDLL)]
        private void clr_plugin_unload(string args)
        {
            Msg("M: clr_plugin_unload 0\n");
            try
            {
                Msg("M: clr_plugin_unload A\n");
                //Find if plugin is loaded
                bool found = false;
                lock (this._plugins)
                {
                    foreach (AppDomain dom in this._plugins.Keys)
                    {
                        Msg("M: clr_plugin_unload B\n");
                        ClsPluginBase plugin = this._plugins[dom];
                        Msg("M: clr_plugin_unload C {0}\n", plugin.Name);
                        if (plugin.Name.Equals(args, StringComparison.InvariantCultureIgnoreCase))
                        {
                            found = true;
                            Msg("M: clr_plugin_unload D\n");
                            //Deinit plugin
                            plugin.UnInit();

                            Msg("M: clr_plugin_unload E\n");

                            //Clean commands
                            List<ClsConCommand> lstCommands = new List<ClsConCommand>();
                            lock (this._ConCommands)
                            {
                                foreach (string name in this._ConCommands.Keys)
                                {
                                    if (this._ConCommands[name].Plugin == plugin)
                                    {
                                        lstCommands.Add(this._ConCommands[name].Command);
                                    }
                                }
                            }
                            Msg("M: clr_plugin_unload F {0}\n", lstCommands.Count);
                            foreach (ClsConCommand cmd in lstCommands)
                            {
                                this.UnregisterCommand(plugin, cmd);
                            }

                            Msg("M: clr_plugin_unload G\n");
                            //Clean vars
                            List<ClsConVarStrings> lstVars = new List<ClsConVarStrings>();
                            lock (this._ConVarString)
                            {
                                foreach (ulong id in this._ConVarString.Keys)
                                {
                                    if (this._ConVarString[id].Plugin == plugin)
                                    {
                                        lstVars.Add(this._ConVarString[id].Var);
                                    }
                                }
                            }
                            Msg("M: clr_plugin_unload H {0}\n", lstVars.Count);
                            foreach (ClsConVarStrings v in lstVars)
                            {
                                this.UnregisterConVarString(plugin, v);
                            }

                            Msg("M: clr_plugin_unload I\n");
                            //Clean plugin
                            string plugname = plugin.Name;
                            this._plugins.Remove(dom);
                            Msg("M: clr_plugin_unload J\n");
                            this._plugins.Remove(dom);
                            AppDomain.Unload(dom);

                            Msg("Plugin '{0}' unloaded\n", plugname);
                            break;
                        }
                    }
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
