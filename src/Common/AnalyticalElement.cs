using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Kataclysm.Common.Extensions;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.Common
{
    public abstract class AnalyticalElement : IReportsLevel
    {
        public readonly string ID;
        public Point3D EndI { get; protected set; }
        public Point3D EndJ { get; protected set; }
        public bool SupportsFloorLoads { get; protected set; }
        public double Length { get; protected set; }
        public double ReducibleUniformLiveLoadCoefficient { get; protected set; } = 1.0;

        protected const double FUZZDISTANCE = 2; //Inches

        [JsonProperty]
        private List<PointLoad3D> _externalPointLoads = new List<PointLoad3D>();
        [JsonProperty]
        private List<InternalPointLoad3D> _internalPointLoads = new List<InternalPointLoad3D>();
        [JsonProperty]
        private List<DistributedLoad3D> _externalDistributedLoads = new List<DistributedLoad3D>();
        [JsonProperty]
        private List<InternalDistributedLoad3D> _internalDistributedLoads = new List<InternalDistributedLoad3D>();

        [JsonProperty]
        public List<string> SupportedGeometryIds { get; protected set; }
        [JsonProperty]
        private List<Point3D> _supportCoordinates;

        protected AnalyticalElement(string id, IEnumerable<string> supportedGeometryIds, bool supportsFloorLoads = true)
            : this(id, supportedGeometryIds, new List<Point3D?>(), supportsFloorLoads)
        {
        }

        protected AnalyticalElement(string id, IEnumerable<string> supportedGeometryIds,
            IEnumerable<Point3D?> supportCoordinates, bool supportsFloorLoads = true)
        {
            ID = id;
            SupportedGeometryIds = supportedGeometryIds.ToList();
            _supportCoordinates = supportCoordinates.Cast<Point3D>().ToList();
            SupportsFloorLoads = supportsFloorLoads;
        }

        #region API Functions

        public abstract BuildingLevel Level { get; }

        public IReadOnlyList<Point3D> SupportCoordinates => new ReadOnlyCollection<Point3D>(_supportCoordinates);

        public IReadOnlyList<PointLoad3D> ExternalPointLoads =>
            new ReadOnlyCollection<PointLoad3D>(_externalPointLoads);

        public IReadOnlyList<InternalPointLoad3D> InternalPointLoads =>
            new ReadOnlyCollection<InternalPointLoad3D>(_internalPointLoads);

        public IReadOnlyList<DistributedLoad3D> ExternalDistributedLoads =>
            new ReadOnlyCollection<DistributedLoad3D>(_externalDistributedLoads);

        public IReadOnlyList<InternalDistributedLoad3D> InternalDistributedLoads =>
            new ReadOnlyCollection<InternalDistributedLoad3D>(_internalDistributedLoads);

        public void AddDistributedLoad(DistributedLoad3D load)
        {
            _externalDistributedLoads.Add(load);
        }

        public void AddInternalDistributedLoad(InternalDistributedLoad3D load)
        {
            _internalDistributedLoads.Add(load);
        }

        public void AddPointLoad(PointLoad3D load)
        {
            _externalPointLoads.Add(load);
        }

        #endregion

        protected internal List<IInternalLoad> GetLoadsFromReactionsFrom(IEnumerable<AnalyticalElement> elements)
        {
            var allLoadsFromReactions = new List<IInternalLoad>();

            foreach (var analyticalElement in elements)
            {
                List<IInternalLoad> LoadsFromReactions = analyticalElement.GetLoadsFromReactions();

                //First, loop through the point loads and determine the point load with the closest distance to the line
                double ClosestDistance = 0;
                int PointLoadCount = 0;
                foreach (IInternalLoad load in LoadsFromReactions)
                {
                    if (PointLoadCount == 0)
                    {
                        if (load is InternalPointLoad3D)
                        {
                            ClosestDistance =
                                GetDistanceFromPointToLine(EndI, EndJ, ((InternalPointLoad3D)load).Location);
                            PointLoadCount += 1;
                        }
                    }
                    else
                    {
                        if (load is InternalPointLoad3D)
                        {
                            ClosestDistance =
                                Math.Min(GetDistanceFromPointToLine(EndI, EndJ, ((InternalPointLoad3D)load).Location),
                                    ClosestDistance);
                            PointLoadCount += 1;
                        }
                    }
                }

                foreach (IInternalLoad load in LoadsFromReactions)
                {
                    if (load is InternalPointLoad3D)
                    {
                        if (GetDistanceFromPointToLine2D(EndI, EndJ, ((InternalPointLoad3D)load).Location) <=
                            FUZZDISTANCE)
                        {
                            //If the load is within the fuzz distance in the X-Y plane
                            if (GetDistanceFromPointToLine(EndI, EndJ, ((InternalPointLoad3D)load).Location) <=
                                ClosestDistance + FUZZDISTANCE)
                            {
                                //If the load in question is within the closest distance (in 3D) + fuzz distance, we can add it

                                if (PointLiesWithinElementInXY(((InternalPointLoad3D)load).Location) == true)
                                {
                                    //If the point load location lies within the endpoints of the element (in the x-y plane), then
                                    //add it to the loads list
                                    allLoadsFromReactions.Add(load);
                                }
                                
                            }
                        }
                    }
                    else
                    {
                        //Distributed load

                        InternalDistributedLoad3D DistrLoad =
                            GetDistributedLoadFromReaction((InternalDistributedLoad3D)load);
                        if (DistrLoad.StartLoad.IsZero() == false || DistrLoad.EndLoad.IsZero() == false)
                        {
                            allLoadsFromReactions.Add(DistrLoad);
                        }
                    }
                }
            }

            return allLoadsFromReactions;
        }

        protected bool PointLiesWithinElementInXY(Point3D point)
        {
            double lineMaxX = Math.Max(EndI.X, EndJ.X);
            double lineMinX = Math.Min(EndI.X, EndJ.X);

            double lineMaxY = Math.Max(EndI.Y, EndJ.Y);
            double lineMinY = Math.Min(EndI.Y, EndJ.Y);

            if (point.X >= lineMinX - FUZZDISTANCE)
            {
                if (point.X <= lineMaxX + FUZZDISTANCE)
                {
                    if (point.Y >= lineMinY - FUZZDISTANCE)
                    {
                        if (point.Y <= lineMaxY + FUZZDISTANCE)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        protected internal abstract List<IInternalLoad> GetLoadsFromReactions();

        protected internal abstract void Analyze();

        internal void RemoveAllInternalLoads()
        {
            _internalPointLoads = new List<InternalPointLoad3D>();
            _internalDistributedLoads = new List<InternalDistributedLoad3D>();
        }

        internal void RemoveAllExternalLoads()
        {
            _externalPointLoads = new List<PointLoad3D>();
            _externalDistributedLoads = new List<DistributedLoad3D>();
        }

        /// <summary>
        /// Returns the distance from a point to a line, in 3D
        /// </summary>
        /// <param name="p1">End I of the line</param>
        /// <param name="p2">End J of the line</param>
        /// <param name="p0">Point in question</param>
        /// <returns></returns>
        protected double GetDistanceFromPointToLine(Point3D p1, Point3D p2, Point3D p0)
        {
            Vector3D x1 = new Vector3D(p1.X, p1.Y, p1.Z);
            Vector3D x2 = new Vector3D(p2.X, p2.Y, p2.Z);
            Vector3D x0 = new Vector3D(p0.X, p0.Y, p0.Z);

            double d = ((x0 - x1).CrossProduct(x0 - x2)).Length / (x2 - x1).Length;

            return d;
        }

        protected double GetDistanceFromPointToLine2D(Point3D p1, Point3D p2, Point3D p0)
        {
            double x1 = p1.X;
            double y1 = p1.Y;
            double x2 = p2.X;
            double y2 = p2.Y;
            double x0 = p0.X;
            double y0 = p0.Y;

            if (Math.Abs(x2 - x1) < 1e-10 && Math.Abs(y2 - y1) < 1e-10)
            {
                //This is a vertical element that would create simply a point (not a line) in the X-Y plane
                return Math.Sqrt(Math.Pow(x0 - x1, 2) + Math.Pow(y0 - y1, 2));
            }
            else
            {
                return Math.Abs((x2 - x1) * (y1 - y0) - (x1 - x0) * (y2 - y1)) / Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            }
        }

        

        protected internal void AddReactionLoads(IEnumerable<IInternalLoad> loads)
        {
            foreach (var load in loads)
            {
                if (load is InternalPointLoad3D)
                {
                    _internalPointLoads.Add((InternalPointLoad3D) load);
                }
                else if (load is InternalDistributedLoad3D)
                {
                    _internalDistributedLoads.Add((InternalDistributedLoad3D) load);
                }
            }
        }

        protected double Interpolate(double x0, double y0, double x1, double y1, double x)
        {
            return (y1 - y0) / (x1 - x0) * (x - x0) + y0;
        }

        protected PointLoad3D InterpolatePointLoad(double StartDist, PointLoad3D StartLoad, double EndDist,
            PointLoad3D EndLoad, Point3D Location, double Dist)
        {
            double loadMagnitude_Fx = Interpolate(StartDist, StartLoad.Fx, EndDist, EndLoad.Fx, Dist);
            double loadMagnitude_Fy = Interpolate(StartDist, StartLoad.Fy, EndDist, EndLoad.Fy, Dist);
            double loadMagnitude_Fz = Interpolate(StartDist, StartLoad.Fz, EndDist, EndLoad.Fz, Dist);
            double loadMagnitude_Mx = Interpolate(StartDist, StartLoad.Mx, EndDist, EndLoad.Mx, Dist);
            double loadMagnitude_My = Interpolate(StartDist, StartLoad.My, EndDist, EndLoad.My, Dist);
            double loadMagnitude_Mz = Interpolate(StartDist, StartLoad.Mz, EndDist, EndLoad.Mz, Dist);

            return new PointLoad3D(Location, loadMagnitude_Fx, loadMagnitude_Fy, loadMagnitude_Fz, loadMagnitude_Mx,
                loadMagnitude_My, loadMagnitude_Mz, StartLoad.LoadPattern);
        }

        protected InternalDistributedLoad3D GetDistributedLoadFromReaction(InternalDistributedLoad3D load)
        {
            if (GetDistanceFromPointToLine2D(EndI, EndJ, load.StartLoad.Location) <= FUZZDISTANCE)
            {
                if (GetDistanceFromPointToLine2D(EndI, EndJ, load.EndLoad.Location) <= FUZZDISTANCE)
                {
                    //If both the start and end points are on the line that defines the element (within a fuzz distance), we can try to add it
                    Vector3D Vector_I_J = EndJ - EndI;
                    UnitVector3D UnitVector_I_J = Vector_I_J.Normalize();

                    Vector3D Vector_I_StartLoad = load.StartLoad.Location - EndI;
                    Vector3D Vector_I_EndLoad = load.EndLoad.Location - EndI;

                    double StartLoadProjection = Math.Abs(Vector_I_StartLoad.DotProduct(UnitVector_I_J));
                    double EndLoadProjection = Math.Abs(Vector_I_EndLoad.DotProduct(UnitVector_I_J));
                    double ElementLength = Math.Abs(Vector_I_J.DotProduct(UnitVector_I_J));

                    PointLoad3D StartLoad = null;
                    PointLoad3D EndLoad = null;

                    //Check start load

                    if (Math.Sign(Vector_I_StartLoad.DotProduct(UnitVector_I_J)) ==
                        Math.Sign(Vector_I_J.DotProduct(UnitVector_I_J)))
                    {
                        //The start load position is towards the J end from the I end
                        if (Math.Sign(Vector_I_EndLoad.DotProduct(UnitVector_I_J)) ==
                            Math.Sign(Vector_I_J.DotProduct(UnitVector_I_J)))
                        {
                            //The end load position is towards the J end from the I end
                            if (Math.Abs(Vector_I_StartLoad.DotProduct(UnitVector_I_J)) <
                                Math.Abs(Vector_I_EndLoad.DotProduct(UnitVector_I_J)))
                            {
                                //the start load is before the end load
                                if (Math.Abs(Vector_I_StartLoad.DotProduct(UnitVector_I_J)) <
                                    Math.Abs(Vector_I_J.DotProduct(UnitVector_I_J)))
                                {
                                    //The start load is before the end of the member
                                    StartLoad = load.StartLoad; //The start load is acceptable

                                    if (Math.Abs(Vector_I_EndLoad.DotProduct(UnitVector_I_J)) <=
                                        Math.Abs(Vector_I_J.DotProduct(UnitVector_I_J)))
                                    {
                                        //The end load is before or equal to the end of the member
                                        EndLoad = load.EndLoad;
                                    }
                                    else
                                    {
                                        //The end load is beyond the end of the member, so we must interpolate
                                        EndLoad = InterpolatePointLoad(StartLoadProjection, load.StartLoad,
                                            EndLoadProjection, load.EndLoad, EndJ, ElementLength);
                                    }
                                }

                                //Otherwise, we don't add the load (the start load is off of the member and the end load is beyond the start load)
                            }
                            else
                            {
                                //The end load is before the start load
                                //We have already determined that the end load is in the same direction as the J end, so it is definitely on the element
                                StartLoad = load.EndLoad;

                                if (Math.Abs(Vector_I_StartLoad.DotProduct(UnitVector_I_J)) <=
                                    Math.Abs(Vector_I_J.DotProduct(UnitVector_I_J)))
                                {
                                    //The start load is before or equal to the end of the member
                                    EndLoad = load.StartLoad;
                                }
                                else
                                {
                                    //The start load is beyond the end of the member, so we must interpolate
                                    EndLoad = InterpolatePointLoad(EndLoadProjection, load.EndLoad, StartLoadProjection,
                                        load.StartLoad, EndJ, ElementLength);
                                }
                            }
                        }
                        else
                        {
                            //The end load position is away from the J end, or is at the I end
                            if (Math.Sign(Vector_I_EndLoad.DotProduct(UnitVector_I_J)) == 0)
                            {
                                //The end load is at the I end
                                StartLoad = load.EndLoad;
                            }
                            else
                            {
                                //The end load is before the I end, so we must interpoate
                                StartLoad = InterpolatePointLoad(0, load.EndLoad,
                                    EndLoadProjection + StartLoadProjection, load.StartLoad, EndI, EndLoadProjection);
                            }


                            if (Math.Abs(Vector_I_StartLoad.DotProduct(UnitVector_I_J)) <=
                                Math.Abs(Vector_I_J.DotProduct(UnitVector_I_J)))
                            {
                                //If the start load is less than or equal to the end of the member
                                EndLoad = load.StartLoad;
                            }
                            else
                            {
                                //The start load is beyond the end of the member, so we need to interpolate
                                EndLoad = InterpolatePointLoad(0, load.EndLoad, EndLoadProjection + StartLoadProjection,
                                    load.StartLoad, EndJ, EndLoadProjection + ElementLength);
                            }
                        }
                    }
                    else
                    {
                        //The start load projecttion is opposite of the member direction (i.e. the start load is before the I end), or is at the I end
                        if (Math.Sign(Vector_I_EndLoad.DotProduct(UnitVector_I_J)) ==
                            Math.Sign(Vector_I_J.DotProduct(UnitVector_I_J)))
                        {
                            //The end load position is towards the J end from the I end
                            if (Vector_I_StartLoad.DotProduct(UnitVector_I_J) == 0)
                            {
                                //The start load is at end I
                                StartLoad = load.StartLoad;
                            }
                            else
                            {
                                //The start load is befoore end I, so we need to interpolate
                                StartLoad = InterpolatePointLoad(0, load.StartLoad,
                                    StartLoadProjection + EndLoadProjection, load.EndLoad, EndI, StartLoadProjection);
                            }

                            if (Math.Abs(Vector_I_EndLoad.DotProduct(UnitVector_I_J)) <=
                                Math.Abs(Vector_I_J.DotProduct(UnitVector_I_J)))
                            {
                                //The end load is before or equal to the end of the member
                                EndLoad = load.EndLoad;
                            }
                            else
                            {
                                //The end load is beyond the end of the member, so we must interpolate
                                EndLoad = InterpolatePointLoad(0, load.StartLoad,
                                    StartLoadProjection + EndLoadProjection, load.EndLoad, EndJ,
                                    StartLoadProjection + ElementLength);
                            }
                        }
                    }

                    if (StartLoad != null && EndLoad != null)
                    {
                        InternalDistributedLoad3D newIDL3D =
                            new InternalDistributedLoad3D(StartLoad, EndLoad, load.Source);
                        return newIDL3D;
                    }
                }
            }

            return new InternalDistributedLoad3D(new PointLoad3D(EndI, 0, 0, 0, 0, 0, 0, LoadPattern.Dead),
                new PointLoad3D(EndJ, 0, 0, 0, 0, 0, 0, LoadPattern.Dead), load.Source);
        }

        public override string ToString()
        {
            return ($"element {ID}");
        }

        protected abstract void ApplySelfWeight();

        protected abstract double GetSelfWeightLineLoadMagnitude();

        public abstract List<FloorLoad> GetSelfWeightLoadsForSeismicMass();

        public abstract void ReduceLiveLoads(IEnumerable<AnalyticalElement> supportedElements);

        public void AddExternalLoad(Load3D load)
        {
            if (load.GetType().Equals(typeof(DistributedLoad3D)) == true)
            {
                _externalDistributedLoads.Add((DistributedLoad3D)load);
            }
            else if (load.GetType().Equals(typeof(PointLoad3D)) == true)
            {
                _externalPointLoads.Add((PointLoad3D)load);
            }
        }

        public void RedefineEndI(Point3D endI)
        {
            EndI = endI;
        }

        public void RedefineEndJ(Point3D endJ)
        {
            EndJ = endJ;
        }
    }
}