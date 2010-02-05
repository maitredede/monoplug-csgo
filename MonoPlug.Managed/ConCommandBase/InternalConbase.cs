using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal abstract class InternalConbase : ObjectBase
    {
        private readonly string _name;
        private readonly string _help;
        private readonly FCVAR _flags;
        private UInt64 _nativeId;

        internal string Name { get { return this._name; } }
        internal string Help { get { return this._help; } }
        internal FCVAR Flags { get { return this._flags; } }
        internal UInt64 NativeID { get { return this._nativeId; } }

        protected InternalConbase(string name, string help, FCVAR flags)
        {
            this._name = name;
            this._help = help;
            this._flags = flags;
            this._nativeId = 0;
        }

        internal void SetId(UInt64 nativeId)
        {
            this._nativeId = nativeId;
        }

        internal abstract ClsPluginBase Plugin { get; }
    }

    internal abstract class InternalConbase<TPublic> : InternalConbase where TPublic : ClsConCommandBase
    {
        private TPublic _public;

        internal InternalConbase(string name, string help, FCVAR flags)
            : base(name, help, flags)
        {
        }

        internal TPublic Public { get { return this._public; } }

        internal void Set(TPublic value)
        {
            this._public = value;
        }

        internal override ClsPluginBase Plugin
        {
            get
            {
                if (this._public != null)
                {
                    return this._public.Plugin;
                }
                return null;
            }
        }
    }
}
