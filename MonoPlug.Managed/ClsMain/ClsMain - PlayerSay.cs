﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;

namespace MonoPlug
{
    partial class ClsMain
    {
        private readonly ReaderWriterLock _lckSayCommands = new ReaderWriterLock();
        private readonly Dictionary<string, List<InternalSayCommand>> _dicSayCommands = new Dictionary<string, List<InternalSayCommand>>();
        private readonly Dictionary<ClsPluginBase, List<InternalSayCommand>> _dicSayCommandsPlugin = new Dictionary<ClsPluginBase, List<InternalSayCommand>>();

        internal bool Raise_PlayerSay(ClsPlayer player, string text, string[] args)
        {
            this._lckSayCommands.AcquireReaderLock(Timeout.Infinite);
            try
            {
                string[] arr = text.Split(new char[] { ' ' }, 2);
                string test = arr[0].ToLowerInvariant();
                if (this._dicSayCommands.ContainsKey(test))
                {
                    bool hide = false;
                    List<InternalSayCommand> lst = this._dicSayCommands[test];
                    foreach (InternalSayCommand cmd in lst)
                    {
                        cmd.Execute(player, text, args);
                        hide |= cmd.Hide;
                    }
                    return hide;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                this._lckSayCommands.ReleaseLock();
            }
        }

        InternalSayCommand IEngineWrapper.RegisterSayCommand(string name, bool async, bool hidden, ClsPluginBase plugin, ConCommandDelegate code)
        {
#if DEBUG
            this._msg.DevMsg("ClsMain::RegisterSayCommand '{0}'\n", name);
#endif
            //TODO : SayCommand check name with regex
            if (name.Contains(' '))
            {
                throw new ArgumentException("name must not contain spaces");
            }

            this._lckSayCommands.AcquireWriterLock(Timeout.Infinite);
            try
            {
                IThreadPool pool;
                if (plugin == null)
                {
                    pool = this._thPool;
                }
                else
                {
                    pool = plugin.ThreadPool;
                }
                InternalSayCommand cmd = new InternalSayCommand(name, async, hidden, plugin, code, pool);

                //Add SayCmd to name-indexed list
                List<InternalSayCommand> lst;
                if (this._dicSayCommands.ContainsKey(name.ToLowerInvariant()))
                {
                    lst = this._dicSayCommands[name.ToLowerInvariant()];
                }
                else
                {
                    lst = new List<InternalSayCommand>();
                    this._dicSayCommands.Add(name.ToLowerInvariant(), lst);
                }
                lst.Add(cmd);

                //Add SayCmd to plugin-indexed list
                if (this._dicSayCommandsPlugin.ContainsKey(plugin))
                {
                    lst = this._dicSayCommandsPlugin[plugin];
                }
                else
                {
                    lst = new List<InternalSayCommand>();
                    this._dicSayCommandsPlugin.Add(plugin, lst);
                }
                lst.Add(cmd);

                return cmd;
            }
            finally
            {
                this._lckSayCommands.ReleaseReaderLock();
#if DEBUG
                this._msg.DevMsg("ClsMain::RegisterSayCommand (exit)\n");
#endif
            }
        }

        void IEngineWrapper.UnregisterSayCommand(InternalSayCommand command)
        {
            Check.NonNull("command", command);
            this._lckSayCommands.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (this._dicSayCommands.ContainsKey(command.Name.ToLowerInvariant()))
                {
                    List<InternalSayCommand> lst = this._dicSayCommands[command.Name.ToLowerInvariant()];
                    if (lst.Contains(command))
                    {
                        lst.Remove(command);
                    }
                    if (lst.Count == 0)
                    {
                        this._dicSayCommands.Remove(command.Name.ToLowerInvariant());
                    }
                }

                if (this._dicSayCommandsPlugin.ContainsKey(command.Plugin))
                {
                    List<InternalSayCommand> lst = this._dicSayCommandsPlugin[command.Plugin];
                    if (lst.Contains(command))
                    {
                        lst.Remove(command);
                    }
                    if (lst.Count == 0)
                    {
                        this._dicSayCommandsPlugin.Remove(command.Plugin);
                    }
                }
            }
            finally
            {
                this._lckSayCommands.ReleaseReaderLock();
            }
        }
    }
}