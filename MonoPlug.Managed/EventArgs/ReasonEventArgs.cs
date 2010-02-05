using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Reason of event trigger
    /// </summary>
    public class ReasonEventArgs : EventArgs
    {
        private readonly string _reason;

        internal ReasonEventArgs(string reason)
        {
            this._reason = reason;
        }

        /// <summary>
        /// Reason for event triggering
        /// </summary>
        public string Reason { get { return this._reason; } }
    }
}
