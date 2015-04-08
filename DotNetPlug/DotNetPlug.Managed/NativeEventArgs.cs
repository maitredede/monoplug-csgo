using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    [DebuggerDisplay("{Name} ({Type}) : {ToString()}")]
    [StructLayout(LayoutKind.Sequential)]
    public sealed class NativeEventArgs
    {
        [MarshalAs(UnmanagedType.BStr)]
        public string Name;
        public NativeEventArgType Type;
        public short IntVal;
        public string StrVal;
        public bool BoolVal;
        public UInt64 LongVal;
        public float FloatVal;

        public override string ToString()
        {
            switch (this.Type)
            {
                case NativeEventArgType.Int:
                    return this.IntVal.ToString();
                case NativeEventArgType.String:
                    return this.StrVal;
                case NativeEventArgType.Bool:
                    return this.BoolVal.ToString();
                case NativeEventArgType.Long:
                    return this.LongVal.ToString();
                case NativeEventArgType.Float:
                    return this.FloatVal.ToString();
                default:
                    return base.ToString();
            }
        }
    }
}
