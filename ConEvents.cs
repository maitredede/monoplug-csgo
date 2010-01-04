using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;

namespace MonoPlug
{
    public sealed class ConEvents : ClsPluginBase
    {
        delegate void DT(EventHandler d);

        protected override void Load()
        {
            try
            {
                this.ClientCommand += this.ClientCommand_Sample;
                this.LevelShutdown += this.Events_LevelShutdown;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    Msg(ex.GetType().FullName + "\n");
                    Msg(ex.Message + "\n");
                    Msg(ex.StackTrace + "\n");
                    ex = ex.InnerException;
                }
            }
        }

        protected override void Unload()
        {
            try
            {
                Msg("ConEvents::Unload : A\n");
                this.LevelShutdown -= this.Events_LevelShutdown;
                this.ClientCommand -= this.ClientCommand_Sample;
                Msg("ConEvents::Unload : A\n");
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    Msg(ex.GetType().FullName + "\n");
                    Msg(ex.Message + "\n");
                    Msg(ex.StackTrace + "\n");
                    ex = ex.InnerException;
                }
            }
        }

        public override string Name
        {
            get { return "ConEvents"; }
        }

        public override string Description
        {
            get { return "Dump to console all events that managed code can handle."; }
        }

        private void Events_LevelShutdown(object sender, EventArgs e)
        {
            this.Msg("ConEvents: LevelShutdown\n");
        }

        private void ClientCommand_Sample(object sender, ClientCommandEventArgs e)
        {
            string name;
            if (e.Player == null)
            {
                name = "<null>";
            }
            else
            {
                name = e.Player.Name ?? "<player name is null>";
            }
            this.Msg("ConEvents: ClientCommand from {0} {1}\n", name);
        }
    }
}
