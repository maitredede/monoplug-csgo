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
        ////private readonly ConCommandBaseData _data;
        //private readonly IMessage _msg;
        //private readonly IThreadPool _thPool;
        //private readonly ClsPluginBase _plugin;
        //private readonly string _name;
        //private readonly string _help;
        //private readonly FCVAR _flags;
        //private UInt64 _nativeId;
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

        //internal ClsConCommandBase(IMessage msg, IThreadPool thPool, ClsPluginBase plugin, string name, string help, FCVAR flags)
        //{
        //    this._msg = msg;
        //    this._thPool = thPool;
        //    this._plugin = plugin;
        //    this._name = name;
        //    this._help = help;
        //    this._flags = flags;
        //    this._nativeId = 0;
        //}

        //internal void SetId(UInt64 nativeId)
        //{
        //    this._nativeId = nativeId;
        //}

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
        //internal UInt64 NativeID { get { return this._nativeId; } }
        //internal ClsPluginBase Plugin { get { return this._plugin; } }
        //internal IMessage Msg { get { return this._msg; } }
        //internal IThreadPool ThreadPool { get { return this._thPool; } }
    }
}
