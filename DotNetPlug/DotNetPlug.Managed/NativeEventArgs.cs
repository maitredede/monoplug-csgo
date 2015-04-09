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
    internal sealed class NativeEventArgs
    {
        [MarshalAs(UnmanagedType.BStr)]
        public string Name;
        public ArgumentValueType Type;
        public short IntVal;
        public string StrVal;
        public bool BoolVal;
        public UInt64 LongVal;
        public float FloatVal;

        public override string ToString()
        {
            switch (this.Type)
            {
                case ArgumentValueType.Int:
                    return this.IntVal.ToString();
                case ArgumentValueType.String:
                    return this.StrVal;
                case ArgumentValueType.Bool:
                    return this.BoolVal.ToString();
                case ArgumentValueType.Long:
                    return this.LongVal.ToString();
                case ArgumentValueType.Float:
                    return this.FloatVal.ToString();
                default:
                    return base.ToString();
            }
        }
    }
}
