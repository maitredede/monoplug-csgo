using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class CTuple<T1, T2>
    {
        private T1 _t1;
        private T2 _t2;

        public T1 Item1 { get { return this._t1; } set { this._t1 = value; } }
        public T2 Item2 { get { return this._t2; } set { this._t2 = value; } }

        public CTuple(T1 item1, T2 item2)
            : this()
        {
            this._t1 = item1;
            this._t2 = item2;
        }

        public CTuple()
        {
        }
    }
}
