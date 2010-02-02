using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MonoPlug
{
    /// <summary>
    /// Config file : root
    /// </summary>
    [XmlType("TConfig")]
    [XmlRoot("config")]
    [Obsolete("Remove", true)]
    public sealed class ClsConfig
    {
        private List<ClsConfigPlugin> _lst = new List<ClsConfigPlugin>();
        private ClsConfigMySQL _mysql;

        /// <summary>
        /// MySQL Configuration
        /// </summary>
        [XmlElement("mysql")]
        public ClsConfigMySQL MySQL { get { return this._mysql; } set { this._mysql = value; } }
        /// <summary>
        /// Plugin entries
        /// </summary>
        [XmlArray("plugin")]
        public List<ClsConfigPlugin> Plugin { get { return this._lst; } set { this._lst = value; } }
    }
}
