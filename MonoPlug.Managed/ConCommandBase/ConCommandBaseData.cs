using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal abstract class ConCommandBaseData : MarshalByRefObject
    {
        protected ConCommandBaseData(ClsPluginBase plugin, string name, string help, FCVAR flags)
        {
            this._plugin = plugin;
            this._name = name;
            this._help = help;
            this._flags = flags;
        }
        private readonly ClsPluginBase _plugin;
        private readonly string _name;
        private readonly string _help;
        private readonly FCVAR _flags;
        private UInt64 _nativeId;

        public ClsPluginBase Plugin { get { return this._plugin; } }
        public string Name { get { return this._name; } }
        public string Help { get { return this._help; } }
        public FCVAR Flags { get { return this._flags; } }
        public UInt64 NativeID { get { return this._nativeId; } set { this._nativeId = value; } }
    }
}
