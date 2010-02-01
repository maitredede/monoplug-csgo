using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace MonoPlug
{
    public interface IDatabase
    {
        MySqlConnection GetConnection();
    }
}
