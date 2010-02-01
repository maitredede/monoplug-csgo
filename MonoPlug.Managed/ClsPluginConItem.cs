using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class ClsPluginConItem : MarshalByRefObject, IConItem
    {
        private readonly ClsMain _main;
        private readonly ClsPluginBase _plugin;

        internal ClsPluginConItem(ClsMain main, ClsPluginBase plugin)
        {
            this._main = main;
            this._plugin = plugin;
        }

        public ClsConVar RegisterConvar(string name, string help, FCVAR flags, string defaultValue)
        {
            return this._main.RegisterConvar(this._plugin, name, help, flags, defaultValue);
        }

        public void UnregisterConvar(ClsConVar var)
        {
            this._main.UnregisterConvar(this._plugin, var);
        }

        public ClsConCommand RegisterConCommand(string name, string help, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate completion, bool async)
        {
            return this._main.RegisterConCommand(this._plugin, name, help, flags, code, completion, async);
        }

        public void UnregisterConCommand(ClsConCommand command)
        {
            this._main.UnregisterConCommand(this._plugin, command);
        }

    }
}
