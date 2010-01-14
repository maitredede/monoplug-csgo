using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal ClsPlayer[] GetPlayers()
        {
            return this.InterThreadCall<ClsPlayer[], object>(this.GetPlayers_CALL, null);
        }

        private ClsPlayer[] GetPlayers_CALL(object data)
        {
            return NativeMethods.Mono_GetPlayers();
        }
    }
}
