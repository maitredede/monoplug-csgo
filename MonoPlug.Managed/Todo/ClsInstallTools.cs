using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;
using LumenWorks.Framework.IO.Csv;
using System.Threading;

namespace MonoPlug
{
    /// <summary>
    /// Plugin containing tools for installation
    /// </summary>
    [DontListPlugin]
    public sealed class ClsInstallTools : ClsPluginBase
    {
        private ClsConCommand _test_mysql;
        private ClsConCommand _geoip_install;

        /// <summary>
        /// Plugin name
        /// </summary>
        public override string Name
        {
            get { return "InstallTools"; }
        }

        /// <summary>
        /// Plugin description
        /// </summary>
        public override string Description
        {
            get { return "Tools to help install MonoPlug"; }
        }

        /// <summary>
        /// Plugin load
        /// </summary>
        protected override void Load()
        {
#if DEBUG
            this.Message.DevMsg("{0}: ({1} : {2})\n", this.Name, "loading", "A");
#endif
            this._test_mysql = this.ConItems.RegisterConCommand("clr_mysql_test", "Test MySQL connection", FCVAR.FCVAR_NONE, this.MySQL_Test, null, true);
#if DEBUG
            this.Message.DevMsg("{0}: ({1} : {2})\n", this.Name, "loading", "B");
#endif
            this._geoip_install = this.ConItems.RegisterConCommand("clr_geoip_install", "Create GeoIP table and download data", FCVAR.FCVAR_NONE, this.InstallGeoIP, null, true);
#if DEBUG
            this.Message.DevMsg("{0}: ({1} : {2})\n", this.Name, "loading", "X");
#endif
        }

        /// <summary>
        /// Plugin unload
        /// </summary>
        protected override void Unload()
        {
            this.ConItems.UnregisterConCommand(this._geoip_install);
            this.ConItems.UnregisterConCommand(this._test_mysql);
        }

        private void MySQL_Test(string line, string[] args)
        {
            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection con = this.Database.GetConnection())
                {
                    this.Message.Msg("MySQL test : Connection OK\n");
                    using (MySql.Data.MySqlClient.MySqlTransaction tx = con.BeginTransaction())
                    {
                        this.Message.Msg("MySQL test : Transaction OK\n");
                        using (MySql.Data.MySqlClient.MySqlCommand com = con.CreateCommand())
                        {
                            com.Transaction = tx;
                            this.Message.Msg("MySQL test : Command OK\n");

                            com.CommandText = "SELECT NOW()";
                            object result = com.ExecuteScalar();
                            if (result != null)
                            {
                                this.Message.Msg("MySQL test : server NOW() = {0}\n", result);
                            }
                            else
                            {
                                this.Message.Msg("MySQL test : server NOW() = {0}\n", "<null>");
                            }

                            com.CommandText = "SELECT @@version";
                            result = com.ExecuteScalar();
                            if (result != null)
                            {
                                this.Message.Msg("MySQL test : server version : {0}\n", result);
                            }
                            else
                            {
                                this.Message.Msg("MySQL test : server version : {0}\n", "<null>");
                            }
                        }

                        tx.Rollback();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Message.Warning(ex);
            }
            finally
            {
                this.Message.Msg("MySQL test ended\n");
            }
        }

        private long _geo = 0;

        private void InstallGeoIP(string line, string[] args)
        {
            if (Interlocked.CompareExchange(ref this._geo, 1, 0) == 0)
            {
                this.Message.Msg("Operation already running...\n");
                return;
            }
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.CreateDefault(new Uri("http://geolite.maxmind.com/download/geoip/database/GeoIPCountryCSV.zip"));
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    long length = response.ContentLength;
                    if (length >= 0)
                    {
                        this.Message.Msg("Downloading file, size is {0}\n", length);
                    }
                    else
                    {
                        this.Message.Msg("Downloading file...\n");
                    }

                    using (MySqlConnection con = this.Database.GetConnection())
                    {
                        using (MySqlTransaction tx = con.BeginTransaction())
                        {
                            using (MySqlCommand com = con.CreateCommand())
                            {
                                com.Transaction = tx;

                                com.CommandText = "DROP TABLE IF EXISTS geoipcountry";
                                com.ExecuteNonQuery();
                                com.CommandText = @"CREATE TABLE `geopipcountry` (
  `begin_ip` varchar(15) NOT NULL,
  `end_ip` varchar(15) NOT NULL,
  `begin_num` bigint(20) NOT NULL,
  `end_num` bigint(20) NOT NULL,
  `country` varchar(4) NOT NULL,
  `name` varchar(100) NOT NULL,
  KEY `idx_ip` (`begin_ip`,`end_ip`),
  KEY `idx_num` (`begin_num`,`end_num`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8";
                                com.ExecuteNonQuery();

                                using (Stream webstream = response.GetResponseStream())
                                {
                                    using (ZipInputStream zis = new ZipInputStream(webstream))
                                    {
                                        ZipEntry entry = zis.GetNextEntry();
                                        if (entry != null)
                                        {
                                            using (StreamReader sr = new StreamReader(zis))
                                            {
                                                com.CommandText = @"INSERT INTO geopipcountry (`begin_ip`, `end_ip`, `begin_num`, `end_num`, `country`, `name`)
                                                    VALUES (@bi, @ei, @bn, @en, @c, @n)";
                                                MySqlParameter[] p = new MySqlParameter[6];
                                                p[0] = com.Parameters.Add("@bi", MySqlDbType.VarChar);
                                                p[1] = com.Parameters.Add("@ei", MySqlDbType.VarChar);
                                                p[2] = com.Parameters.Add("@bn", MySqlDbType.Int64);
                                                p[3] = com.Parameters.Add("@en", MySqlDbType.Int64);
                                                p[4] = com.Parameters.Add("@c", MySqlDbType.VarChar);
                                                p[5] = com.Parameters.Add("@n", MySqlDbType.VarChar);
                                                com.Prepare();
                                                using (CsvReader csv = new CsvReader(sr, false))
                                                {
                                                    int count = 0;
                                                    while (csv.ReadNextRecord())
                                                    {
                                                        for (int i = 0; i < p.Length; i++)
                                                        {
                                                            p[i].Value = csv[i];
                                                        }
                                                        count += com.ExecuteNonQuery();
                                                    }
                                                    this.Message.Warning("Lines added : {0}\n");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            this.Message.Warning("No file to decompress\n");
                                        }
                                    }
                                }
                            }
                            tx.Commit();
                        } //using Tx
                    }
                }
                else
                {
                    this.Message.Warning("Can't get file : {0}\n", response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                this.Message.Warning(ex);
            }
            finally
            {
                Interlocked.Exchange(ref this._geo, 0);
                this.Message.Msg("GeoIP Done.\n");
            }
        }
    }
}
