using System.IO;
using System.Reflection;
using Kataclysm.Randomizer;
using Kataclysm.StructuralAnalysis;
using Kataclysm.StructuralAnalysis.Model;
using System.Data.SQLite;
using Kataclysm.Common;

namespace Kaboom
{
    class Program
    {
        private static SQLiteConnection _dbConn;
        
        static void Main(string[] args)
        {
            _dbConn = DB.CreateConnection();
            
            for (int i = 0; i < 100; i++)
            {
                // Randomize
                RandomizedBuilding randomized = Randomize.Random();
            
                // Generate Serialized Model from randomization
                //   Includes wall layouts, boundary info, and mass
                // TODO Anthonie to link up
                var serializedModel = new SerializedModel();
            
                // Feed it through the rigid analysis
                var manager = new AnalysisManager(serializedModel);
                WallCostCharacterization wallCostCharacterization = manager.Run();
            
                // Write to csv
                string executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string fpath = Path.Combine(executingPath, @"DataOutput\testData.csv");
                
                wallCostCharacterization.WriteToCSV(fpath);
            
                // Output to SQL Lite db
                wallCostCharacterization.WriteToDB(_dbConn);
            }
        }
    }
}
