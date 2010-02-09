using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    internal sealed class ClsPluginEngine : ObjectBase, IEngine
    {
        private readonly IEngineWrapper _conEntry;
        private readonly ClsPluginBase _owner;

        internal ClsPluginEngine(ClsPluginBase owner, IEngineWrapper conEntry)
        {
#if DEBUG
            Check.NonNull("owner", owner);
            Check.NonNull("conEntry", conEntry);
#endif
            this._conEntry = conEntry;
            this._owner = owner;
        }

        ClsConVar IEngine.RegisterConvar(string name, string help, FCVAR flags, string defaultValue)
        {
            Check.NonNullOrEmpty("name", name);
            Check.NonNullOrEmpty("help", help);
            Check.ValidFlags(flags, "flags");

            InternalConvar icvar = this._conEntry.RegisterConvar(name, help, flags, defaultValue);
            if (icvar != null)
            {
                ClsConVar var = new ClsConVar(icvar, this._owner);
                icvar.Set(var);
                return var;
            }
            else
            {
                return null;
            }
        }

        void IEngine.UnregisterConvar(ClsConVar var)
        {
            Check.NonNull("var", var);

            this._conEntry.UnregisterConvar(var.Internal);
        }

        ClsConCommand IEngine.RegisterConCommand(string name, string help, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate completion, bool async)
        {
            Check.NonNullOrEmpty("name", name);
            Check.NonNullOrEmpty("help", help);
            Check.ValidFlags(flags, "flags");
            Check.NonNull("code", code);

            InternalConCommand icmd = this._conEntry.RegisterConCommand(name, help, flags, code, completion, async);
            if (icmd == null)
            {
                return null;
            }
            else
            {
                ClsConCommand cmd = new ClsConCommand(icmd, this._owner);
                icmd.Set(cmd);
                return cmd;
            }
        }

        void IEngine.UnregisterConCommand(ClsConCommand command)
        {
            this._conEntry.UnregisterConCommand(command.Internal);
        }

        ClsPlayer[] IEngine.GetPlayers()
        {
            return this._conEntry.GetPlayers();
        }

        void IEngine.ServerCommand(string command)
        {
            this._conEntry.ServerCommand(command);
        }

        void IEngine.ClientPrint(ClsPlayer client, string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }

            this._conEntry.ClientPrint(client, message);
        }

        ClsSayCommand IEngine.RegisterSayCommand(string trigger, bool async, bool hidden, ConCommandDelegate code)
        {
            Check.NonNullOrEmpty("trigger", trigger);
            Check.NonNull("code", code);

            InternalSayCommand cmd = this._conEntry.RegisterSayCommand(trigger, async, hidden, this._owner, code);
            if (cmd != null)
            {
                return new ClsSayCommand(cmd);
            }
            return null;
        }

        void IEngine.UnregisterSayCommand(ClsSayCommand command)
        {
            Check.NonNull("command", command);
            this._conEntry.UnregisterSayCommand(command.Internal);
        }

        void IEngine.ClientDialog(ClsPlayer player, string title, string message, Color color, int level, int time)
        {
            Check.NonNull("player", player);

            this._conEntry.ClientDialogMessage(player, title, message, color, level, time);
        }

        #region IEngine Membres


        void IEngine.ClientMenuMessage(ClsPlayer player, string title, string message, int level, int time)
        {
            Check.NonNull("player", player);

            this._conEntry.ClientMenuMessage(player, title, message, level, time);
        }

        #endregion
    }
}
