using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPlug
{
    internal sealed class EngineSynchronizationContextWorkItem : IDisposable
    {
        private readonly SendOrPostCallback m_delegate;
        private readonly object m_state;
        private readonly ManualResetEventSlim m_latch;

        public EngineSynchronizationContextWorkItem(SendOrPostCallback d, object state, bool synchronous)
        {
            this.m_delegate = d;
            this.m_state = state;
            if (synchronous)
            {
                this.m_latch = new ManualResetEventSlim(false);
            }
        }

        public void Execute()
        {
            this.m_delegate.Invoke(this.m_state);
        }

        public void Release()
        {
            this.m_latch.Set();
        }

        public void Wait()
        {
            this.m_latch.Wait();
        }

        public void Dispose()
        {
            if (this.m_latch != null)
            {
                this.m_latch.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
