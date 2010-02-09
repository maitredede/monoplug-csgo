using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Say command
    /// </summary>
    public sealed class ClsSayCommand : ObjectBase
    {
        private readonly InternalSayCommand _cmd;

        internal ClsSayCommand(InternalSayCommand cmd)
        {
            Check.NonNull("cmd", cmd);

            this._cmd = cmd;
        }

        internal InternalSayCommand Internal { get { return this._cmd; } }
    }
}
