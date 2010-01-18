using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Base class for Convars and ConCommands
    /// </summary>
    public abstract class ClsConCommandBase : MarshalByRefObject
    {
        private readonly string _name;
        private readonly string _help;
        private readonly UInt64 _nativeId;
        private readonly FCVAR _flags;

        internal ClsConCommandBase(UInt64 nativeId, string name, string help, FCVAR flags)
        {
            this._nativeId = nativeId;
            this._name = name;
            this._help = help;
            this._flags = flags;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get { return this._name; } }
        /// <summary>
        /// Help text
        /// </summary>
        public string Help { get { return this._help; } }
        /// <summary>
        /// Flags
        /// </summary>
        public FCVAR Flags { get { return this._flags; } }
        internal UInt64 NativeID { get { return this._nativeId; } }

    }
}
