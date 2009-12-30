using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace MonoPlug
{
    [System.Diagnostics.DebuggerDisplay("#{Id} {Name}")]
    public sealed class ClsPlayer : MarshalByRefObject
    {
        internal ClsPlayer()
        {
        }

        internal void SetId(int value) { this._id = value; }
        internal void SetName(string value) { this._name = value; }
        internal void SetSteamID(string value) { this._steamid = value; }
        internal void SetTeamIndex(int value) { this._teamIndex = value; }

        private int _id;
        private string _name;
        private string _steamid;
        private int _teamIndex;
        private int _frag;
        private int _death;
        private bool _isConnected;
        private int _armor;

        private bool _isPlayer;
        private bool _isHLTV;
        private bool _isFakeClient;
        private bool _isDead;
        private bool _isInAVehicle;
        private bool _isObserver;

        private TimeSpan _connected;
        private int _ping;
        private int _loss;
        private string _state;
        private IPEndPoint _address;

        public int Id { get { return this._id; } }
        public string Name { get { return this._name; } }
        public string SteamID { get { return this._steamid; } }
        public TimeSpan Connected { get { return this._connected; } }
        public int Ping { get { return this._ping; } }
        public int Loss { get { return this._loss; } }
        public string State { get { return this._state; } }
        public IPEndPoint Address { get { return this._address; } }
    }
}
