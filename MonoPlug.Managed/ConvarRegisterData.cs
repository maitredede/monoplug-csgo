using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    [Obsolete("replaced with convarmain", true)]
    internal struct ConvarRegisterData
    {
        internal ConvarRegisterData(string name, string help, FCVAR flags, string defaultValue, ThreadStart changed)
        {
            this._name = name;
            this._help = help;
            this._flags = flags;
            this._defaultValue = defaultValue;
            this._changed = changed;
        }

        private string _name;
        private string _help;
        private FCVAR _flags;
        private string _defaultValue;
        private ThreadStart _changed;

        public string Name { get { return this._name; } }
        public string Help { get { return this._help; } }
        public FCVAR Flags { get { return this._flags; } }
        public string DefaultValue { get { return this._defaultValue; } }
        public ThreadStart Changed { get { return this._changed; } }
    }
}
