using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        [ConCommand("clr_test", "Add the clr_untest command", FCVAR.FCVAR_GAMEDLL)]
        private void clr_test(string args)
        {
            this.RegisterCommand(null, "clr_untest", "Remove the clr_test command", this.clr_untest, FCVAR.FCVAR_GAMEDLL);
            Mono_Msg("clr_untest registered OK\n");
        }

        [ConCommand("clr_untest", "Remove the clr_test command", FCVAR.FCVAR_GAMEDLL)]
        private void clr_untest(string args)
        {
            this.UnregisterCommand(null, "clr_test");
            Mono_Msg("clr_test unregistred\n");
        }
    }
}
