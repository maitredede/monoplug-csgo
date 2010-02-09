using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Drawing;

namespace MonoPlug
{
    /// <summary>
    /// Attach all possible raised events
    /// </summary>
    public sealed class ConEvents : ClsPluginBase
    {
        private ClsConCommand _cmd;
        private ClsSayCommand _thetime;

        /// <summary>
        /// Load the plugin
        /// </summary>
        protected override void Load()
        {
            try
            {
                this.Events.ClientDisconnect += this.Events_ClientDisconnect;
                this.Events.ClientPutInServer += this.Events_ClientPutInServer;
                this.Events.ConsoleMessage += this.Events_ConMessage;
                this.Events.LevelShutdown += this.Events_LevelShutdown;
                this.Events.PlayerConnect += this.Events_PlayerConnect;
                this.Events.PlayerDisconnect += this.Events_PlayerDisconnect;
                this.Events.ServerActivate += this.Events_ServerActivate;
                this.Events.ServerShutdown += this.Events_ServerShutdown;
                this.Events.ServerSpawn += this.Events_ServerSpawn;

                this._cmd = this.Engine.RegisterConCommand("clr_test_cmd", "Test command that print command and arguments with indexes", FCVAR.FCVAR_NONE, this.CmdTest, null, false);

                this._thetime = this.Engine.RegisterSayCommand("thetime", true, false, this.TheTime);
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
            //this.Message.Msg("ConEvents: LevelShutdown\n");
        }

        private void Events_ClientDisconnect(object sender, ClientEventArgs e)
        {
            //this.Message.Msg("ConEvents::Events_ClientDisconnect : {0}\n", e.Client);
        }

        private void Events_ClientPutInServer(object sender, ClientEventArgs e)
        {
            //try
            //{
            //    this.Message.Msg("ConEvents::Events_ClientPutInServer : {0}\n", e.Client.Dump());
            //}
            //catch (Exception ex)
            //{
            //    this.Message.Warning(ex);
            //}
        }

        private void Events_PlayerConnect(object sender, ClientEventArgs e)
        {
            //string client;
            //if (e.Client == null)
            //{
            //    client = "<null>";
            //}
            //else
            //{
            //    client = e.Client.Dump();
            //}
            //this.Message.Msg("ConEvents::Events_PlayerConnect : {0}\n", client);
        }

        private void Events_ConMessage(object sender, ConMessageEventArgs e)
        {
            //don't msg here, potential loopback
        }

        private void Events_ServerShutdown(object sender, ReasonEventArgs e)
        {
            //this.Message.Msg("ConEvents::Events_ServerShutdown : {0}\n", e.Reason);
        }

        private void Events_ServerActivate(object sender, ServerActivateEventArgs e)
        {
            //this.Message.Msg("ConEvents::Events_ServerActivate : maxclients={0}\n", e.MaxClients);
        }

        private void Events_PlayerDisconnect(object sender, PlayerDisconnectEventArgs e)
        {
            this.Message.Msg("ConEvents::Events_PlayerDisconnect : {0} reason={1}\n", e.Client, e.Reason);
        }

        private void Events_ServerSpawn(object sender, ServerSpawnEventArgs e)
        {
            //this.Message.Msg("ConEvents::Events_ServerSpawn : {0}\n", e.GetFullString());
        }

        private void CmdTest(ClsPlayer sender, string line, string[] args)
        {
            if (sender == null)
            {
                this.Message.Msg("TEST : sender is Console\n");
            }
            else
            {
                this.Message.Msg("TEST : sender is {0}\n", sender);
            }
            this.Message.Msg("TEST : line={0}\n", line);
            for (int i = 0; i < args.Length; i++)
            {
                this.Message.Msg("TEST : args[{0}]={1}\n", i, args[i]);
            }
        }

        private void TheTime(ClsPlayer sender, string line, string[] args)
        {
#if DEBUG
            this.Message.DevMsg("TheTime triggered");
#endif
            string time = DateTime.Now.ToLongTimeString();
            if (sender == null)
            {
                this.Message.Msg("The time : {0}\n", time);
            }
            else
            {
                this.Engine.ClientDialog(sender, "The time", time, Color.Azure, 1, 5);
            }
        }
    }
}
