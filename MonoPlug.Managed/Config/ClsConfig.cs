using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MonoPlug
{
    [XmlType("TConfig")]
    [XmlRoot("config")]
    public sealed class ClsConfig
    {
        private List<ClsConfigPlugin> _lst = new List<ClsConfigPlugin>();
        private ClsConfigMySQL _mysql;

        [XmlArray("plugin")]
        public List<ClsConfigPlugin> Plugin { get { return this._lst; } set { this._lst = value; } }
        [XmlElement("mysql")]
        public ClsConfigMySQL MySQL { get { return this._mysql; } set { this._mysql = value; } }
    }
}
