using System.Collections.Generic;
using BuildingLayout;
using Kataclysm.Common;
using Kataclysm.Randomizer;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.StructuralAnalysis.Model
{
    public class SerializedModel
    {
        // Project Settings
        public ModelSettings ModelSettings { get; set; }
        public SeismicParameters SeismicParameters { get; set; }

        // Load Bearing Elements
        public List<BearingWall> BearingWalls { get; set; }
        public List<OneWayDeck> OneWayDecks { get; set; }
        public List<Polygon2D> UnitBoundaries { get; set; }
        
        // Building Config
        public RandomizedBuilding RandomizedBuilding { get; set; }
    }
}