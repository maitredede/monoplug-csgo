using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        private void clr(ClsPlayer sender, string line, string[] args)
        {
            if (args.Length < 2)
            {
                this._clrPrintUsage();
            }
            else
            {
                switch (args[1].ToLowerInvariant())
                {
                    case "version":
                        this._clrPrintVersion();
                        break;
                    case "list":
                        this._clrList();
                        break;
                    case "refresh":
                        this.RefreshPluginCache();
                        break;
                    case "load":
                        this._msg.DevMsg("load {0}\n", args.Length);
                        if (args.Length > 2)
                        {
                            for (int i = 2; i < args.Length; i++)
                            {
                                this._msg.DevMsg("Loading {0}\n", args[i]);
                                this.LoadPlugin(args[i]);
                            }
                        }
                        break;
                    case "unload":
                        if (args.Length > 2)
                        {
                            for (int i = 2; i < args.Length; i++)
                            {
                                this.UnloadPlugin(args[i]);
                            }
                        }
                        break;
                    case "reload":
                        this._clrReload();
                        break;
                    case "dump":
                        this._clrDump();
                        break;
                }
            }
        }

        private void _clrPrintUsage()
        {
            this._msg.Msg("Usage :\n");
            this._msg.Msg(" - version : show versions of core assemblies\n");
            this._msg.Msg(" - list    : list available and loaded plugins\n");
            this._msg.Msg(" - refresh : refresh plugin list from disk\n");
            this._msg.Msg(" - dump    : dump loaded plugins, var and commands\n");
            this._msg.Msg(" - load    : load a plugin\n");
            this._msg.Msg(" - unload  : unload a plugin\n");
            this._msg.Msg(" - reload  : reload config file and load/unload plugins\n");
        }

        private void _clrPrintVersion()
        {
            this._msg.Msg("Mono version : {0}\n", ClsMain.GetMonoVersion());
            ClsProxy.WriteAssemblyVersion(this._msg, typeof(MySql.Data.MySqlClient.MySqlConnection), "MySQL version : {0}\n");
            ClsProxy.WriteAssemblyVersion(this._msg, typeof(ICSharpCode.SharpZipLib.Zip.ZipEntry), "SharpZip version : {0}\n");
        }

        private void _clrDump()
        {
            List<string> keys = new List<string>();
            Dictionary<string, List<string>> var = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> cmd = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> say = new Dictionary<string, List<string>>();

            this._lckConCommandBase.AcquireReaderLock(Timeout.Infinite);
            this._lckSayCommands.AcquireReaderLock(Timeout.Infinite);
            try
            {
                try
                {
                    //Get convars and concommands
                    foreach (ulong nativeId in this._conCommandBase.Keys)
                    {
                        InternalConbase cb = this._conCommandBase[nativeId];
                        string key;
                        if (cb.Plugin == null)
                        {
                            //for core
                            key = string.Empty;
                        }
                        else
                        {
                            key = cb.Plugin.Name;
                        }

                        if (!keys.Contains(key))
                        {
                            keys.Add(key);
                        }

                        if (cb.IsCommand)
                        {
                            if (!cmd.ContainsKey(key))
                            {
                                cmd.Add(key, new List<string>());
                            }
                            cmd[key].Add(cb.Name);
                        }
                        else
                        {
                            if (!var.ContainsKey(key))
                            {
                                var.Add(key, new List<string>());
                            }
                            cmd[key].Add(cb.Name);
                        }
                    }

                    //Get saycommands
                    foreach (ClsPluginBase plugin in this._dicSayCommandsPlugin.Keys)
                    {
                        string key = plugin.Name;
                        if (!keys.Contains(key))
                        {
                            keys.Add(key);
                        }
                        if (!say.ContainsKey(key))
                        {
                            say.Add(key, new List<string>());
                        }
                        foreach (InternalSayCommand saycmd in this._dicSayCommandsPlugin[plugin])
                        {
                            say[key].Add(saycmd.Name);
                        }
                    }

                    //Write down
                    foreach (string key in keys)
                    {
                        if (string.IsNullOrEmpty(key))
                        {
                            this._msg.Msg("Core items :\n");
                        }
                        else
                        {
                            this._msg.Msg("Plugin {0} items :\n", key);
                        }

                        if (cmd.ContainsKey(key) && cmd[key].Count > 0)
                        {
                            foreach (string name in cmd[key])
                            {
                                this._msg.Msg(" cmd: {0}\n", name);
                            }
                        }
                        if (var.ContainsKey(key) && var[key].Count > 0)
                        {
                            foreach (string name in var[key])
                            {
                                this._msg.Msg(" var: {0}\n", name);
                            }
                        }
                        if (say.ContainsKey(key) && say[key].Count > 0)
                        {
                            foreach (string name in say[key])
                            {
                                this._msg.Msg(" say: {0}\n", name);
                            }
                        }
                    }
                }
                finally
                {
                    this._lckSayCommands.ReleaseReaderLock();
                }

            }
            finally
            {
                this._lckConCommandBase.ReleaseReaderLock();
            }
        }
    }
}
