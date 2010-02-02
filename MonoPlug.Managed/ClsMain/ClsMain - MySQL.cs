using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using MySql.Data;
using System.Threading;
using MySql.Data.MySqlClient;

namespace MonoPlug
{
    partial class ClsMain : IDatabase
    {
        MySqlConnection IDatabase.GetConnection()
        {
            this._lckConfig.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (this._configLoadedOK)
                {
                    if (this._config != null)
                    {
                        if (this._config.mysql != null)
                        {
                            MySqlConnectionStringBuilder mcsb = new MySqlConnectionStringBuilder();
                            mcsb.Server = this._config.mysql.host;
                            mcsb.UserID = this._config.mysql.user;
                            mcsb.Password = this._config.mysql.pass;
                            mcsb.Database = this._config.mysql.@base;
                            mcsb.Port = (uint)this._config.mysql.port;
                            try
                            {
                                MySqlConnection con = new MySqlConnection(mcsb.ConnectionString);
                                con.Open();
                                return con;
                            }
                            catch (Exception ex)
                            {
                                this._msg.Warning(ex);
                                throw new InvalidOperationException("MySQL Connection error", ex);
                            }
                        }
                        else
                        {
                            this._msg.Warning("Error in config file : mysql node not present\n");
                            throw new InvalidOperationException("Error in config file : mysql node not present");
                        }
                    }
                    else
                    {
                        this._msg.Warning("Empty config file\n");
                        throw new InvalidOperationException("Empty config file");
                    }
                }
                else
                {
                    this._msg.Warning("Config file not loaded\n");
                    throw new InvalidOperationException("Config file not loaded");
                }
            }
            finally
            {
                this._lckConfig.ReleaseReaderLock();
            }
        }
    }
}
