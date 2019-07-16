using System.Data.SQLite;
using Kataclysm.Common;
using NUnit.Framework;

namespace Common.Tests
{
    [TestFixture]
    public class DBTests
    {
        [Test]
        public void DB_CreateConnection_Succeeds()
        {
            var dbConn = DB.CreateConnection();
            
            
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = dbConn.CreateCommand();

            sqlite_cmd.CommandText = "INSERT INTO WallCostCharacterization(unit_mix) VALUES ('test');";
            sqlite_cmd.ExecuteNonQuery();
        }
    }
}