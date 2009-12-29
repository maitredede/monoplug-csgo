using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        private readonly object _lckMsg = new object();

        internal void Msg(string message)
        {
        }

        internal void Msg(string format, params object[] vals)
        {
            this.Msg(string.Format(format, vals));
        }
    }
}
