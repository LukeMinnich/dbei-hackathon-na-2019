using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildingLayout;
using Grasshopper.Kernel;
using MathNet.Spatial.Euclidean;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace GenerativeBuilding
{
    public class GenerativeBuildingComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public GenerativeBuildingComponent()
          : base("GenerativeBuilding", "GenerativeBuilding",
              "GenerativeBuilding",
              "GenerativeBuilding", "GenerativeBuilding")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("UnitDefCSV", "UnitDefCSV", "Unit Definition CSV", GH_ParamAccess.item);
            pManager.AddTextParameter("UnitMixCSV", "UnitMixCSV", "Unit Mix CSV", GH_ParamAccess.item);

            pManager.AddLineParameter("MainCorridor", "Main Corridor", "Main Corridor", GH_ParamAccess.item);
            pManager.AddLineParameter("LeftLeg", "Left Corridor", "Left Corridor", GH_ParamAccess.item);
            pManager.AddLineParameter("MiddleLeg", "Middle Corridor", "Middle Corridor", GH_ParamAccess.item);
            pManager.AddLineParameter("RightLeg", "Right Corridor", "Right Corridor", GH_ParamAccess.item);

            pManager.AddNumberParameter("Unit Depth", "Unit Depth", "Unit Depth", GH_ParamAccess.item);
            pManager.AddNumberParameter("Hall Width", "Hall Width", "Hall Width", GH_ParamAccess.item);

            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Units", "Units", "Units", GH_ParamAccess.list);
            pManager.AddLineParameter("CorridorLines", "CorridorLines", "CorridorLines", GH_ParamAccess.list);
            pManager.AddTextParameter("UnitNames", "UnitNames", "UnitNames", GH_ParamAccess.list);
            pManager.AddLineParameter("ShearWallLines", "ShearWallLines", "ShearWallLines", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var defPath = "";
            var mixPath = "";
            var mainCorridor = new Line();
            var leftLeg = new Line();
            var middleLeg = new Line();
            var rightLeg = new Line();
            double unitDepth = 0;
            double hallWidth = 0;

            DA.GetData(0, ref defPath);
            DA.GetData(1, ref mixPath);
            DA.GetData(2, ref mainCorridor);
            DA.GetData(3, ref leftLeg);
            DA.GetData(4, ref middleLeg);
            DA.GetData(5, ref rightLeg);
            DA.GetData(6, ref unitDepth);
            DA.GetData(7, ref hallWidth);

            var unitList = new List<BuildingUnit>();

            var csv = File.ReadAllLines(defPath);

            for (int i = 1; i < csv.Length; i++)
            {
                var lineSplit = csv[i].Split(',');

                unitList.Add(new BuildingUnit(lineSplit[0], lineSplit[1], Convert.ToDouble(lineSplit[2]), Convert.ToDouble(lineSplit[3]), Convert.ToDouble(lineSplit[4]), Convert.ToDouble(lineSplit[5])));
            }

            BuildingUnitMix desiredUnitMix = new BuildingUnitMix();


            csv = File.ReadAllLines(mixPath);

            for (int i = 1; i < csv.Length; i++)
            {
                var lineSplit = csv[i].Split(',');

                desiredUnitMix.Add(lineSplit[0], Convert.ToDouble(lineSplit[2]));
            }

            var corridors = new List<Corridor>();

            var mainCorridorObject = new MainCorridor();
            var location = CorridorLocation.Main;

            var corridorLineList = new List<UnitsLine>();

            var leftLegObject = new Corridor();
            if (leftLeg.Length > 0.001)
            {
                var leftLine2D = CreateLine2D(leftLeg);
                location = CorridorLocation.LeftLeg;
                leftLegObject = new Corridor(leftLine2D, unitDepth, hallWidth, location);
                corridorLineList.AddRange(leftLegObject.GetLines());
            }

            var midLegObject = new Corridor();
            if (middleLeg.Length > 0.001)
            {
                var midLine2D = CreateLine2D(middleLeg);
                location = CorridorLocation.MiddleLeg;
                midLegObject = new Corridor(midLine2D, unitDepth, hallWidth, location);
                corridorLineList.AddRange(midLegObject.GetLines());
            }

            var rightLegObject = new Corridor();
            if (rightLeg.Length > 0.001)
            {
                var rightLine2D = CreateLine2D(rightLeg);
                location = CorridorLocation.RightLeg;
                rightLegObject = new Corridor(rightLine2D, unitDepth, hallWidth, location);
                corridorLineList.AddRange(rightLegObject.GetLines());
            }

            var mainLine2D = CreateLine2D(mainCorridor);
            location = CorridorLocation.Main;
            mainCorridorObject.CenterLine = mainLine2D;
            mainCorridorObject.UnitDepth = unitDepth;
            mainCorridorObject.HallWidth = hallWidth;
            mainCorridorObject.CorridorLocation = location;

            mainCorridorObject.LeftLeg = leftLegObject;
            mainCorridorObject.MiddleLeg = midLegObject;
            mainCorridorObject.RightLeg = rightLegObject;

            mainCorridorObject.CalculateMainLines();


            var currentPercentage = new BuildingUnitMix();
            foreach (var kvp in desiredUnitMix)
            {
                currentPercentage.Add(kvp.Key, 0);
            }
            corridorLineList.AddRange(mainCorridorObject.GetLines());

            corridorLineList.OrderBy(x => x.Line.Length);
            double usedLength = 0;

            var unitPriority = new List<string>()
            {
                "C",
                "B",
                "A",
                "S"
            };

            var totalUnitList = new List<Tuple<string, double>>();

            var polyUnits = new List<PolylineCurve>();
            var unitNames = new List<string>();

            var shearWallLines = new List<Line>();

            foreach (var corridorLine in corridorLineList)
            {
                var corridorLength = corridorLine.Line.Length;
                var units = FittingAlgorithm.CreateUnitLine(unitList, desiredUnitMix, corridorLength, ref currentPercentage, unitPriority, ref totalUnitList, ref usedLength);

                var buildingUnits = new List<BuildingUnit>();

                double startLength = 0;
                var lineVector = corridorLine.Line.Direction;

                var angle = new Vector2D(1, 0).SignedAngleTo(lineVector);


                foreach (var unit in units)
                {
                    var matchingUnit = unitList.Single(x => (x.Type == unit.Item1 && x.Width == unit.Item2));

                    matchingUnit.Rotation = angle;
                    matchingUnit.Location = corridorLine.Line.StartPoint + startLength * lineVector;

                    var unitPolygon = matchingUnit.GetPolygon();

                    var unitWallLines = matchingUnit.GetShearWallLines();

                    foreach (var unitLine in unitWallLines)
                    {
                        shearWallLines.Add(new Line(new Point3d(unitLine.StartPoint.X, unitLine.StartPoint.Y, 0), new Point3d(unitLine.EndPoint.X, unitLine.EndPoint.Y, 0)));


                    }
                    var unitPoints = unitPolygon.ToPolyLine2D().Vertices;

                    var point3dList = new List<Point3d>();
                    foreach (var vertex in unitPoints)
                    {
                        point3dList.Add(new Point3d(vertex.X, vertex.Y, 0));
                    }

                    var unitPolyline = new PolylineCurve(point3dList);
                    polyUnits.Add(unitPolyline);
                    unitNames.Add(matchingUnit.Type);



                    buildingUnits.Add(matchingUnit);

                    startLength += matchingUnit.Width;
                }

                corridorLine.BuildingUnits = buildingUnits;
            }

            var lineList = new List<Line>();
            foreach (var corridorLine in corridorLineList)
            {
                lineList.Add(new Line(new Point3d(corridorLine.Line.StartPoint.X, corridorLine.Line.StartPoint.Y, 0), new Point3d(corridorLine.Line.EndPoint.X, corridorLine.Line.EndPoint.Y, 0)));
            }


            //foreach(var corridorLine in corridorLineList)
            //{
            //    foreach(var unit in corridorLine.BuildingUnits)
            //    {
            //        foreach(var shearWallLine in unit.GetShearWallLines())
            //        {
            //            shearWallLines.Add(new Line(new Point3d(shearWallLine.StartPoint.X, shearWallLine.StartPoint.Y, 0), new Point3d(shearWallLine.EndPoint.X, shearWallLine.EndPoint.Y, 0)));
            //        }

            //    }
            //}

            DA.SetDataList(0, polyUnits);
            DA.SetDataList(1, lineList);
            DA.SetDataList(2, unitNames);
            DA.SetDataList(3, shearWallLines);
        }

        private static Line2D CreateLine2D(Line leftLeg)
        {
            var startPoint = leftLeg.ToNurbsCurve().PointAtStart;
            var leftStart = new Point2D(startPoint.X, startPoint.Y);
            var endPoint = leftLeg.ToNurbsCurve().PointAtEnd;
            var leftEnd = new Point2D(endPoint.X, endPoint.Y);
            var leftLine2D = new Line2D(leftStart, leftEnd);

            return leftLine2D;
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a15d5ad7-d3ca-4d74-a0d1-7f8549cb2ba6"); }
        }
    }
}
