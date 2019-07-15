using System;
using System.IO;
using Kataclysm.Common;
using Kataclysm.Common.Extensions;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class AnalyticalWallLateral
    {
        [JsonProperty]
        private LineSegment2D _wallLine;

        [JsonProperty]
        public GeneralLightFramedShearWall DesignWall { get; set; }

        [JsonIgnore]
        public AnalyticalWall AnalyticalWall { get; set; }

        [JsonProperty]
        private string _uniqueId;
        public string UniqueId
        {
            get
            {
                return _uniqueId;
            }
            set
            {
                _uniqueId = value;
            }
        }

        [JsonProperty]
        private string _groupId;
        public string GroupId
        {
            get
            {
                return _groupId;
            }
            set
            {
                _groupId = value;
            }
        }

        public LineSegment2D WallLine
        {
            get { return _wallLine; }
            set
            {
                _wallLine = value;

                UnitVector = _wallLine.CalculateUnitVector();
                Angle = _wallLine.CalculateAngle();
                Centroid = _wallLine.Midpoint();
            }
        }

        public Vector2D UnitVector { get; set; }
        public double Angle { get; set; }
        public Point2D Centroid { get; set; }

        public SeismicAnalysisResults SeismicAnalysisResults { get; set; }
        public WindAnalysisResults WindAnalysisResults { get; set; }

        public double Stiffness { get; set; }

        public AnalyticalWallLateral(GeneralWoodShearWall shearWall, AnalyticalWall analyticalWall)
        {
            DesignWall = shearWall;
            AnalyticalWall = analyticalWall;
            WallLine = new LineSegment2D(new Point2D(analyticalWall.EndI.X, analyticalWall.EndI.Y), new Point2D(analyticalWall.EndJ.X, analyticalWall.EndJ.Y));
            UniqueId = analyticalWall.UniqueId;
            //TopLevel = analyticalWall.TopLevel;

            SeismicAnalysisResults = new SeismicAnalysisResults();
            WindAnalysisResults = new WindAnalysisResults();
        }

        public double GetMaxCumulativeShearForce(LoadPatternType patternType)
        {
            double maxWallForce = 0;
            double minWallForce = 0;
            var length = WallLine.Length;

            switch (patternType)
            {
                case LoadPatternType.Earthquake:
                    maxWallForce = SeismicAnalysisResults.CumulativeShear.GetForceListSeismic().Max();
                    minWallForce = SeismicAnalysisResults.CumulativeShear.GetForceListSeismic().Min();
                    SeismicAnalysisResults.MaxCumulativeShearForce = Math.Max(Math.Abs(maxWallForce), Math.Abs(minWallForce)) / length;
                    return SeismicAnalysisResults.MaxCumulativeShearForce;

                case LoadPatternType.Wind:
                    maxWallForce = WindAnalysisResults.CumulativeShear.GetForceListWind().Max();
                    minWallForce = WindAnalysisResults.CumulativeShear.GetForceListWind().Min();
                    WindAnalysisResults.MaxCumulativeShearForce = Math.Max(Math.Abs(maxWallForce), Math.Abs(minWallForce)) / length;
                    return WindAnalysisResults.MaxCumulativeShearForce;

                default:
                    throw new InvalidDataException("expected wind or seismic");
            }
        }
        public double GetMaxCumulativeOverturning(LoadPatternType loadPattern)
        {
            double maxWallMoment;
            double minWallMoment;

            switch (loadPattern)
            {
                case LoadPatternType.Earthquake:
                    maxWallMoment = SeismicAnalysisResults.CumulativeMoment.GetForceListSeismic().Max();
                    minWallMoment = SeismicAnalysisResults.CumulativeMoment.GetForceListSeismic().Min();
                    SeismicAnalysisResults.MaxCumulativeOverturning = Math.Max(Math.Abs(maxWallMoment), Math.Abs(minWallMoment));
                    return SeismicAnalysisResults.MaxCumulativeOverturning;

                case LoadPatternType.Wind:
                    maxWallMoment = WindAnalysisResults.CumulativeMoment.GetForceListWind().Max();
                    minWallMoment = WindAnalysisResults.CumulativeMoment.GetForceListWind().Min();
                    WindAnalysisResults.MaxCumulativeOverturning = Math.Max(Math.Abs(maxWallMoment), Math.Abs(minWallMoment));
                    return WindAnalysisResults.MaxCumulativeOverturning;

                default:
                    throw new InvalidDataException("expected wind or seismic");
            }


        }
        public double GetMaxShearForce(LoadPatternType patternType)
        {
            double maxWallForce;
            double minWallForce;
            
            var length = WallLine.Length;

            switch (patternType)
            {
                case LoadPatternType.Earthquake:
                    maxWallForce = SeismicAnalysisResults.ShearForce.GetForceListSeismic().Max();
                    minWallForce = SeismicAnalysisResults.ShearForce.GetForceListSeismic().Min();
                    SeismicAnalysisResults.MaxShearForce = Math.Max(Math.Abs(maxWallForce), Math.Abs(minWallForce)) / length;
                    return SeismicAnalysisResults.MaxShearForce;

                case LoadPatternType.Wind:
                    maxWallForce = WindAnalysisResults.ShearForce.GetForceListWind().Max();
                    minWallForce = WindAnalysisResults.ShearForce.GetForceListWind().Min();
                    WindAnalysisResults.MaxShearForce = Math.Max(Math.Abs(maxWallForce), Math.Abs(minWallForce)) / length;
                    return WindAnalysisResults.MaxShearForce;

                default:
                    throw new InvalidDataException("expected wind or seismic");
            }
        }
        public double GetMaxOverturning(LoadPatternType patternType)
        {
            double maxWallMoment;
            double minWallMoment;

            switch (patternType)
            {
                case LoadPatternType.Earthquake:
                    maxWallMoment = SeismicAnalysisResults.OverturningMoment.GetForceListSeismic().Max();
                    minWallMoment = SeismicAnalysisResults.OverturningMoment.GetForceListSeismic().Min();
                    SeismicAnalysisResults.MaxOverturning = Math.Max(Math.Abs(maxWallMoment), Math.Abs(minWallMoment));
                    return SeismicAnalysisResults.MaxOverturning;

                case LoadPatternType.Wind:
                    maxWallMoment = WindAnalysisResults.OverturningMoment.GetForceListWind().Max();
                    minWallMoment = WindAnalysisResults.OverturningMoment.GetForceListWind().Min();
                    WindAnalysisResults.MaxOverturning = Math.Max(Math.Abs(maxWallMoment), Math.Abs(minWallMoment));
                    return WindAnalysisResults.MaxOverturning;

                default:
                    throw new InvalidDataException("expected wind or seismic");
            }
        }

        public Vector2D StiffnessVector()
        {
            double x = Stiffness * Math.Pow(Math.Cos(Angle * Math.PI / 180), 2);
            double y = Stiffness * Math.Pow(Math.Sin(Angle * Math.PI / 180), 2);

            Vector2D wallStiffVector = new Vector2D(x, y);

            return wallStiffVector;
        }

        public Vector2D StiffnessVectorCentroid()
        {
            double x = StiffnessVector().X * Centroid.Y;
            double y = StiffnessVector().Y * Centroid.X;


            Vector2D wallStiffVectorCentroid = new Vector2D(x, y);

            return wallStiffVectorCentroid;
        }

    }
}