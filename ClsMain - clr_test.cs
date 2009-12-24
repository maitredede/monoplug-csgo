using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        private void clr_test(string args)
        {
            Mono_Msg(string.Format("CLRTEST args={0}\n", args));
            this.UnregisterCommand(null, "clr_test");
            Mono_Msg("CLRTEST Unregistered OK\n");
        }

    }
}
