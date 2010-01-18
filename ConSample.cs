using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    public sealed class ConSample : ClsPluginBase
    {
        public override string Name
        {
            get { return "ConSample"; }
        }

        public override string Description
        {
            get { return "Sample Metamod Source plugin ported to Mono"; }
        }

        protected override void Load()
        {
            //this.ClientCommand += this.ClientCommand_Sample;
        }

        protected override void Unload()
        {
            //this.ClientCommand -= this.ClientCommand_Sample;
        }

        private void ClientCommand_Sample(object sender, ClientCommandEventArgs e)
        {
        }
    }
}
