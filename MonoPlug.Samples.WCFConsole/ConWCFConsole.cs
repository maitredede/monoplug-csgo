using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ServiceModel;
using System.Threading;
using System.Net;
using System.ServiceModel.Description;

namespace MonoPlug
{
    /// <summary>
    /// WCFConsole plugin
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, Name = "ConWCFConsole")]
    public sealed class ConWCFConsole : ClsPluginBase, IConsoleServer
    {
        private readonly ReaderWriterLock _lck = new ReaderWriterLock();
        private readonly List<IConsoleClient> _clients = new List<IConsoleClient>();
        private readonly NetTcpBinding _netTcp = new NetTcpBinding(SecurityMode.None, false);

        private ClsConVar _wcfconsole_port;
        private ServiceHost _host;
        private readonly object _lckHost = new object();

        /// <summary>
        /// Get plugin name
        /// </summary>
        public override string Name
        {
            get { return "WCFConsole"; }
        }

        /// <summary>
        /// Get plugin description
        /// </summary>
        public override string Description
        {
            get { return "Remote console over a WCF service"; }
        }

        /// <summary>
        /// Load the plugin
        /// </summary>
        protected override void Load()
        {
            this._wcfconsole_port = this.ConItem.RegisterConvar("wcfconsole_port", "WCF Console listen port", FCVAR.FCVAR_NONE, "28001");
            this._wcfconsole_port.ValueChanged += this._wcfconsole_port_ValueChanged;
#if DEBUG
            this.Message.DevMsg("WCFConsole: Console Attaching\n");
#endif
            this.ConMessage += this.ConWCFConsole_ConMessage;

#if DEBUG
            this.Message.DevMsg("WCFConsole: Console Attached\n");
#endif

            this.InitHost();
        }

        private void _wcfconsole_port_ValueChanged(object sender, EventArgs e)
        {
            lock (this._lckHost)
            {
                if (this._host != null)
                {
                    if (this._host.State != CommunicationState.Closed)
                        this._host.Close();
                }

                this.InitHost();
            }
        }

        private void InitHost()
        {
#if DEBUG
            this.Message.DevMsg("WCFConsole::InitHost ({0})\n", "enter");
            try
            {
#endif
                //TODO : reinit host
                string hostName = Dns.GetHostName();
                Uri netTcpUri = new Uri(string.Format("net.tcp://{0}:{1}", hostName, this._wcfconsole_port.ValueString));
                this._host = new ServiceHost(this, netTcpUri);
                this._host.AddServiceEndpoint(typeof(IConsoleServer), this._netTcp, "WCFConsole");

#if DEBUG
                this.Message.Msg("WCF : Open host");
                this.DumpHost("TCP", this._host);
#endif
                this._host.Open();
#if DEBUG
            }
            catch (Exception ex)
            {
                this.Message.Warning(ex);
            }
            finally
            {
                this.Message.DevMsg("WCFConsole::InitHost ({0})\n", "exit");
            }
#endif
        }

        /// <summary>
        /// Unload the plugin
        /// </summary>
        protected override void Unload()
        {
            this.ConMessage -= this.ConWCFConsole_ConMessage;

            this.ConItem.UnregisterConvar(this._wcfconsole_port);

            if (this._host != null)
            {
                this._host.Close();
            }
        }

        private void ConWCFConsole_ConMessage(object sender, ConMessageEventArgs e)
        {
            this._lck.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (IConsoleClient client in this._clients)
                {
                    client.BeginConsoleMessage(e.HasColor, e.IsDebug, e.Color, e.Message, this.End_ConMessage, client);
                    //ThreadPool.QueueUserWorkItem(this.SendMessage, new Data(client, e));
                }
            }
            finally
            {
                this._lck.ReleaseReaderLock();
            }
        }

        private void End_ConMessage(IAsyncResult ar)
        {
            IConsoleClient client = (IConsoleClient)ar.AsyncState;
            try
            {
                client.EndConsoleMessage(ar);
            }
            catch (Exception ex)
            {
                this.Message.Warning("WCFConsole: Client connection error\n");
                this.Message.Warning(ex);
                this._lck.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    if (this._clients.Contains(client))
                    {
                        this._clients.Remove(client);
                    }
                }
                finally
                {
                    this._lck.ReleaseWriterLock();
                }
            }
        }

        //private struct Data
        //{
        //    public IConsoleClient Client;
        //    public ConMessageEventArgs ConsoleMessage;
        //    public Data(IConsoleClient client, ConMessageEventArgs e)
        //    {
        //        this.Client = client;
        //        this.ConsoleMessage = e;
        //    }
        //}

        //private void SendMessage(object state)
        //{
        //    Data data = (Data)state;
        //    try
        //    {
        //        data.Client.ConsoleMessage(data.ConsoleMessage.HasColor, data.ConsoleMessage.IsDebug, data.ConsoleMessage.Color, data.ConsoleMessage.Message);
        //    }
        //    catch
        //    {
        //        this._lck.AcquireWriterLock(Timeout.Infinite);
        //        try
        //        {
        //            if (this._clients.Contains(data.Client))
        //            {
        //                this._clients.Remove(data.Client);
        //            }
        //        }
        //        finally
        //        {
        //            this._lck.ReleaseWriterLock();
        //        }
        //    }
        //}

        void IConsoleServer.Ping()
        {
            IConsoleClient client = OperationContext.Current.GetCallbackChannel<IConsoleClient>();
            this._lck.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (!this._clients.Contains(client))
                {
                    LockCookie cookie = this._lck.UpgradeToWriterLock(Timeout.Infinite);
                    try
                    {
                        this._clients.Add(client);

                    }
                    finally
                    {
                        this._lck.DowngradeFromWriterLock(ref cookie);
                    }
                }
            }
            finally
            {
                this._lck.ReleaseReaderLock();
            }
        }

#if DEBUG
        public void DumpHost(string name, ServiceHost host)
        {
            foreach (var behavior in host.Description.Behaviors)
            {
                WriteLine(name, "DescBehavior", behavior.GetType().Name);
            }
            foreach (Uri address in host.BaseAddresses)
            {
                WriteLine(name, "BaseAddress", address.ToString());
            }
            foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
            {
                WriteLine(name, "Endpoint", endpoint.Address.ToString());
                WriteLine(name, "  Binding", endpoint.Binding.GetType().Name);
                foreach (var behavior in endpoint.Behaviors)
                {
                    WriteLine(name, "    Behavior", behavior.GetType().Name);
                }
            }
            foreach (var ext in host.Extensions)
            {
                WriteLine(name, "Extension", ext.GetType().Name);
            }
        }

        private void WriteLine(string type, string name, string value)
        {
            this.Message.Msg("{0}: {1}: {2}\n", type, name, value);
        }
#endif

        internal void Raise_ConMessage(bool hasColor, bool debug, int r, int g, int b, int a, string msg)
        {
            //Build color dic
            Dictionary<Color, ConsoleColor> _base = new Dictionary<Color, ConsoleColor>();
            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                string s = cc.ToString();
                Color c = (Color)typeof(Color).GetField(s).GetValue(null);
                _base.Add(c, cc);
            }

            float fa = 0.5f;
            float fb = 0.5f;
            float fc = 1f;

            float h1, s1, l1;

            //Find nearest color
            Color nearest = Color.FromArgb(a, r, g, b);
            h1 = nearest.GetHue();
            s1 = nearest.GetSaturation();
            l1 = nearest.GetBrightness();
            float original = fa * h1 * h1 + fb * s1 * s1 + fc * l1 * l1;
            float nearestDiff = float.MaxValue;
            foreach (Color test in _base.Keys)
            {
                h1 = test.GetHue();
                s1 = test.GetSaturation();
                l1 = test.GetBrightness();
                float testfact = fa * h1 * h1 + fb * s1 * s1 + fc * l1 * l1;
                float diff = Math.Abs(testfact - original);
                if (diff < nearestDiff)
                {
                    nearest = test;
                    nearestDiff = diff;
                }
            }

            Console.ForegroundColor = _base[nearest];
            Console.Write(msg);
        }
    }
}
