using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Exception raised when a parallel operation has failed
    /// </summary>
    public sealed class ParallelException<T> : Exception
    {
        private Dictionary<T, Exception> _dic;

        internal ParallelException(Dictionary<T, Exception> dic)
            : base("Exceptions occured in processing, check Exceptions property")
        {
            this._dic = dic;
        }

        /// <summary>
        /// Dictionnary of item and exception it raised
        /// </summary>
        public Dictionary<T, Exception> Exceptions { get { return this._dic; } }
    }
}
