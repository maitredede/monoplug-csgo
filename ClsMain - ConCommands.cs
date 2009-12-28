
using System;
using System.Collections.Generic;

namespace MonoPlug
{
    partial class ClsMain
    {
        private Dictionary<string, ConCommandEntry> _commands;

        internal bool RegisterCommand(ClsPluginBase plugin, string name, string description, ConCommandDelegate code, FCVAR flags)
        {
            if (code == null) throw new ArgumentNullException("code");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(description)) throw new ArgumentNullException("description");
            if (!Enum.IsDefined(typeof(FCVAR), flags)) throw new ArgumentOutOfRangeException("flags");

            ConCommandEntry entry = new ConCommandEntry();
            entry.Plugin = plugin;
            entry.Name = name;
            entry.Description = description;
            entry.Code = code;

            lock (this._commands)
            {
                if (this._commands.ContainsKey(name))
                {
                    return false;
                }

                Mono_RegisterConCommand(name, description, code, (int)flags);
                if (true)
                {
                    this._commands.Add(name, entry);
                    return true;
                }
                //else
                //{
                //    return false;
                //}
            }
        }

        internal bool UnregisterCommand(ClsPluginBase plugin, string name)
        {
            lock (this._commands)
            {
                if (this._commands.ContainsKey(name))
                {
                    ConCommandEntry entry = this._commands[name];
                    if (entry.Plugin == plugin)
                    {
                        Mono_UnregisterConCommand(name);
                        this._commands.Remove(name);
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
