using System.Collections.Generic;
using Kataclysm.Common;
using Kataclysm.StructuralAnalysis.Model;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class AnalyticalWall : AnalyticalElement
    {
//        public CalculationLog Log { get; set; }
        
        [JsonProperty]
        public BearingWall BearingWall;
        
        private AnalyticalWall(BearingWall wall)
            : base(wall.UniqueId, wall.SupportedGeometryIds, wall.SupportsFloorLoads.Value)
        {
            BearingWall = wall;
            EndI = BearingWall.EndI.Value;
            EndJ = BearingWall.EndJ.Value;

            Length = EndI.DistanceTo(EndJ);

//            Log = new CalculationLog(ID);
        }

        public static AnalyticalWall Create(BearingWall wall)
        {
            return new AnalyticalWall(wall);
        }
        
        public override BuildingLevel Level => BearingWall.TopLevel;

        public BuildingLevel TopLevel => BearingWall.TopLevel;

        public BuildingLevel BotLevel => BearingWall.BottomLevel;

        protected override void ApplySelfWeight()
        {
            double w = GetSelfWeightLineLoadMagnitude();

            InternalDistributedLoad3D selfWeight = new InternalDistributedLoad3D(new PointLoad3D(EndI, 0, 0, w, 0, 0, 0, LoadPattern.Dead), new PointLoad3D(EndJ, 0, 0, w, 0, 0, 0, LoadPattern.Dead), this);

            AddInternalDistributedLoad(selfWeight);
        }

        protected override double GetSelfWeightLineLoadMagnitude()
        {
            double height = TopLevel.Elevation - BotLevel.Elevation;

            double floorDepth = TopLevel.Elevation - TopOfWallElevation;

            double w = (height - floorDepth) * BearingWall.SelfWeight;

            return -w;
        }

        public override List<FloorLoad> GetSelfWeightLoadsForSeismicMass()
        {
            var loads = new List<FloorLoad>();

            double w = GetSelfWeightLineLoadMagnitude();

            LineSegment2D seg = new LineSegment2D(new Point2D(EndI.X, EndI.Y), new Point2D(EndJ.X, EndJ.Y));

            var selfWeightTop = new UniformLineLoad(seg, w * 0.5, TopLevel, Projection.Vertical, LoadPattern.Dead);

            var selfWeightBot = new UniformLineLoad(seg, w * 0.5, BotLevel, Projection.Vertical, LoadPattern.Dead);

            loads.Add(selfWeightTop);
            loads.Add(selfWeightBot);

            return loads;
        }

        public override void ReduceLiveLoads(IEnumerable<AnalyticalElement> supportedElements)
        {
            ReduceLiveLoads(supportedElements, Log);
        }

        public string UniqueId => BearingWall.UniqueId;

        public string PierID => BearingWall.PierID;

        public double Thickness => BearingWall.Thickness.Value;

        public double TopOfWallElevation => BearingWall.EndI.Value.Z;
    }
}