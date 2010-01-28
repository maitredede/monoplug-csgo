using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ServiceModel;
using System.Threading;
using System.Reflection;

namespace MonoPlug
{
    /// <summary>
    /// Client component of the WCFConsole server plugin
    /// </summary>
    public partial class ClsWCFConsoleClient : Component, IConsoleClient
    {
        private readonly DuplexChannelFactory<IConsoleServer> _channelFactory;
        private IConsoleServer _server = null;

        /// <summary>
        /// Create a ClsWCFConsoleClient
        /// </summary>
        /// <param name="container"></param>
        public ClsWCFConsoleClient(IContainer container)
            : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Create a ClsWCFConsoleClient
        /// </summary>
        public ClsWCFConsoleClient()
        {
            InitializeComponent();

            this._channelFactory = new DuplexChannelFactory<IConsoleServer>(this);
        }

        /// <summary>
        /// Raised when a message has arrived
        /// </summary>
        public event EventHandler<ConMessageEventArgs> ConsoleMessage;

        private void timerPing_Tick(object sender, EventArgs e)
        {
            if (this._server != null)
            {
                try
                {
                    this._server.Ping();
                }
                catch
                {
                    this.Disconnect();
                }
            }
        }

        private sealed class ClsConsoleMessageAsyncResult : IAsyncResult, IDisposable
        {
            private readonly ManualResetEvent _wh = new ManualResetEvent(false);
            private readonly object _as;
            private readonly AsyncCallback _cb;
            private readonly ConMessageEventArgs _data;
            private Exception _err = null;
            private bool _completed = false;

            internal ClsConsoleMessageAsyncResult(ConMessageEventArgs data, AsyncCallback callback, object state)
            {
                this._data = data;
                this._cb = callback;
                this._as = state;
            }

            internal ConMessageEventArgs Data { get { return this._data; } }
            internal Exception Error { get { return this._err; } set { this._err = value; } }
            internal AsyncCallback Callback { get { return this._cb; } }

            void IDisposable.Dispose()
            {
                ((IDisposable)this._wh).Dispose();
            }

            public object AsyncState
            {
                get { return this._as; }
            }

            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get { return this._wh; }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get { return this._completed; }
            }

            internal void SetCompleted()
            {
                this._completed = true;
                this._wh.Set();
            }
        }

        IAsyncResult IConsoleClient.BeginConsoleMessage(bool hasColor, bool debug, Color color, string message, AsyncCallback callback, object state)
        {
            ConMessageEventArgs e = new ConMessageEventArgs(hasColor, debug, color, message);
            ClsConsoleMessageAsyncResult ar = new ClsConsoleMessageAsyncResult(e, callback, state);
            ThreadPool.QueueUserWorkItem(this.DoConsoleMessage, ar);
            return ar;
        }

        private void DoConsoleMessage(object state)
        {
            ClsConsoleMessageAsyncResult ar = (ClsConsoleMessageAsyncResult)state;
            try
            {
                if (this.ConsoleMessage != null)
                {
                    this.ConsoleMessage(this, ar.Data);
                }
            }
            catch (Exception ex)
            {
                ar.Error = ex;
            }
            finally
            {
                ar.SetCompleted();
                ar.Callback(ar);
            }
        }

        void IConsoleClient.EndConsoleMessage(IAsyncResult ar)
        {
            using (ClsConsoleMessageAsyncResult r = (ClsConsoleMessageAsyncResult)ar)
            {
                r.AsyncWaitHandle.WaitOne();
                if (r.Error != null)
                {
                    throw new TargetInvocationException(r.Error);
                }
            }
        }

        /// <summary>
        /// Connect to WCFConsole plugin
        /// </summary>
        /// <param name="host">Server ip or dns</param>
        /// <param name="port">Server port</param>
        public void Connect(string host, int port)
        {
            string s = string.Format("net.tcp://{0}:{1}/WCFConsole", host, port);
            Uri url = new Uri(s);
            this._channelFactory.Open();
            this._server = this._channelFactory.CreateChannel();
            this.timerPing.Enabled = true;
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public void Disconnect()
        {
            if (this._server != null)
            {
                try
                {
                    this._channelFactory.Close();
                }
                finally
                {
                    this._server = null;
                    this.timerPing.Enabled = false;
                }
            }
        }
    }
}
