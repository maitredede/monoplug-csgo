using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ServiceModel;

namespace MonoPlug
{
    public partial class ClsWCFConsoleClient : Component, IConsoleClient
    {
        private readonly DuplexChannelFactory<IConsoleServer> _channelFactory;
        private IConsoleServer _server = null;

        public ClsWCFConsoleClient(IContainer container)
            : this()
        {
            container.Add(this);
        }

        public ClsWCFConsoleClient()
        {
            InitializeComponent();

            this._channelFactory = new DuplexChannelFactory<IConsoleServer>(this, "WCFConsole");
        }

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

        void IConsoleClient.ConsoleMessage(bool hasColor, bool debug, Color color, string message)
        {
            ConMessageEventArgs e = new ConMessageEventArgs(hasColor, debug, color, message);
            if (this.ConsoleMessage != null)
            {
                this.ConsoleMessage(this, e);
            }
        }

        public void Connect(string host, int port)
        {
            string s = string.Format("net.tcp://{0}:{1}/WCFConsole", host, port);
            Uri url = new Uri(s);
            this._channelFactory.Open();
            this._server = this._channelFactory.CreateChannel();
            this.timerPing.Enabled = true;
        }

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
