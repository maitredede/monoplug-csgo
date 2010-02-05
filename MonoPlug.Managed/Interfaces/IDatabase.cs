using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Net;

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

        /// <summary>
        /// Get a contry code based on ip address
        /// </summary>
        /// <param name="address">Address to lookup</param>
        /// <returns>ISO Country code</returns>
        string GeoIP_GetCountry(IPAddress address);
        /// <summary>
        /// Get all defined admins
        /// </summary>
        /// <returns>Admin list</returns>
        ClsAdminEntry[] GetAdmins();
    }
}
