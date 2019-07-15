using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Kataclysm.Common;
using Kataclysm.Common.Extensions;
using Kataclysm.Common.Units.Conversion;
using Katerra.Apollo.Structures.Common.Units;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class RigidAnalysis : IReportsLevel
    {
        public BuildingLevelLateral2 LateralLevel { get; }
        public List<AnalyticalWallLateral> Walls { get; }
        public List<LateralLevelForce> Forces { get; }
        public List<LoadCase> LoadCases { get; }
        
        public LevelStiffnessParameters LevelStiffness { get; private set; }
        public Dictionary<string, ShearWallPanelRigidAnalysisParameters> WallStiffnesses { get; private set; }
        
        public RigidResponses Responses { get; } = new RigidResponses();

        [JsonIgnore] public BuildingLevel Level => LateralLevel.Level;
        
        private int _nodeCount;
        private double Cd;
        
        public RigidAnalysis(BuildingLevelLateral2 lateralLevel, IEnumerable<AnalyticalWallLateral> walls,
            IEnumerable<LateralLevelForce> forces, IEnumerable<LoadCase> loadCases, double Cd)
        {
            LateralLevel = lateralLevel;
            Walls = walls.ToList();
            Forces = forces.ToList();
            LoadCases = loadCases.ToList();
            this.Cd = Cd;
        }

        public void Analyze()
        {
            WallStiffnesses = DetermineAllWallStiffnessParametersSansOffset();

            LevelStiffness = DetermineLevelStiffnessParametersSansTorsional();
            
            LateralLevel.CenterOfRigidity = DetermineCenterOfRigidity();

            UpdateAllWallStiffnessSignedOffsets();
            
            LevelStiffness.J = DetermineLevelTorsionalStiffness();
            
            Responses.WallLoadPatternResults = DetermineAllWallLoadPatternResponses();

            Responses.WallLoadCaseResults = DetermineAllWallLoadCaseResponses();

            Responses.TorsionalIrregularityTrackingNodes = DefineDriftTrackingLocations();

            Responses.NodalLoadPatternResults = DetermineAllNodeLoadPatternResponses();
            
            Responses.NodalLoadCaseResults = DetermineAllNodeLoadCaseResponses();
        }

        public NodalDisplacement DetermineRigidbodyPointDisplacement(Point2D coordinate, LoadPattern loadPattern)
        {
            LateralLevelForce forceAtLoadPattern = Forces.First(f => f.LoadPattern == loadPattern);
            
            var alpha = new Unitless((coordinate - LateralLevel.CenterOfRigidity).SignedAngleTo(Vector2D.XAxis));
            var d = new Length(coordinate.DistanceTo(LateralLevel.CenterOfRigidity), LengthUnit.Inch);
            
            Moment M = forceAtLoadPattern.TotalT(LateralLevel.CenterOfRigidity);
            
            var rotation = (Unitless) (M / LevelStiffness.J);

            Length displacementX = (Length) (forceAtLoadPattern.DirectX / LevelStiffness.X + d * rotation * Result.Sin(alpha));
            Length displacementY = (Length) (forceAtLoadPattern.DirectY / LevelStiffness.Y + d * rotation * Result.Cos(alpha));

            if (LoadPatternTypeConverter.Convert(loadPattern) == LoadPatternType.Earthquake)
            {
                displacementX = (Length) (displacementX * Cd);
                displacementY = (Length) (displacementY * Cd);
            }
            
            return new NodalDisplacement
            {
                LoadPattern = loadPattern,
                Ux = displacementX.ConvertTo(LengthUnit.Inch),
                Uy = displacementY.ConvertTo(LengthUnit.Inch),
                Rz = 0
            };
        }

        private void UpdateAllWallStiffnessSignedOffsets()
        {
            foreach (AnalyticalWallLateral wall in Walls)
            {
                WallStiffnesses[wall.UniqueId].SignedOffsetFromCenterOfRigidity = DetermineSignedOffset(wall);
            }
        }

        private Dictionary<string, ShearWallPanelResponseCollection> DetermineAllWallLoadCaseResponses()
        {
            var analysisResults = new Dictionary<string, ShearWallPanelResponseCollection>();

            foreach (AnalyticalWallLateral wall in Walls)
            {
                ShearWallPanelResponseCollection responses = DetermineWallLoadCaseResponses(wall);

                analysisResults.Add(responses.ElementId, responses);
            }

            return analysisResults;
        }

        private Dictionary<string, ShearWallPanelResponseCollection> DetermineAllWallLoadPatternResponses()
        {
            var analysisResults = new Dictionary<string, ShearWallPanelResponseCollection>();

            foreach (AnalyticalWallLateral wall in Walls)
            {
                ShearWallPanelResponseCollection responses = DetermineWallLoadPatternResponses(wall);

                analysisResults.Add(responses.ElementId, responses);
            }

            return analysisResults;
        }

        private Dictionary<string, NodalResponseCollection> DetermineAllNodeLoadCaseResponses()
        {
            var analysisResults = new Dictionary<string, NodalResponseCollection>();

            foreach (KeyValuePair<string,Point2D> node in Responses.TorsionalIrregularityTrackingNodes)
            {
                NodalResponseCollection responses = DetermineNodeLoadCaseResponses(node.Key);
                
                analysisResults.Add(responses.ElementId, responses);
            }

            return analysisResults;
        }

        private Dictionary<string, NodalResponseCollection> DetermineAllNodeLoadPatternResponses()
        {
            var analysisResults = new Dictionary<string, NodalResponseCollection>();

            foreach (KeyValuePair<string, Point2D> node in Responses.TorsionalIrregularityTrackingNodes)
            {
                var responses = DetermineNodeLoadPatternResponses(node.Value, node.Key);
                
                analysisResults.Add(responses.ElementId, responses);
            }
            
            return analysisResults;
        }

        private Dictionary<string, Point2D> DefineDriftTrackingLocations()
        {
            var boundaryNodes = new Dictionary<string, Point2D>();

            Tuple<Point2D, Point2D> nodesX =
                LateralLevel.Boundary.GetMinMaxPointsAtGreatestBoundaryExtent(LateralDirection.X);
            Tuple<Point2D, Point2D> nodesY =
                LateralLevel.Boundary.GetMinMaxPointsAtGreatestBoundaryExtent(LateralDirection.Y);
            
            var allNodes = new List<Point2D>
            {
                nodesX.Item1,
                nodesX.Item2,
                nodesY.Item1,
                nodesY.Item2
            };

            List<Point2D> uniqueNodes = allNodes.DistinctBasedOnDistanceApart(1e-3);

            uniqueNodes.ForEach(n => boundaryNodes.Add(NodalResponse.FormatNodeIdAlt(_nodeCount++), n));

            return boundaryNodes;
        }

        private Moment DetermineLevelTorsionalStiffness()
        {
            Moment SumOfJ = new Moment(0, MomentUnit.KipInch);

            foreach (ShearWallPanelRigidAnalysisParameters wallStiffness in WallStiffnesses.Values)
            {
                SumOfJ = (Moment) (SumOfJ + wallStiffness.K * (wallStiffness.SignedOffsetFromCenterOfRigidity ^ 2));
            }

            return SumOfJ;
        }

        private Length DetermineSignedOffset(AnalyticalWallLateral wall)
        {
            var wallSegment = wall.WallLine;

            Point2D closestPointAlongExtensionToCenterOfRigidity =
                wallSegment.ClosestPointToExtension(LateralLevel.CenterOfRigidity);
            
            double distanceFromCenterOfRigidity =
                closestPointAlongExtensionToCenterOfRigidity.DistanceTo(LateralLevel.CenterOfRigidity);

            double signedDistanceFromCenterOfRigidity;

            switch (wallSegment.WhichSide(LateralLevel.CenterOfRigidity))
            {
                case Side.Right:
                    signedDistanceFromCenterOfRigidity = -distanceFromCenterOfRigidity;
                    break;
                case Side.Left:
                    signedDistanceFromCenterOfRigidity = distanceFromCenterOfRigidity;
                    break;
                case Side.Colinear:
                    signedDistanceFromCenterOfRigidity = 0;
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }

            return new Length(signedDistanceFromCenterOfRigidity, LengthUnit.Inch);
        }

        private ShearWallPanelResponseCollection DetermineWallLoadCaseResponses(AnalyticalWallLateral wall)
        {
            ShearWallPanelResponseCollection loadPatternResults = Responses.WallLoadPatternResults[wall.UniqueId];

            IEnumerable<ShearWallPanelResponse> loadCaseResults =
                loadPatternResults.GetSuperimposedResponsesAtLoadCases(LoadCases);
            
            return new ShearWallPanelResponseCollection(loadCaseResults);
        }

        private ShearWallPanelResponseCollection DetermineWallLoadPatternResponses(AnalyticalWallLateral wall)
        {
            ShearWallPanelRigidAnalysisParameters wallStiffness = WallStiffnesses[wall.UniqueId];

            var responses = new ShearWallPanelResponseCollection(wall.UniqueId, wall.WallLine);
            
            foreach (LateralLevelForce force in Forces)
            {
                responses.Add(DetermineWallResponse(wall, wallStiffness, force));
            }

            return responses;
        }

        private NodalResponseCollection DetermineNodeLoadCaseResponses(string nodeId)
        {
            NodalResponseCollection loadPatternResults = Responses.NodalLoadPatternResults[nodeId];

            IEnumerable<NodalResponse> loadCaseResults =
                loadPatternResults.GetSuperimposedResponsesAtLoadCases(LoadCases);
            
            return new NodalResponseCollection(loadCaseResults);
        }

        private NodalResponseCollection DetermineNodeLoadPatternResponses(Point2D vertex, string nodeId)
        {
            var responses = new NodalResponseCollection(nodeId, vertex);
            
            foreach (LateralLevelForce force in Forces)
            {
                NodalDisplacement displacement = DetermineRigidbodyPointDisplacement(vertex, force.LoadPattern);

                responses.Add(new NodalResponse(nodeId, force.LoadPattern, vertex)
                {
                    Displacement = displacement,
                    Stress = new NodalStress {LoadPattern = force.LoadPattern},
                    Reaction = new NodalForce {LoadPattern = force.LoadPattern},
                    ExternalForce = new NodalForce {LoadPattern = force.LoadPattern}
                });
            }

            return responses;
        }

        private ShearWallPanelResponse DetermineWallResponse(AnalyticalWallLateral wall,
            ShearWallPanelRigidAnalysisParameters wallStiffness, LateralLevelForce force)
        {
            Force directXShear = (Force) (wallStiffness.Kx * force.DirectX / LevelStiffness.X);
            Force directYShear = (Force) (wallStiffness.Ky * force.DirectY / LevelStiffness.Y);
            
            Vector2D directVector = new Vector2D(directXShear.Value, directYShear.Value);
            
            var wallUnitVector = wall.WallLine.ToVector2D().Normalize();
            
            var response = new ShearWallPanelResponse(wall.UniqueId, force.LoadPattern, wall.WallLine);

            response.DirectShear = directVector.AngleTo(wallUnitVector).Degrees < 90
                ? new Force(directVector.Length, ForceUnit.Kip)
                : new Force(-directVector.Length, ForceUnit.Kip);

            response.TorsionalShear = (Force) (wallStiffness.K * wallStiffness.SignedOffsetFromCenterOfRigidity *
                                               force.TotalT(LateralLevel.CenterOfRigidity) / LevelStiffness.J);

            return response;
        }

        private LevelStiffnessParameters DetermineLevelStiffnessParametersSansTorsional()
        {
            ForcePerLength SumOfX = new ForcePerLength(0, ForcePerLengthUnit.KipPerInch);
            ForcePerLength SumOfY = new ForcePerLength(0, ForcePerLengthUnit.KipPerInch);

            foreach (ShearWallPanelRigidAnalysisParameters wallStiffness in WallStiffnesses.Values)
            {
                SumOfX = (ForcePerLength) (SumOfX + wallStiffness.Kx);
                SumOfY = (ForcePerLength) (SumOfY + wallStiffness.Ky);
            }
            
            return new LevelStiffnessParameters
            {
                X = SumOfX,
                Y = SumOfY
            };
        }

        private Dictionary<string, ShearWallPanelRigidAnalysisParameters> DetermineAllWallStiffnessParametersSansOffset()
        {
            var stiffnesses = new Dictionary<string, ShearWallPanelRigidAnalysisParameters>();

            foreach (AnalyticalWallLateral wall in Walls)
            {
                ShearWallPanelRigidAnalysisParameters stiffnessParameters = DetermineWallStiffnessParametersSansOffset(wall);
                
                stiffnesses.Add(wall.UniqueId, stiffnessParameters);
            }

            return stiffnesses;
        }

        private ShearWallPanelRigidAnalysisParameters DetermineWallStiffnessParametersSansOffset(AnalyticalWallLateral wall)
        {
            var wallVector = wall.WallLine.ToVector2D();
            var wallAngleToXAxis = new Unitless(wallVector.SignedAngleTo(Vector2D.XAxis).Radians, UnitlessUnit.Radian);
            
            var k = new ForcePerLength(wall.Stiffness, ForcePerLengthUnit.KipPerInch);
            
            return new ShearWallPanelRigidAnalysisParameters
            {
                K = k,
                AngleFromWallAxisToGlobalXAxis = wallAngleToXAxis
            };
        }

        private Point2D DetermineCenterOfRigidity()
        {
            Force SumOfXTimesYCenters = new Force(0, ForceUnit.Kip);
            Force SumOfYTimesXCenters = new Force(0, ForceUnit.Kip);

            foreach (AnalyticalWallLateral wall in Walls)
            {
                ShearWallPanelRigidAnalysisParameters wallStiffness = WallStiffnesses[wall.UniqueId];

                SumOfXTimesYCenters = (Force) (SumOfXTimesYCenters + wallStiffness.Kx * new Length(wall.Centroid.Y, LengthUnit.Inch));
                SumOfYTimesXCenters = (Force) (SumOfYTimesXCenters + wallStiffness.Ky * new Length(wall.Centroid.X, LengthUnit.Inch));
            }

            Length xCenter = (Length) (SumOfYTimesXCenters / LevelStiffness.Y);
            Length yCenter = (Length) (SumOfXTimesYCenters / LevelStiffness.X);
            
            return new Point2D(xCenter.ConvertTo(LengthUnit.Inch), yCenter.ConvertTo(LengthUnit.Inch));
        }
        
        public static LevelDictionary<PointDrift> GetDesignBoundaryCornerDrifts(LevelDataDictionary<RigidAnalysis> analyses)
        {
            var drifts = new LevelDictionary<PointDrift>();

            List<BuildingLevelLateral2> sortedLevels =
                analyses.Select(l => l.Value.LateralLevel).OrderBy(l => l.Level.Elevation).ToList();

            for (var i = 0; i < sortedLevels.Count; i++)
            {
                BuildingLevel level = sortedLevels[i].Level;

                RigidAnalysis analysis = analyses[level];

                List<PointDrift> driftsAtLevel =
                    DisplacementsWithUnknownDrifts(level, analysis.Responses.NodalLoadCaseResults.Values.SelectMany(r => r));

                var storyHeight = sortedLevels[i].Height;

                if (i == 0)
                {
                    // Lowest level; compare to ground
                    driftsAtLevel.ForEach(d =>
                    {
                        d.Drift = (Unitless) Result.Abs(d.Displacement / storyHeight);
                        d.CoordinateBelow = d.Coordinate;
                        d.DisplacementBelow = new Length(0, LengthUnit.Inch);
                    });
                }
                else
                {
                    // Elevated level; compare to level below 
                    List<PointDrift> driftsAtLevelBelow = drifts[sortedLevels[i - 1].Level];
                    
                    driftsAtLevel.ForEach(d =>
                    {
                        PointDrift driftBelow = d.GetMatchingDriftAtLevelBelow(driftsAtLevelBelow);

                        d.Drift = (Unitless) Result.Abs((d.Displacement - driftBelow.Displacement) / storyHeight);
                        d.CoordinateBelow = driftBelow.Coordinate;
                        d.DisplacementBelow = driftBelow.Displacement;
                    });
                }
                
                drifts.Add(level, driftsAtLevel);
            }

            return drifts;
        }

        private static List<PointDrift> DisplacementsWithUnknownDrifts(BuildingLevel level,
            IEnumerable<NodalResponse> nodalResponses)
        {
            var drifts = new List<PointDrift>();

            foreach (NodalResponse response in nodalResponses)
            {
                drifts.Add(new PointDrift
                {
                    Name = response.ElementId,
                    Coordinate = response.Coordinate,
                    Level = level,
                    LoadCategory = response.LoadCase.LoadPatternType,
                    Displacement = new Length(response.Displacement.Ux, LengthUnit.Inch),
                    Direction = LateralDirection.X,
                    LoadCase = response.LoadCase
                });
                
                drifts.Add(new PointDrift
                {
                    Name = response.ElementId,
                    Coordinate = response.Coordinate,
                    Level = level,
                    LoadCategory = response.LoadCase.LoadPatternType,
                    Displacement = new Length(response.Displacement.Uy, LengthUnit.Inch),
                    Direction = LateralDirection.Y,
                    LoadCase = response.LoadCase
                });
            }

            return drifts;
        }
    }
}