
using System;
using System.Collections.Generic;

namespace MonoPlug
{
    partial class ClsMain
    {
        private Dictionary<string, ConCommandEntry> _commands;
        private Dictionary<string, ConVarEntry> _vars;

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

        internal bool RegisterConVarString(ClsPluginBase plugin, string name, string description, FCVAR flags, string defaultValue, ConVarStringGetDelegate getFunction, ConVarStringSetDelegate setFunction)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(description)) throw new ArgumentNullException("description");
            if (!Enum.IsDefined(typeof(FCVAR), flags)) throw new ArgumentOutOfRangeException("flags");
            if (getFunction == null) throw new ArgumentNullException("getFunction");
            if (setFunction == null) throw new ArgumentNullException("setFunction");

            ConVarEntry entry = new ConVarEntry();
            entry.Plugin = plugin;
            entry.Name = name;
            entry.Description = description;
            entry.Get = getFunction;
            entry.Set = setFunction;

            lock (this._vars)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("name");
                }
                if (this._vars.ContainsKey(name))
                {
                    return false;
                }

                Mono_RegisterConVarString(name, description, defaultValue, getFunction, setFunction, (int)flags);
                if (true)
                {
                    this._vars.Add(name, entry);
                    return true;
                }
                //else
                //{
                //    return false;
                //}
            }
        }
    }
}
