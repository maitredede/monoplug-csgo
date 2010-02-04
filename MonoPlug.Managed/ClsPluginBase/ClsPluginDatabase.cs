using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Net;

namespace MonoPlug
{
    internal sealed class ClsPluginDatabase : MarshalByRefObject, IDatabase
    {
        private readonly IDatabaseConfig _db;
        private readonly ClsPluginBase _owner;
        private readonly IMessage _msg;

        internal ClsPluginDatabase(ClsPluginBase owner, IDatabaseConfig db, IMessage msg)
        {
#if DEBUG
            Check.NonNull("owner", owner);
            Check.NonNull("db", db);
            Check.NonNull("msg", msg);
#endif
            this._owner = owner;
            this._db = db;
            this._msg = msg;
        }

        MySqlConnection IDatabase.GetConnection()
        {
            try
            {
                MySqlConnection con = new MySqlConnection(this._db.GetConnectionString());
                con.Open();
                return con;
            }
            catch (Exception ex)
            {
                this._msg.Warning(ex);
                throw new InvalidOperationException("MySQL Connection error", ex);
            }
        }

        string IDatabase.GeoIP_GetCountry(IPAddress address)
        {
            Check.NonNull("address", address);

            string ret;

            using (MySqlConnection con = ((IDatabase)this).GetConnection())
            {
                using (MySqlTransaction tx = con.BeginTransaction())
                {
                    using (MySqlCommand com = con.CreateCommand())
                    {
                        com.Transaction = tx;

                        com.CommandText = "SELECT country FROM geoipcountry WHERE @num BETWEEN begin_num AND end_num LIMIT 1";
                        byte[] addr = address.GetAddressBytes();
                        long num = addr[0] * 256 << 3 + addr[1] * 256 << 2 + addr[2] * 256 + addr[3];
                        com.Parameters.AddWithValue("@num", num);

                        object country = com.ExecuteScalar();
                        if (country == null || country == DBNull.Value)
                        {
                            ret = null;
                        }
                        else
                        {
                            ret = (string)country;
                        }
                    }
                    tx.Commit();
                }
            }

            return ret;
        }
    }
}
