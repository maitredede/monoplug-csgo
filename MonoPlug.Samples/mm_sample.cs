using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    /// <summary>
    /// Sample_mm plugin ported to Mono
    /// </summary>
    public sealed class mm_sample : ClsPluginBase
    {
        private ClsConVar _sample_cvar;

        /// <summary>
        /// Get plugin name
        /// </summary>
        public override string Name
        {
            get { return "mm_sample"; }
        }

        /// <summary>
        /// Get plugin description
        /// </summary>
        public override string Description
        {
            get { return "Sample_mm plugin ported to Mono"; }
        }

        /// <summary>
        /// Load plugin
        /// </summary>
        protected override void Load()
        {
            //this.ClientCommand += this.ClientCommand_Sample;
            this.Message.Msg("Starting plugin.\n");
            this._sample_cvar = this.ConItems.RegisterConvar("sample_cvar_clr", "A sample convar", FCVAR.FCVAR_NONE, "42");

            //this.LevelInit += this.Sample_LevelInit;
            //this.ServerActivate += this.Sample_ServerActivate;
            //GameFrame is ommited for performance reasons
            this.Events.LevelShutdown += this.Sample_LevelShutdown;
            //this.ClientActive += this.Sample_ClientActive;
            this.Events.ClientDisconnect += this.Sample_ClientDisconnect;
            this.Events.ClientPutInServer += this.Sample_ClientPutInServer;
            //this.ClientSettingsChanged += this.Sample_ClientSettingsChanged;
            //this.ClientConnect += this.Sample_ClientConnect;
            //this.ClientCommand += this.Sample_ClientCommand;
            this.Message.Msg("All hooks started!\n");
        }

        private void Sample_ClientPutInServer(object sender, ClientEventArgs e)
        {
            //this.ClientDialogMessage(e.Client, "Hello", "Hello there", Color.FromArgb(255, 0, 0, 255), 5, 10);
        }

        private void Sample_ClientDisconnect(object sender, ClientEventArgs e)
        {
        }

        private void Sample_LevelShutdown(object sender, EventArgs e)
        {
            this.Message.Log("Sample_LevelShutdown()");
        }

        /// <summary>
        /// Unload plugin
        /// </summary>
        protected override void Unload()
        {
            //this.LevelInit -= this.Sample_LevelInit;
            //this.ServerActivate -= this.Sample_ServerActivate;
            //GameFrame is ommited for performance reasons
            this.Events.LevelShutdown -= this.Sample_LevelShutdown;
            //this.ClientActive -= this.Sample_ClientActive;
            this.Events.ClientDisconnect -= this.Sample_ClientDisconnect;
            this.Events.ClientPutInServer -= this.Sample_ClientPutInServer;
            //this.ClientSettingsChanged -= this.Sample_ClientSettingsChanged;
            //this.ClientConnect -= this.Sample_ClientConnect;
            //this.ClientCommand -= this.Sample_ClientCommand;

            this.ConItems.UnregisterConvar(this._sample_cvar);
        }

        //private void Sample_LevelShutdown(object sender, EventArgs e)
        //{
        //}

        //private void Sample_ServerActivate()
        //{
        //}

        //private void Sample_ClientActive(object sender, ClientActiveEventArgs e)
        //{
        //    this.Log("ClientActive({0},{1})", e.Client.Id, e.LoadGame);
        //}

        //private void Sample_ClientCommand(object sender, ClientCommandEventArgs e)
        //{
        //    if (e.Command.Equals("msg", StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        this.ClientDialogMessage(e.Client, "Just a simple hello", "Just a simple hello message", Color.White, 1, 20);
        //    }
        //}

        //private void Sample_ClientSettingsChanged(object sender, ClientEventArgs e)
        //{
        //}
    }
}
