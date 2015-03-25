using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPlug
{
    [ComVisible(true)]
    public sealed class PluginManager : IDisposable
    {
        private static PluginManager s_instance;

        internal static PluginManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (typeof(PluginManager))
                    {
                        if (s_instance == null)
                        {
                            s_instance = new PluginManager();
                        }
                    }
                }
                return s_instance;
            }
        }

        private readonly SynchronizationContext m_syncCtx;

        private PluginManager()
        {
            this.m_syncCtx = new EngineSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(this.m_syncCtx);
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
        }

        internal void WorkError(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
