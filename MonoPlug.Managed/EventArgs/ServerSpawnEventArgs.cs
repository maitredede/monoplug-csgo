using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Server has spawned
    /// </summary>
    public sealed class ServerSpawnEventArgs : EventArgs
    {
        internal ServerSpawnEventArgs(string hostname, string address, string port, string game, string mapname, int maxplayers, string os, bool dedicated, bool password)
        {
            this._hostname = hostname;
            this._address = address;
            this._port = port;
            this._game = game;
            this._mapname = mapname;
            this._maxplayers = maxplayers;
            this._os = os;
            this._dedicated = dedicated;
            this._password = password;
        }

        private string _hostname;
        private string _address;
        private string _port;
        private string _game;
        private string _mapname;
        private int _maxplayers;
        private string _os;
        private bool _dedicated;
        private bool _password;

        /// <summary>
        /// Return a string of all vars
        /// </summary>
        /// <returns></returns>
        public object GetFullString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("hostname={0} ", this._hostname);
            sb.AppendFormat("address={0} ", this._address);
            sb.AppendFormat("port={0} ", this._port);
            sb.AppendFormat("game={0} ", this._game);
            sb.AppendFormat("mapname={0} ", this._mapname);
            sb.AppendFormat("maxplayers={0} ", this._maxplayers);
            sb.AppendFormat("os={0} ", this._os);
            sb.AppendFormat("dedicated={0} ", this._dedicated);
            sb.AppendFormat("password={0}", this._password);
            return sb.ToString();
        }

        /// <summary>
        /// Hostname
        /// </summary>
        public string Hostname { get { return this._hostname; } }
        /// <summary>
        /// Address
        /// </summary>
        public string Address { get { return this._address; } }
        /// <summary>
        /// Port
        /// </summary>
        public string Port { get { return this._port; } }
        /// <summary>
        /// Game
        /// </summary>
        public string Game { get { return this._game; } }
        /// <summary>
        /// Map name
        /// </summary>
        public string Mapname { get { return this._mapname; } }
        /// <summary>
        /// Max players
        /// </summary>
        public int MaxPlayers { get { return this._maxplayers; } }
        /// <summary>
        /// Operating system
        /// </summary>
        public string OS { get { return this._os; } }
        /// <summary>
        /// Dedicated server
        /// </summary>
        public bool Dedicated { get { return this._dedicated; } }
        /// <summary>
        /// Password protected
        /// </summary>
        public bool Password { get { return this._password; } }
    }
}
