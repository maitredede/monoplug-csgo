using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MonoPlug
{
    /// <summary>
    /// Config file : MySQL Entry
    /// </summary>
    [XmlType("TMySQL")]
    [XmlRoot("mysql")]
    [Obsolete("Remove", true)]
    public sealed class ClsConfigMySQL
    {
        private string _host;
        private int _port;
        private string _user;
        private string _pass;
        private string _base;

        /// <summary>
        /// Server host
        /// </summary>
        [XmlAttribute("host")]
        public string Host { get { return this._host; } set { this._host = value; } }
        /// <summary>
        /// Server port
        /// </summary>
        [XmlAttribute("port")]
        public int Port { get { return this._port; } set { this._port = value; } }
        /// <summary>
        /// Db User
        /// </summary>
        [XmlAttribute("user")]
        public string User { get { return this._user; } set { this._user = value; } }
        /// <summary>
        /// User password
        /// </summary>
        [XmlAttribute("pass")]
        public string Pass { get { return this._pass; } set { this._pass = value; } }
        /// <summary>
        /// Default database
        /// </summary>
        [XmlAttribute("base")]
        public string Base { get { return this._base; } set { this._base = value; } }
    }
}
