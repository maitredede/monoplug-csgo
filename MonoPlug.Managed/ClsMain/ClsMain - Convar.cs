using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void Raise_ConVarChange(UInt64 nativeID)
        {
            this._lckConCommandBase.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (this._conCommandBase.ContainsKey(nativeID))
                {
                    InternalConbase item = this._conCommandBase[nativeID];
                    if (item is InternalConvar)
                    {
                        InternalConvar cvar = (InternalConvar)item;
                        cvar.RaiseValueChanged();
                    }
                }
            }
            finally
            {
                this._lckConCommandBase.ReleaseReaderLock();
            }
        }
    }
}
