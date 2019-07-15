using System;
using System.Collections.Generic;
using Kataclysm.Common;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.StructuralAnalysis.Model
{
    public class BearingWall : IReportsLevel
    {
        public string UniqueId { get; set; }
        public string GroupID { get; set; }
        public string PierID { get; set; }
        public Point3D? EndI { get; set; }
        public Point3D? EndJ { get; set; }
//        public BuildingMaterial? MaterialCategory { get; set; }
        public double? Thickness { get; set; }
        public BuildingLevel TopLevel { get; set; }
        public BuildingLevel BottomLevel { get; set; }
        public List<string> SupportedGeometryIds { get; set; } = new List<string>();
        public bool? SupportsFloorLoads { get; set; } = true;
        public bool? IsShearWall { get; set; }
        public string ShearWallType { get; set; }
        public double SelfWeight { get; set; } = 8.0 / 1000.0 / 144.0; // Default is equivalent to 8 psf of wall area
        public double CladdingWeight { get; set; } = 0.0;
        public string ParentWallId { get; set; }
        public bool HasOpening { get; set; } = false;
        public Tuple<double?, double?> RoughOpeningBottomAndTop { get; set; }
//        public ExposureLocation ExposureLocation {get;set;}

        public BearingWall()
        {
        }

        public BearingWall(Point3D endI, Point3D endJ)
        {
            EndI = endI;
            EndJ = endJ;
        }

        public BuildingLevel Level => TopLevel;
    }
}