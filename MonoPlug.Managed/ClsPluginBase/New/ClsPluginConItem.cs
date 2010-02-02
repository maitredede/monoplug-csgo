using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class ClsPluginConItem : MarshalByRefObject, IConItem
    {
        private readonly IConItemEntry _conEntry;
        private readonly ClsPluginBase _owner;

        internal ClsPluginConItem(ClsPluginBase owner, IConItemEntry conEntry)
        {
#if DEBUG
            Check.NonNull("owner", owner);
            Check.NonNull("conEntry", conEntry);
#endif
            this._conEntry = conEntry;
            this._owner = owner;
        }

        ClsConVar IConItem.RegisterConvar(string name, string help, FCVAR flags, string defaultValue)
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

        void IConItem.UnregisterConvar(ClsConVar var)
        {
            Check.NonNull("var", var);

            this._conEntry.UnregisterConvar(var.Internal);
        }

        ClsConCommand IConItem.RegisterConCommand(string name, string help, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate completion, bool async)
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

        void IConItem.UnregisterConCommand(ClsConCommand command)
        {
            this._conEntry.UnregisterConCommand(command.Internal);
        }

    }
}
