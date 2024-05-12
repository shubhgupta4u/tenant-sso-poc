using Microsoft.Data.Sqlite;
using System;
using System.Data;

namespace tenant_sso_poc.data
{
    public class DataContext
    {
        private static readonly Lazy<DataContext> lazy = new Lazy<DataContext>(() => new DataContext());

        public static DataContext Instance { get { return lazy.Value; } }

        private DataContext()
        {
            SQLitePCL.Batteries.Init();
        }

        public IDbConnection CreateConnection()
        {
            //SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            return new SqliteConnection(Constants.CONNECTION_STRING);
        }        
    }
}
