using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Base class for Convars and ConCommands
    /// </summary>
    public abstract class ClsConCommandBase : ObjectBase
    {
        private readonly InternalConbase _internalBase;
        private readonly ClsPluginBase _owner;

        internal InternalConbase InternalBase { get { return this._internalBase; } }

        internal ClsConCommandBase(InternalConbase internalBase, ClsPluginBase owner)
        {
#if DEBUG
            Check.NonNull("internalBase", internalBase);
            Check.NonNull("owner", owner);
#endif
            this._internalBase = internalBase;
            this._owner = owner;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get { return this._internalBase.Name; } }
        /// <summary>
        /// Help text
        /// </summary>
        public string Help { get { return this._internalBase.Help; } }
        /// <summary>
        /// Flags
        /// </summary>
        public FCVAR Flags { get { return this._internalBase.Flags; } }

        internal ClsPluginBase Plugin { get { return this._owner; } }
    }
}
