using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoPlug;

namespace TSServerPlugin
{
    public sealed class ClsTSServer : ClsPluginBase
    {
        private INativeMethods _dll;
        private ClsServer _server;

        public override string Name
        {
            get { return "TS-Server"; }
        }

        public override string Description
        {
            get { return "TeamSpeak 3 Server"; }
        }

        protected override void Load()
        {
            this.Message.Msg("OS is : {0}\n", Environment.OSVersion.VersionString);
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    this._dll = new NativeMethods_Win32();
                    break;
                case PlatformID.Unix:
                    this._dll = new NativeMethods_Linux32();
                    break;
                default:
                    throw new PlatformNotSupportedException(string.Format("Plateform '{0}' not supported by this plugin", Environment.OSVersion.VersionString));
            }
            this._server = new ClsServer(this._dll);
        }

        protected override void Unload()
        {
            if (this._server != null)
            {
                try
                {
                    using (this._server)
                    {
                    }
                }
                finally
                {
                    this._server = null;
                }
            }
        }
    }
}
