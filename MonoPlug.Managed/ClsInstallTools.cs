﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading;
using System.Reflection;

namespace MonoPlug
{
    /// <summary>
    /// Plugin containing tools for installation
    /// </summary>
    [DontListPlugin]
    public sealed class ClsInstallTools : ClsPluginBase
    {
        private ClsConCommand _test_all;
        private ClsConCommand _test_mysql;
        private ClsConCommand _geoip_install;
        private ClsConCommand _versions = null;

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
            this._test_all = this.Engine.RegisterConCommand("clr_test_all", "Do various tests", FCVAR.FCVAR_NONE, this.Test_all, null, true);
            this._test_mysql = this.Engine.RegisterConCommand("clr_mysql_test", "Test MySQL connection", FCVAR.FCVAR_NONE, this.MySQL_Test, null, true);
            this._geoip_install = this.Engine.RegisterConCommand("clr_geoip_install", "Create GeoIP table and download data", FCVAR.FCVAR_NONE, this.InstallGeoIP, null, true);
            this._versions = this.Engine.RegisterConCommand("clr_versions", "Print versions of Mono and libs used for core", FCVAR.FCVAR_NONE, this.clr_versions, null, false);
        }

        /// <summary>
        /// Plugin unload
        /// </summary>
        protected override void Unload()
        {
            this.Engine.UnregisterConCommand(this._versions);
            this.Engine.UnregisterConCommand(this._versions);
            this.Engine.UnregisterConCommand(this._geoip_install);
            this.Engine.UnregisterConCommand(this._test_all);
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

        private readonly object _geoLock = new object();
        private bool _geoRunning = false;

        private void InstallGeoIP(string line, string[] args)
        {
            lock (this._geoLock)
            {
                if (this._geoRunning)
                {
                    this.Message.Msg("Operation already running...\n");
                    return;
                }
                this._geoRunning = true;
            }
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.CreateDefault(new Uri("http://geolite.maxmind.com/download/geoip/database/GeoIPCountryCSV.zip"));
                this.Message.Msg("Sending request...\n");
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
                        using (MySqlCommand com = con.CreateCommand())
                        {
                            //com.Transaction = tx;

                            com.CommandText = "DROP TABLE IF EXISTS `geoipcountry`";
                            com.ExecuteNonQuery();
                            com.CommandText = @"CREATE TABLE `geoipcountry` (
  `begin_ip` varchar(15) NOT NULL,
  `end_ip` varchar(15) NOT NULL,
  `begin_num` bigint(20) NOT NULL,
  `end_num` bigint(20) NOT NULL,
  `country` varchar(4) NOT NULL,
  `name` varchar(100) NOT NULL
) ENGINE=INNODB DEFAULT CHARSET=utf8";
                            com.ExecuteNonQuery();
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (Stream webstream = response.GetResponseStream())
                                {
                                    int read;
                                    byte[] buffer = new byte[10240];
                                    while ((read = webstream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        ms.Write(buffer, 0, read);

                                        if (length > 0)
                                        {
                                            this.Message.DevMsg("Downloading : {0} / {1} ({2:F}%)\n", ms.Length, length, (double)ms.Length / (double)length * (double)100);
                                        }
                                        else
                                        {
                                            this.Message.DevMsg("Downloading : {0}\n", ms.Length);
                                        }
                                    }
                                }

                                ms.Seek(0, SeekOrigin.Begin);

                                this.Message.DevMsg("Opening zip file...\n");
                                using (ZipInputStream zis = new ZipInputStream(ms))
                                {
                                    this.Message.DevMsg("Getting file entry... CanDecompressEntry={0}\n", zis.CanDecompressEntry);
                                    ZipEntry entry = zis.GetNextEntry();
                                    if (entry != null)
                                    {
                                        string tmp = Path.GetTempFileName();
                                        using (FileStream fs = File.Open(tmp, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                                        {
                                            int read;
                                            byte[] buffer = new byte[10240];
                                            while ((read = zis.Read(buffer, 0, buffer.Length)) > 0)
                                            {
                                                fs.Write(buffer, 0, read);
                                            }
                                            fs.Flush();
                                        }

                                        MySqlBulkLoader loader = new MySqlBulkLoader(con);
                                        loader.ConflictOption = MySqlBulkLoaderConflictOption.None;
                                        loader.FieldQuotationCharacter = '"';
                                        loader.FieldTerminator = ",";
                                        loader.FileName = tmp;
                                        loader.LineTerminator = "\n";
                                        loader.NumberOfLinesToSkip = 0;
                                        loader.TableName = "geoipcountry";
                                        int count = loader.Load();
                                        this.Message.Msg("Lines added : {0}\n", count);

                                        com.Parameters.Clear();

                                        com.CommandText = "CREATE INDEX `IDX_geoipcountry_begin_num` ON `geoipcountry` (`begin_num`)";
                                        com.ExecuteNonQuery();
                                        com.CommandText = "CREATE INDEX `IDX_geoipcountry_end_num` ON `geoipcountry` (`end_num`)";
                                        com.ExecuteNonQuery();

                                        com.CommandText = "CREATE INDEX `IDX_geoipcountry_begin_ip` ON `geoipcountry` (`begin_ip`)";
                                        com.ExecuteNonQuery();
                                        com.CommandText = "CREATE INDEX `IDX_geoipcountry_end_ip` ON `geoipcountry` (`end_ip`)";
                                        com.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        this.Message.Warning("No file to decompress\n");
                                    }
                                }
                            }
                        }
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
                lock (this._geoLock)
                {
                    this._geoRunning = false;
                }
                this.Message.Msg("GeoIP Done.\n");
            }
        }

        private void clr_versions(string line, string[] args)
        {
            this.Message.Msg("Mono version : {0}\n", ClsMain.GetMonoVersion());
            try
            {
                this.WriteAssemblyVersion(typeof(MySql.Data.MySqlClient.MySqlConnection), "MySQL version : {0}\n");
            }
            catch (Exception ex)
            {
                this.Message.Warning(ex);
            }
            try
            {
                this.WriteAssemblyVersion(typeof(ICSharpCode.SharpZipLib.Zip.ZipEntry), "SharpZip version : {0}\n");
            }
            catch (Exception ex)
            {
                this.Message.Warning(ex);
            }
        }

        private void WriteAssemblyVersion(Type type, string format)
        {
            try
            {
                AssemblyName asmName = type.Assembly.GetName();
                if (asmName != null)
                {
                    this.Message.Msg(format, asmName.Version.ToString(4));
                }
                else
                {
                    object[] arr = type.Assembly.GetCustomAttributes(typeof(AssemblyVersionAttribute), true);
                    if (arr != null && arr.Length > 0)
                    {
                        foreach (AssemblyVersionAttribute att in arr)
                        {
                            this.Message.Msg(format, att.Version);
                        }
                    }
                    else
                    {
                        this.Message.Msg(format, "no version");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Message.Warning(format, ex.Message);
            }
        }

        private void Test_all(string line, string[] args)
        {
        }
    }
}
