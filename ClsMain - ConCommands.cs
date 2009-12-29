
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MonoPlug
{
    partial class ClsMain
    {
        private void LoadAllCommands(object item)
        {
            Type t = item.GetType();
            Type a = typeof(ConCommandAttribute);
            Type d = typeof(ConCommandDelegate);
            foreach (MethodInfo m in t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                object[] arr = m.GetCustomAttributes(a, false);
                if (arr.Length == 1)
                {
                    ConCommandAttribute att = (ConCommandAttribute)arr[0];
                    if (IsMethodOfSignature(m, d) || IsMethodOfSignature(m, d, item))
                    {
                        ConCommandDelegate code = (ConCommandDelegate)Delegate.CreateDelegate(d, m, false);
                        if (code == null)
                        {
                            code = (ConCommandDelegate)Delegate.CreateDelegate(d, item, m);
                        }
                        this.RegisterCommand(null, att.Name, att.Description, code, att.Flags);
                    }
                }
            }
        }

        private void LoadAllCommands(ClsPluginBase plugin)
        {
            Type t = plugin.GetType();
            Type a = typeof(ConCommandAttribute);
            Type d = typeof(ConCommandDelegate);
            foreach (MethodInfo m in t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                object[] arr = m.GetCustomAttributes(a, false);
                if (arr.Length == 1)
                {
                    ConCommandAttribute att = (ConCommandAttribute)arr[0];
                    if (IsMethodOfSignature(m, d) || IsMethodOfSignature(m, d, plugin))
                    {
                        ConCommandDelegate code = (ConCommandDelegate)Delegate.CreateDelegate(d, m, false);
                        if (code == null)
                        {
                            code = (ConCommandDelegate)Delegate.CreateDelegate(d, plugin, m);
                        }
                        this.RegisterCommand(plugin, att.Name, att.Description, code, att.Flags);
                    }
                }
            }
        }

        private Dictionary<string, ConCommandEntry> _commands = null;

        internal bool RegisterCommand(ClsPluginBase plugin, string name, string description, ConCommandDelegate code, FCVAR flags)
        {
            if (code == null) throw new ArgumentNullException("code");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(description)) throw new ArgumentNullException("description");
            ValidateFlags(flags, "flags");

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
