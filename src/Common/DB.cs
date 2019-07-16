using System;
using System.Data.SQLite;

namespace Kataclysm.Common
{
    public static class DB
    {
        public static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            string path = @"C:\_play\dbei-hackathon-na-2019\extern\DB\Consuela.db";
            
            sqlite_conn = new SQLiteConnection($"Data Source={path};Version=3;New=True;Compress=True;");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
 
            }
            return sqlite_conn;
        }
    }
}