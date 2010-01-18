using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    internal struct ConVarEntry
    {
        private ClsPluginBase _plugin;
        private ClsConvar _var;
        private ConvarRegisterData _regData;

        internal ConVarEntry(ClsPluginBase plugin, ClsConvar var, ConvarRegisterData regData)
        {
            this._plugin = plugin;
            this._var = var;
            this._regData = regData;
        }
        public ClsPluginBase Plugin { get { return this._plugin; } }
        public ClsConvar Var { get { return this._var; } }
        public ConvarRegisterData RegData { get { return this._regData; } }
    }
}
