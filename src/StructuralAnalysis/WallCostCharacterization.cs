using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;
using Kataclysm.Common;
using Kataclysm.Randomizer;

namespace Kataclysm.StructuralAnalysis
{
    public class WallCostCharacterization
    {
        public RandomizedBuilding RandomizedBuilding { get; set; }
        public double GrossSquareFeet { get; set; }
        public double TotalStructuralShearWallCost { get; set; }
        public bool MeetsWallDesignLimit { get; set; }
        public bool MeetsDriftLimit { get; set; }
        public Geometry Geometry { get; set; }

        public void WriteToCSV(string fpath)
        {
            using (StreamWriter sw = new StreamWriter(fpath))
            {
                var headers = new List<string>
                {
                    "unit_mix",
                    "typology",
                    "seismicity",
                    "number_of_stories",
                    "drift_limit",
                    "gross_square_feet",
                    "total_structural_shear_wall_cost",
                    "cost_per_square_foot",
                    "meets_wall_design_limit",
                    "meets_drift_limit"
                };

                var headerString = XFile.CreateCSVString(headers);
                
                sw.WriteLine(headerString);
                
                var items = new List<string>
                {
                    RandomizedBuilding.UnitMix.Description,
                    RandomizedBuilding.Typology.Description,
                    RandomizedBuilding.Seismicity.Description,
                    RandomizedBuilding.NumberOfStories.ToString(),
                    RandomizedBuilding.DriftLimit.ToString(),
                    GrossSquareFeet.ToString(),
                    TotalStructuralShearWallCost.ToString(),
                    (TotalStructuralShearWallCost / GrossSquareFeet).ToString(),
                    MeetsWallDesignLimit.ToString(),
                    MeetsDriftLimit.ToString()
                };

                var csvString = XFile.CreateCSVString(items);
                
                sw.WriteLine(csvString);
            }
        }

        public void WriteToDB(SQLiteConnection dbConn)
        {
             var insertionText = $"INSERT INTO WallCostCharacterization(" +
                  $"unit_mix, " +
                  $"typology, " +
                  $"seismicity, " +
                  $"number_of_stories, " +
                  $"drift_limit, " +
                  $"gross_square_feet, " +
                  $"total_structural_shear_wall_cost, " +
                  $"cost_per_square_foot, " +
                  $"meets_wall_design_limit, " +
                  $"meets_drift_limit)" +
                  $" VALUES (" +
                  $"'{RandomizedBuilding.UnitMix.Description}', " +
                  $"'{RandomizedBuilding.Typology.Description}', " +
                  $"'{RandomizedBuilding.Seismicity.Description}', " +
                  $"{RandomizedBuilding.NumberOfStories}, " +
                  $"{RandomizedBuilding.DriftLimit}, " +
                  $"{GrossSquareFeet}, " +
                  $"{TotalStructuralShearWallCost}, " +
                  $"{TotalStructuralShearWallCost / GrossSquareFeet}, " +
                  $"{MeetsWallDesignLimit}, " +
                  $"{MeetsDriftLimit});";
            
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = dbConn.CreateCommand();

            sqlite_cmd.CommandText = insertionText
            sqlite_cmd.ExecuteNonQuery();
        }
    }
}