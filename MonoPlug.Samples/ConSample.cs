using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Sample_mm plugin ported to Mono
    /// </summary>
    public sealed class mm_sample : ClsPluginBase
    {
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
        }

        /// <summary>
        /// Unload plugin
        /// </summary>
        protected override void Unload()
        {
            //this.ClientCommand -= this.ClientCommand_Sample;
        }

        private void ClientCommand_Sample(object sender, ClientCommandEventArgs e)
        {
        }
    }
}
