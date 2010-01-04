
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal ClsConCommand RegisterCommand(ClsPluginBase plugin, string name, string description, ConCommandDelegate code, FCVAR flags, ConCommandCompleteDelegate complete)
        {
            if (code == null) throw new ArgumentNullException("code");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(description)) throw new ArgumentNullException("description");
            ValidateFlags(flags, "flags");

            lock (this._ConCommands)
            {
                if (this._ConCommands.ContainsKey(name))
                {
                    return null;
                }

                ConCommandEntry entry = new ConCommandEntry();
                entry.Plugin = plugin;
                entry.Command = new ClsConCommand(0, name, description, flags, code, complete);

                bool ret = false;
                this.InterThreadCall(() =>
                {
                    ret = Mono_RegisterConCommand(name, description, code, (int)flags, complete);
                });
                if (ret)
                {
                    this._ConCommands.Add(name, entry);
                    return entry.Command;
                }
                else
                {
                    return null;
                }
            }
        }

        internal bool UnregisterCommand(ClsPluginBase plugin, ClsConCommand command)
        {
            lock (this._ConCommands)
            {
                if (this._ConCommands.ContainsKey(command.Name))
                {
                    ConCommandEntry entry = this._ConCommands[command.Name];
                    if (entry.Plugin == plugin)
                    {
                        bool ret = false;
                        this.InterThreadCall(() =>
                        {
                            ret = Mono_UnregisterConCommand(command.Name);
                        });
                        this._ConCommands.Remove(command.Name);
                        return true;
                        //						if()
                        //						{
                        //						}
                        //						else
                        //						{
                        //							return false;
                        //						}
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
