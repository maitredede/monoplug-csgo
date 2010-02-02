using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace MonoPlug
{
    partial class ClsMain
    {
        #region Config File IO
        private readonly object _lckConfigFileIO = new object();

        private bool LoadConfigNoLock(out TConfig conf)
        {
            string path = Path.Combine(this._assemblyPath, "config.xml");
#if DEBUG
            this._msg.DevMsg("Loading config file {0}\n", path);
#endif
            bool ok = false;
            try
            {
                lock (this._lckConfigFileIO)
                {
                    using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        XmlSerializer xz = new XmlSerializer(typeof(TConfig));
                        conf = (TConfig)xz.Deserialize(fs);
                        ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                this._msg.Warning(ex);
                ok = false;
                conf = null;
            }
            return ok;
        }

        private bool SaveConfigNoLock(TConfig conf)
        {
            if (conf == null) return false;

            string path = Path.Combine(this._assemblyPath, "config.xml");
#if DEBUG
            this._msg.DevMsg("Saving config file {0}\n", path);
#endif
            bool ok = false;
            try
            {
                lock (this._lckConfigFileIO)
                {
                    using (FileStream fs = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                    {
                        XmlSerializer xz = new XmlSerializer(typeof(TConfig));
                        xz.Serialize(fs, conf);
                        ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                this._msg.Warning(ex);
                ok = false;
            }
            return ok;
        }
        #endregion
    }
}
