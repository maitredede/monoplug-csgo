using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class GameEventData
    {
        public GameEventData()
        {
        }

        internal GameEventData(GameEventEventArgs e)
        {
            this.Event = e.Event;
            this.Args = e.Args.Select(a => new GameEventArg(a)).ToArray();
        }

        public GameEvent Event { get; set; }
        public GameEventArg[] Args { get; set; }
    }

    public sealed class GameEventArg
    {
        public GameEventArg() : base() { }
        internal GameEventArg(GameEventArgument arg)
            : this()
        {
            this.Name = arg.Name;
            this.Type = arg.Type;
            this.ValueInt = arg.ValueInt;
            this.ValueString = arg.ValueString;
            this.ValueBool = arg.ValueBool;
            this.ValueLong = arg.ValueLong;
            this.ValueFloat = arg.ValueFloat;
        }

        public string Name { get; set; }
        public ArgumentValueType Type { get; set; }
        public int ValueInt { get; set; }
        public string ValueString { get; set; }
        public bool ValueBool { get; set; }
        public ulong ValueLong { get; set; }
        public float ValueFloat { get; set; }
    }
}
