using System.Collections.Generic;
using Kataclysm.Common;

namespace Kataclysm.StructuralAnalysis.Model
{
    public class SerializedModel
    {
        // Project Settings
        public ModelSettings ModelSettings { get; set; }
        public SeismicParameters SeismicParameters { get; set; }

        // Loads
        public List<UniformLineLoad> LineLoads { get; set; }

        // Load Bearing Elements
        public List<BearingWall> BearingWalls { get; set; }
        public List<OneWayDeck> OneWayDecks { get; set; }
    }
}