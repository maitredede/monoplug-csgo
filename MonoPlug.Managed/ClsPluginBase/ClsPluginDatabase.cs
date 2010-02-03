using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

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
    }
}
