
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal ClsConCommand RegisterConCommand(ClsPluginBase plugin, string name, string help, ConCommandDelegate code, FCVAR flags, ConCommandCompleteDelegate complete)
        {
            Check.NonNull("code", code);
            Check.NonNullOrEmpty("name", name);
            Check.NonNullOrEmpty("help", help);
            Check.ValidFlags(flags, "flags");

            ConCommandEntry entry = new ConCommandEntry();
            entry.Plugin = plugin;
            entry.Command = new ClsConCommand(ulong.MinValue, name, help, flags, code, complete);

            lock (this._concommands)
            {
                if (this._concommands.ContainsKey(name))
                {
                    return null;
                }

                if (this.InterThreadCall<bool, ConCommandEntry>(this.RegisterConCommand_Call, entry))
                {
                    this._concommands.Add(entry.Command.Name, entry);
                    return entry.Command;
                }
                else
                {
                    return null;
                }
            }
        }

        private bool RegisterConCommand_Call(ConCommandEntry entry)
        {
            return NativeMethods.Mono_RegisterConCommand(entry.Command.Name, entry.Command.Help, entry.Command.Code, (int)entry.Command.Flags, entry.Command.Complete);
        }

        internal bool UnregisterConCommand(ClsPluginBase plugin, ClsConCommand command)
        {
            lock (this._concommands)
            {
                if (this._concommands.ContainsKey(command.Name))
                {
                    ConCommandEntry entry = this._concommands[command.Name];
                    if (entry.Plugin == plugin && entry.Command == command)
                    {
                        if (this.InterThreadCall<bool, string>(this.UnregisterConCommand_Call, command.Name))
                        {
                            this._concommands.Remove(command.Name);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool UnregisterConCommand_Call(string name)
        {
            return NativeMethods.Mono_UnregisterConCommand(name);
        }
    }
}
