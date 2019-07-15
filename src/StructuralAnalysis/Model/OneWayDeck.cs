using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Kataclysm.Common;
using Kataclysm.Common.Extensions;
using Kataclysm.Common.Geometry;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.StructuralAnalysis.Model
{
    public class OneWayDeck : IReportsLevel
    {
        public string UniqueId { get; set; }
        public Polygon3D Polygon { get; set; }
        public Vector3D? SpanDirection { get; set; }
        public BuildingLevel Level { get; set; }
        public BuildingMaterial? MaterialCategory { get; set; }
        public IElasticMaterial MaterialProperties { get; set; }
        public string MaterialName { get; set; } = "";
        public string Type { get; set; }
        public double? TotalThickness { get; set; }
        public double? DiaphragmThickness { get; set; }
        public double? SelfWeight { get; set; }
        public List<Polygon3D> Openings { get; set; } = new List<Polygon3D>();
        public DiaphragmDesignProperties DiaphragmDesignProperties { get; set; } = new DiaphragmDesignProperties();


        #region Private Variables
        [JsonProperty] private List<AreaLoad> _associatedAreaLoads = new List<AreaLoad>();
        [JsonProperty] private List<LineLoad> _associatedLineLoads = new List<LineLoad>();
        [JsonProperty] private List<PointLoad> _associatedPointLoads = new List<PointLoad>();
        #endregion

        #region Public Getters
        public List<AreaLoad> AssociatedAreaLoads
        {
            get
            {
                return _associatedAreaLoads;
            }
        }

        public List<LineLoad> AssociatedLineLoads
        {
            get
            {
                return _associatedLineLoads;
            }
        }

        [JsonIgnore]
        public List<PointLoad> AssociatedPointLoads
        {
            get
            {
                return _associatedPointLoads;
            }
        }
        #endregion

        public void ReversePolygon()
        {
            Polygon = Polygon.Reverse();
        }

        public override string ToString()
        {
            return $"one-way deck {UniqueId}";
        }

        public OperationResult Validate()
        {
            var oR = new OperationResult();

            oR.AddMessagesFrom(ValidateNonBlankString(UniqueId, "unique id"));

            oR.AddMessagesFrom(ValidateNullableValueNotNull(Polygon, "polygon"));

            oR.AddMessagesFrom(ValidateSpanDirection(SpanDirection));

            oR.AddMessagesFrom(Level.Validate());

            oR.AddMessagesFrom(ValidateNullableValueNotNull(MaterialCategory, "material category"));

            oR.AddMessagesFrom(ValidateNullableValueNotNull(MaterialName, "material name"));

            oR.AddMessagesFrom(ValidateNonBlankString(Type, "type"));

            oR.AddMessagesFrom(ValidateNullableValueGreaterThanZero(TotalThickness, "total thickness"));
            
            oR.AddMessagesFrom(ValidateNullableValueGreaterThanZero(DiaphragmThickness, "diaphragm thickness"));

            oR.AddMessagesFrom(ValidateNullableValueGreaterThanZero(SelfWeight, "self weight"));

            oR.AddMessagesFrom(ValidateCollectionNotNull(Openings, "openings"));
            
            oR.AddMessagesFrom(ValidateOpenings(Openings, Polygon));

            if (oR.Messages.Any())
            {
                oR.PrependMessage($"Invalid one-way deck found.");
            }

            return oR.FailsWithMessages();
        }



        public void AssociateAreaLoadsWithDeck(List<AreaLoad> loads)
        {
            _associatedAreaLoads = new List<AreaLoad>();

            _associatedAreaLoads.Clear();

            Polygon2D projectedBoundary = Polygon.ProjectToHorizontalPlane();

            if (loads != null)
            {
                foreach (AreaLoad load in loads)
                {
                    if (load.GetType().Equals(typeof(UniformAreaLoad)) == true)
                    {
                        List<UniformAreaLoad> boundedLoads = GetBoundedPartOfGlobalLoad((UniformAreaLoad)load).ToList();

                        if (boundedLoads != null)
                        {
                            _associatedAreaLoads.AddRange(boundedLoads);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException("Sloping area loads are not implemented yet.");
                    }
                }
            }

            var selfWeight = new UniformAreaLoad(projectedBoundary, Level, Projection.AlongSlope, LoadPattern.Dead,
                -1 * SelfWeight.Value);
            var unitLive = new UniformAreaLoad(projectedBoundary, Level, Projection.Vertical, LoadPattern.UnitLiveLoad, -1.0);

            if (selfWeight.Magnitude < 0)
            {
                _associatedAreaLoads.Add(selfWeight);
            }

            _associatedAreaLoads.Add(unitLive);
        }

        public void AssociatePointLoadsWithDeck(List<PointLoad> loads)
        {
            _associatedPointLoads = new List<PointLoad>();

            _associatedPointLoads.Clear();

            if (loads != null)
            {
                foreach (PointLoad load in loads)
                {
                    PointLoad boundedLoads = GetBoundedPartOfGlobalLoad(load);

                    if (boundedLoads != null)
                    {
                        _associatedPointLoads.Add(boundedLoads);
                    }
                }
            }
            
        }

        public void AssociateLineLoadsWithDeck(List<LineLoad> loads)
        {
            _associatedLineLoads = new List<LineLoad>();

            _associatedLineLoads.Clear();

            Polygon2D projectedBoundary = Polygon.ProjectToHorizontalPlane();

            if (loads != null)
            {
                var segmentedLoads = SubdivideLineLoadsAtDeckBoundary(loads);

                foreach (var load in segmentedLoads)
                {
                    if (projectedBoundary.EnclosesPointIncludingEdges(load.Line.Midpoint()))
                    {
                        _associatedLineLoads.Add(load);
                    }
                }
            }
            
        }

        public Plane GetPlane()
        {
            var vertices = Polygon.Vertices();

            return Plane.FromPoints(vertices[0], vertices[1], vertices[2]);
        }

        private IEnumerable<UniformAreaLoad> GetBoundedPartOfGlobalLoad(UniformAreaLoad load)
        {
            var overlappingBoundaries = load.Boundary.BooleanIntersection(Polygon.ProjectToHorizontalPlane());

            Polygon2D pg = Polygon.ProjectToHorizontalPlane();

            if (overlappingBoundaries != null)
            {
                var loads = new List<UniformAreaLoad>();

                foreach (var boundary in overlappingBoundaries)
                {
                    var subLoad = new UniformAreaLoad()
                    {
                        Boundary = boundary,
                        Level = load.Level,
                        Magnitude = load.Magnitude,
                        Projection = load.Projection,
                        LoadPattern = load.LoadPattern
                    };

                    loads.Add(subLoad);
                }

                return loads;
            }

            return null;
        }

        private PointLoad GetBoundedPartOfGlobalLoad(PointLoad load)
        {
            return (Polygon.ProjectToHorizontalPlane().EnclosesPointIncludingEdges(load.Location.Value))
                ? load
                : null;
        }

        private IEnumerable<UniformAreaLoad> GetBoundedPartOfGlobalLoad(SlopingAreaLoad load)
        {
            throw new NotImplementedException("Sloping area load subdivision is not yet implemented.");
        }

        private List<LineLoad> SubdivideLineLoadsAtDeckBoundary(List<LineLoad> loads)
        {
            var newLoads = new List<LineLoad>();

            foreach (var load in loads)
            {
                switch (load)
                {
                    case UniformLineLoad uniformLoad:
                        newLoads.AddRange(SubdivideLineLoadAtDeckBoundary(uniformLoad));
                        break;
                    case SlopingLineLoad slopingLoad:
                        throw new NotImplementedException();
                    default:
                        throw new InvalidEnumArgumentException();
                }
            }

            return newLoads;
        }

        private List<UniformLineLoad> SubdivideLineLoadAtDeckBoundary(UniformLineLoad load)
        {
            List<Point2D> unorderedVertices = DetermineIntersectionsWithBoundary(load);

            List<LineSegment2D> subdivisions = load.Line.SubdivideAtCollinearPoints(unorderedVertices);

            return CreateLineLoads(load, subdivisions);
        }

        private List<Point2D> DetermineIntersectionsWithBoundary(UniformLineLoad load)
        {
            Polygon2D projectedBoundary = Polygon.ProjectToHorizontalPlane();

            var allIntersections = new List<Point2D>();

            var intersections = load.Line.CalculateIntersections(projectedBoundary).ToList();

            if (intersections.Count > 0) allIntersections.AddRange(intersections);

            return allIntersections;
        }

        private List<UniformLineLoad> CreateLineLoads(UniformLineLoad source, IEnumerable<LineSegment2D> lines)
        {
            var newLoads = new List<UniformLineLoad>();

            foreach (var line in lines)
            {
                newLoads.Add(new UniformLineLoad(line, source.Magnitude.Value,source.Level, source.Projection, source.LoadPattern));
            }

            return newLoads;
        }

        public Rect Bounds()
        {
            List<Point2D> points = Polygon.ProjectToHorizontalPlane().Vertices.ToList();

            double maxX = 0;
            double minX = 0;
            double maxY = 0;
            double minY = 0;

            for (int i = 0; i < points.Count; i++)
            {
                Point2D p = points[i];
                if (i == 0)
                {
                    maxX = p.X;
                    minX = p.X;
                    maxY = p.Y;
                    minY = p.Y;
                }
                else
                {
                    if (p.X < minX)
                    {
                        minX = p.X;
                    }
                    if (p.X > maxX)
                    {
                        maxX = p.X;
                    }
                    if (p.Y < minY)
                    {
                        minY = p.Y;
                    }
                    if (p.Y > maxY)
                    {
                        maxY = p.Y;
                    }
                }
            }

            double p0x = Convert.ToInt32(Math.Floor(minX));
            double p0y = Convert.ToInt32(Math.Floor(minY));

            Point2D p0 = new Point2D(p0x, p0y);

            double width = Convert.ToInt32(Math.Ceiling(maxX - p0x));

            double height = Convert.ToInt32(Math.Ceiling(maxY - p0y));

            return new Rect(p0, width, height);
        }
    }
}