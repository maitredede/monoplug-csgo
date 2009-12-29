using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        [ConCommand("clr_plugin_unload", "Unload a loaded plugin", FCVAR.FCVAR_GAMEDLL)]
        private void clr_plugin_unload(string args)
        {
            //Find if plugin is loaded
            lock (this._plugins)
            {
                foreach (AppDomain dom in this._plugins.Keys)
                {
                    ClsPluginBase plugin = this._plugins[dom];
                    if (plugin.Name.Equals(args, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //Deinit plugin
                        plugin.UnInit();

                        //Clean commands
                        List<string> lstCommands = new List<string>();
                        lock (this._ConCommands)
                        {
                            foreach (string cmd in this._ConCommands.Keys)
                            {
                                if (this._ConCommands[cmd].Plugin == plugin)
                                {
                                    lstCommands.Add(cmd);
                                }
                            }
                        }
                        foreach (string cmd in lstCommands)
                        {
                            this.UnregisterCommand(plugin, cmd);
                        }

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
                        foreach (ClsConVarStrings v in lstVars)
                        {
                            this.UnregisterConVarString(plugin, v);
                        }

                        //Clean plugin
                        string name = plugin.Name;
                        this._plugins.Remove(dom);
                        AppDomain.Unload(dom);

                        Msg("Plugin '{0}' unloaded\n", name);
                        return;
                    }
                }
            }
            Msg("Plugin '{0}' not found or loaded\n", args);
        }
    }
}
