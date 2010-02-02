using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace MonoPlug
{
    /// <summary>
    /// Interface for database
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Get a connection to database (don't forget to close/dispose the connection)
        /// </summary>
        /// <returns></returns>
        MySqlConnection GetConnection();
    }
}
