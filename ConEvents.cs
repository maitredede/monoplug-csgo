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
                Msg("ConEvents:: Load in AppDomain '{0}'\n", AppDomain.CurrentDomain.FriendlyName);
                if (this.Events != null)
                {
                    EventHandler e = new EventHandler(this.Events_LevelShutdown);
                    foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        Msg("Loaded : assembly name={0}\n", a.FullName);
                    }

                    Msg("ConEvents::Load A1\n");
                    Msg(this.Events.ToString() + "\n");
                    DT add = new DT(this.Events.LevelShutdown_Add);
                    Msg("ConEvents::Load A2\n");
                    add.Invoke(this.Events_LevelShutdown);
                    //this.Events.LevelShutdown_Add(this.Events_LevelShutdown);
                    Msg("ConEvents::Load A3\n");
                }
                else
                {
                    Msg("ConEvents::Load E1\n");
                }
                Msg("ConEvents::Loaded\n");
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
            this.Events.LevelShutdown_Remove(this.Events_LevelShutdown);
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

    }
}
