using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace MonoPlug
{
    public sealed class ClientConnectEventArgs : EventArgs
    {
		private int _id;
		private string _name;
		private IPEndPoint _address;
		
        internal ClientConnectEventArgs(int id, string name, IPEndPoint address)
        {
			this._id=id;
			this._name=name;
			this._address=address;
        }

        public int Id { get{return this._id; }}
        public string Name { get{return this._name;} }
        public IPEndPoint Address { get{return this._address;}}
        //public bool Authorized { get; set; }
        //public string RejectReason { get; set; }
    }
}
