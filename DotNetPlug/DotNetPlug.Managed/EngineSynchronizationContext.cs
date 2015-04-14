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
        private readonly ConcurrentQueue<EngineSynchronizationContextWorkItem> m_queue;

        public EngineSynchronizationContext()
        {
            this.m_queue = new ConcurrentQueue<EngineSynchronizationContextWorkItem>();
        }

        /// <summary>
        /// Lors d'une substitution dans une classe dérivée, distribue un message asynchrone à un contexte de synchronisation.
        /// </summary>
        /// <param name="d">Délégué <see cref="T:System.Threading.SendOrPostCallback" /> à appeler.</param>
        /// <param name="state">Objet passé au délégué.</param>
        public override void Post(SendOrPostCallback d, object state)
        {
            EngineSynchronizationContextWorkItem wrk = new EngineSynchronizationContextWorkItem(d, state, true);
            this.m_queue.Enqueue(wrk);
        }

        /// <summary>
        /// Lors d'une substitution dans une classe dérivée, distribue un message synchrone à un contexte de synchronisation.
        /// </summary>
        /// <param name="d">Délégué <see cref="T:System.Threading.SendOrPostCallback" /> à appeler.</param>
        /// <param name="state">Objet passé au délégué.</param>
        public override void Send(SendOrPostCallback d, object state)
        {
            using (EngineSynchronizationContextWorkItem wrk = new EngineSynchronizationContextWorkItem(d, state, true))
            {
                this.m_queue.Enqueue(wrk);
                wrk.Wait();
            }
        }

        public void Tick()
        {
            EngineSynchronizationContextWorkItem item;
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
}
