using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    public abstract class ClsConCommandBase : MarshalByRefObject
    {
        private readonly string _name;
        private readonly string _desc;
        private readonly UInt64 _nativeId;
        private readonly FCVAR _flags;

        internal ClsConCommandBase(UInt64 nativeId, string name, string description, FCVAR flags)
        {
            this._nativeId = nativeId;
            this._name = name;
            this._desc = description;
            this._flags = flags;
        }

        public string Name { get { return this._name; } }
        public string Description { get { return this._desc; } }
        public FCVAR Flags { get { return this._flags; } }
        internal UInt64 NativeID { get { return this._nativeId; } }

    }
}
