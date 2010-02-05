using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Lifetime;

namespace MonoPlug
{
    /// <summary>
    /// Base MarshalByRefObject with infinite lifetime
    /// </summary>
    public abstract class ObjectBase : MarshalByRefObject
    {
        internal ObjectBase()
        {
        }

        /// <summary>
        /// Lifetime overriding to avoid passive plugin removal
        /// </summary>
        /// <returns></returns>
        public sealed override object InitializeLifetimeService()
        {
            ILease lease = (ILease)base.InitializeLifetimeService();
            if (lease.CurrentState == LeaseState.Initial)
            {
                lease.InitialLeaseTime = TimeSpan.Zero;
            }
            return lease;
        }
    }
}
