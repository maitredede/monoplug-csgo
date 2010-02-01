using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;

namespace MonoPlug
{
    /// <summary>
    /// Attach all possible raised events
    /// </summary>
    public sealed class ConEvents : ClsPluginBase
    {
        delegate void DT(EventHandler d);

        /// <summary>
        /// Load the plugin
        /// </summary>
        protected override void Load()
        {
            try
            {
                //this.ClientCommand += this.ClientCommand_Sample;
                this.LevelShutdown += this.Events_LevelShutdown;
                this.ConMessage += this.Events_ConMessage;

                this.ClientPutInServer += this.Events_ClientPutInServer;
                this.ClientDisconnect += this.Events_ClientDisconnect;
            }
            catch (Exception ex)
            {
                this.Message.Warning(ex);
            }
        }

        /// <summary>
        /// Unload the plugin
        /// </summary>
        protected override void Unload()
        {
            try
            {
                this.Message.Msg("ConEvents::Unload : A\n");
                this.LevelShutdown -= this.Events_LevelShutdown;
                //this.ClientCommand -= this.ClientCommand_Sample;
                this.ConMessage -= this.Events_ConMessage;
                this.ClientPutInServer -= this.Events_ClientPutInServer;
                this.ClientDisconnect -= this.Events_ClientDisconnect;
                this.Message.Msg("ConEvents::Unload : A\n");
            }
            catch (Exception ex)
            {
                this.Message.Warning(ex);
            }
        }

        /// <summary>
        /// Get plugin name
        /// </summary>
        public override string Name
        {
            get { return "ConEvents"; }
        }

        /// <summary>
        /// Get plugin description
        /// </summary>
        public override string Description
        {
            get { return "Dump to console all events that managed code can handle."; }
        }

        private void Events_LevelShutdown(object sender, EventArgs e)
        {
            this.Message.Msg("ConEvents: LevelShutdown\n");
        }

        private void Events_ClientCommand(object sender, ClientCommandEventArgs e)
        {
            string name;
            if (e.Client == null)
            {
                name = "<null>";
            }
            else
            {
                name = e.Client.Name ?? "<player name is null>";
            }
            this.Message.Msg("ConEvents: ClientCommand from {0} {1}\n", name);
        }

        private void Events_ClientDisconnect(object sender, ClientEventArgs e)
        {
            this.Message.Msg("Client disconnect : {0}\n", e.Client);
        }

        private void Events_ClientPutInServer(object sender, ClientEventArgs e)
        {
            this.Message.Msg("Client Put in server : {0}\n", e.Client);
        }

        private void Events_ConMessage(object sender, ConMessageEventArgs e)
        {
            //don't msg here, potential loopback
        }
    }
}
