using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPlug
{
    internal sealed class EngineSynchronizationContext : SynchronizationContext
    {
        private readonly ConcurrentQueue<WorkItem> m_queue;

        public EngineSynchronizationContext()
        {
            this.m_queue = new ConcurrentQueue<WorkItem>();
        }

        /// <summary>
        /// Lors d'une substitution dans une classe dérivée, distribue un message asynchrone à un contexte de synchronisation.
        /// </summary>
        /// <param name="d">Délégué <see cref="T:System.Threading.SendOrPostCallback" /> à appeler.</param>
        /// <param name="state">Objet passé au délégué.</param>
        public override void Post(SendOrPostCallback d, object state)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("EngineSynchronizationContext::Post");
#endif
            WorkItem wrk = new WorkItem(d, state, true);
            this.m_queue.Enqueue(wrk);
        }

        /// <summary>
        /// Lors d'une substitution dans une classe dérivée, distribue un message synchrone à un contexte de synchronisation.
        /// </summary>
        /// <param name="d">Délégué <see cref="T:System.Threading.SendOrPostCallback" /> à appeler.</param>
        /// <param name="state">Objet passé au délégué.</param>
        public override void Send(SendOrPostCallback d, object state)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("EngineSynchronizationContext::Send");
#endif
            using (WorkItem wrk = new WorkItem(d, state, true))
            {
                this.m_queue.Enqueue(wrk);
                wrk.Wait();
            }
        }

        public void Tick()
        {
            WorkItem item;
            if (this.m_queue.TryDequeue(out item))
            {
                try
                {
                    item.Execute();
                }
                catch (Exception ex)
                {
                    PluginManager.Instance.WorkError(ex);
                }
                finally
                {
                    item.Release();
                }
            }
        }
    }

    internal sealed class WorkItem : IDisposable
    {
        private readonly SendOrPostCallback m_delegate;
        private readonly object m_state;
        private readonly ManualResetEventSlim m_latch;

        public WorkItem(SendOrPostCallback d, object state, bool synchronous)
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
