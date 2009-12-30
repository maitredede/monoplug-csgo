using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace MonoPlug
{
    public sealed class ClientConnectEventArgs : EventArgs
    {
        internal ClientConnectEventArgs()
        {
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public IPEndPoint Address { get; private set; }
        public bool Authorized { get; set; }
        public string RejectReason { get; set; }
    }
}
