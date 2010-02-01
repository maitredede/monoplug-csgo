using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoPlug;
using System.Threading;

namespace MaitreDede.TnT.AntiBoulet
{
    public sealed class ClsAntiBoulet : ClsPluginBase
    {
        private Timer _timer;

        public override string Name
        {
            get { return "TnT-AntiBoulet"; }
        }

        public override string Description
        {
            get { return "TnT: Dump dans la console des admins les infos des joueurs, pour gérer les boulets"; }
        }

        protected override void Load()
        {
            this._timer = new Timer(this.Tick, null, 1, 30000);
        }

        protected override void Unload()
        {
            this._timer.Dispose();
        }

        private void Tick(object state)
        {
            //TODO
        }
    }
}
