using Kataclysm.Common.Units.Conversion;
using Katerra.Apollo.Structures.Common.Units;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.Common
{
    public class LateralLevelForce : IReportsLoadPattern, IReportsLevel
    {
        public BuildingLevel Level { get; set; }
        public LoadPattern LoadPattern { get; set; }
        public Point2D CenterOfForce { get; set; }
        
        public Force DirectX { get; set; } = new Force(0, ForceUnit.Kip);
        public Force DirectY { get; set; } = new Force(0, ForceUnit.Kip);
        public Moment AccidentalT { get; set; } = new Moment(0, MomentUnit.KipInch);

        public Moment TotalT(Point2D centerOfRigidity)
        {
            Vector2D vectorBetween = CenterOfForce - centerOfRigidity;

            return (Moment) (AccidentalT - new Length(vectorBetween.Y, LengthUnit.Inch) * DirectX +
                             new Length(vectorBetween.X, LengthUnit.Inch) * DirectY);
        }
    }
}