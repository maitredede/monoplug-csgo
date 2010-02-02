using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class ClsPluginDatabase : MarshalByRefObject, IDatabase
    {
        private readonly IDatabase _db;
        private readonly ClsPluginBase _owner;

        internal ClsPluginDatabase(ClsPluginBase owner, IDatabase db)
        {
#if DEBUG
            Check.NonNull("owner", owner);
            Check.NonNull("db", db);
#endif
            this._owner = owner;
            this._db = db;
        }

        public MySql.Data.MySqlClient.MySqlConnection GetConnection()
        {
            return this._db.GetConnection();
        }
    }
}
