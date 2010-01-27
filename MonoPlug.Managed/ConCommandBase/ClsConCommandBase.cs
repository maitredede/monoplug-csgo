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
        private readonly ConCommandBaseData _data;

        internal ClsConCommandBase(ConCommandBaseData data)
        {
            this._data = data;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get { return this._data.Name; } }
        /// <summary>
        /// Help text
        /// </summary>
        public string Help { get { return this._data.Help; } }
        /// <summary>
        /// Flags
        /// </summary>
        public FCVAR Flags { get { return this._data.Flags; } }
        internal UInt64 NativeID { get { return this._data.NativeID; } }
        internal ClsPluginBase Plugin { get { return this._data.Plugin; } }
    }
}
