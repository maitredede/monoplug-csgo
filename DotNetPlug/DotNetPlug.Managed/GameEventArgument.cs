using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class GameEventArgument
    {
        internal GameEventArgument(NativeEventArgs arg)
        {
            this.Name = arg.Name;
            this.Type = arg.Type;
            this.ValueInt = arg.IntVal;
            this.ValueString = arg.StrVal;
            this.ValueBool = arg.BoolVal;
            this.ValueLong = arg.LongVal;
            this.ValueFloat = arg.FloatVal;
        }

        public string Name { get; internal set; }
        public ArgumentValueType Type { get; internal set; }
        public int ValueInt { get; internal set; }
        public string ValueString { get; internal set; }
        public bool ValueBool { get; internal set; }
        public ulong ValueLong { get; internal set; }
        public float ValueFloat { get; internal set; }
    }
}
