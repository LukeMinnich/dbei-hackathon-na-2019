using Kataclysm.Randomizer;
using Kataclysm.StructuralAnalysis;
using Kataclysm.StructuralAnalysis.Model;

namespace Kaboom
{
    class Program
    {
        static void Main(string[] args)
        {
            // Randomize
            RandomizedBuilding randomized = Randomize.Random();
            
            // Generate Serialized Model from randomization
            //   Includes wall layouts, boundary info, and mass
            // TODO Anthonie to link up
            var serializedModel = new SerializedModel();
            
            // Feed it through the rigid analysis
            var manager = new AnalysisManager(serializedModel);
            manager.Run();

            // Determine wall costs
            
            
            // Output to SQL Lite db
            
        }
    }
}
