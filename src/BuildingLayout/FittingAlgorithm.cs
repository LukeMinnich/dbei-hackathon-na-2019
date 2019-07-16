using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BuildingLayout
{
    public static class FittingAlgorithm
    {
        public static void CreateSerializedModel(List<BuildingUnit> unitList, BuildingUnitMix desiredUnitMix, Line2D mainCorridor, Line2D leftLeg, Line2D middleLeg, Line2D rightLeg, double unitDepth, double hallWidth)
        {
            List<Polygon2D> polyUnits = new List<Polygon2D>();
            List<string> unitNames = new List<string>();
            List<Line2D> shearWallLines = new List<Line2D>();
            List<Line2D> lineList = new List<Line2D>();
            MainCorridor mainCorridorObject = new MainCorridor();

            GetBuildingLayout(unitList, desiredUnitMix, mainCorridor, leftLeg, middleLeg, rightLeg, unitDepth, hallWidth, out polyUnits, out unitNames, out shearWallLines, out lineList, out mainCorridorObject);
            var outlinePoints = GetOutlinePoints(unitDepth, hallWidth, mainCorridorObject);

            
        }


        public static List<Point2D> GetOutlinePoints(double unitDepth, double hallWidth, MainCorridor mainCorridorObject)
        {
            var finalPoints = new List<Point2D>() { };

            var corridorWidth = unitDepth * 2 + hallWidth;
            var moveVectorY = corridorWidth / 2 * new Vector2D(0, 1);
            var moveVectorX = corridorWidth / 2 * new Vector2D(1, 0);

            var main1 = mainCorridorObject.CenterLine - moveVectorY;
            var main2 = mainCorridorObject.CenterLine + moveVectorY;

            finalPoints.Add(main1.EndPoint);
            finalPoints.Add(main1.StartPoint);

            if (mainCorridorObject.LeftLeg.CenterLine.Length > 0.001)
            {
                var left1 = mainCorridorObject.LeftLeg.CenterLine - moveVectorX;
                var left2 = mainCorridorObject.LeftLeg.CenterLine + moveVectorX;

                finalPoints.Add(left1.EndPoint);
                finalPoints.Add(left2.EndPoint);
                finalPoints.Add(left2.StartPoint + moveVectorY);
            }
            else
            {
                finalPoints.Add(main2.StartPoint);
            }

            if (mainCorridorObject.MiddleLeg.CenterLine.Length > 0.001)
            {
                var mid1 = mainCorridorObject.MiddleLeg.CenterLine - moveVectorX;
                var mid2 = mainCorridorObject.MiddleLeg.CenterLine + moveVectorX;

                finalPoints.Add(mid1.StartPoint + moveVectorY);
                finalPoints.Add(mid1.EndPoint);
                finalPoints.Add(mid2.EndPoint);
                finalPoints.Add(mid2.StartPoint + moveVectorY);
            }
            else
            {

            }
            if (mainCorridorObject.RightLeg.CenterLine.Length > 0.001)
            {
                var right1 = mainCorridorObject.RightLeg.CenterLine - moveVectorX;
                var right2 = mainCorridorObject.RightLeg.CenterLine + moveVectorX;

                finalPoints.Add(right1.StartPoint + moveVectorY);
                finalPoints.Add(right1.EndPoint);
                finalPoints.Add(right2.EndPoint);
            }
            else
            {
                finalPoints.Add(main2.EndPoint);
            }


            finalPoints.Add(main1.EndPoint);

            return finalPoints;
        }

        public static void GetBuildingLayout(List<BuildingUnit> unitList, BuildingUnitMix desiredUnitMix, Line2D mainCorridor, Line2D leftLeg, Line2D middleLeg, Line2D rightLeg, double unitDepth, double hallWidth, out List<Polygon2D> polyUnits, out List<string> unitNames, out List<Line2D> shearWallLines, out List<Line2D> lineList, out MainCorridor mainCorridorOutput)
        {
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

            polyUnits = new List<Polygon2D>();
            unitNames = new List<string>();
            shearWallLines = new List<Line2D>();
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
                        shearWallLines.Add(new Line2D(new Point2D(unitLine.StartPoint.X, unitLine.StartPoint.Y), new Point2D(unitLine.EndPoint.X, unitLine.EndPoint.Y)));


                    }
                    var unitPoints = unitPolygon.ToPolyLine2D().Vertices;

                    var unitPolyline = new Polygon2D(unitPoints);
                    polyUnits.Add(unitPolyline);
                    unitNames.Add(matchingUnit.Type);



                    buildingUnits.Add(matchingUnit);

                    startLength += matchingUnit.Width;
                }

                corridorLine.BuildingUnits = buildingUnits;
            }

            lineList = new List<Line2D>();
            foreach (var corridorLine in corridorLineList)
            {
                lineList.Add(new Line2D(new Point2D(corridorLine.Line.StartPoint.X, corridorLine.Line.StartPoint.Y), new Point2D(corridorLine.Line.EndPoint.X, corridorLine.Line.EndPoint.Y)));
            }

            mainCorridorOutput = mainCorridorObject;
        }

        private static void GetUnitDefinitions(string defPath, string mixPath, out List<BuildingUnit> unitList, out BuildingUnitMix desiredUnitMix)
        {
            unitList = new List<BuildingUnit>();
            var csv = File.ReadAllLines(defPath);

            for (int i = 1; i < csv.Length; i++)
            {
                var lineSplit = csv[i].Split(',');

                unitList.Add(new BuildingUnit(lineSplit[0], lineSplit[1], Convert.ToDouble(lineSplit[2]), Convert.ToDouble(lineSplit[3]), Convert.ToDouble(lineSplit[4]), Convert.ToDouble(lineSplit[5])));
            }

            desiredUnitMix = new BuildingUnitMix();
            csv = File.ReadAllLines(mixPath);

            for (int i = 1; i < csv.Length; i++)
            {
                var lineSplit = csv[i].Split(',');

                desiredUnitMix.Add(lineSplit[0], Convert.ToDouble(lineSplit[2]));
            }
        }

        private static Line2D CreateLine2D(Line2D leftLeg)
        {
            var startPoint = leftLeg.StartPoint;
            var leftStart = new Point2D(startPoint.X, startPoint.Y);
            var endPoint = leftLeg.EndPoint;
            var leftEnd = new Point2D(endPoint.X, endPoint.Y);
            var leftLine2D = new Line2D(leftStart, leftEnd);

            return leftLine2D;
        }


        public static List<Tuple<string, double>> CreateUnitLine(List<BuildingUnit> unitList,
            BuildingUnitMix desiredUnitMix, double corridorLength, ref BuildingUnitMix currentPercentage,
            List<string> unitPriority, ref List<Tuple<string, double>> totalUnitList, ref double usedLength)
        {
            var unitLine = new List<Tuple<string, double>>();

            var corridorLengthRemaining = corridorLength;

            var minWidth = unitList.Select(x => x.Width).Min();
            var failInt = 0;

            while (corridorLengthRemaining > 0.001)
            {
                foreach (var unit in unitPriority)
                {
                    if (currentPercentage[unit] < desiredUnitMix[unit])
                    {
                        var availableUnits = unitList.Where(x => x.Type == unit).ToList();
                        var unitLength = availableUnits.Select(x => x.Width).Min();

                        if (unitLength > corridorLengthRemaining)
                        {
                            continue;
                        }

                        var unitArea = availableUnits.Select(x => x.Area()).Min();

                        usedLength += unitLength;
                        unitLine.Add(new Tuple<string, double>(unit, unitLength));
                        totalUnitList.Add(new Tuple<string, double>(unit, unitLength));

                        corridorLengthRemaining -= unitLength;

                        break;
                    }
                }

                foreach (var unitCheck in unitPriority)
                {
                    var totalUnitLineLength = totalUnitList.Where(x => x.Item1 == unitCheck).Select(x => x.Item2).Sum();

                    currentPercentage[unitCheck] = totalUnitLineLength / usedLength;
                }

                if (corridorLengthRemaining < minWidth)
                {
                    for (int i = 0; i < unitLine.Count; i++)
                    {
                        var unit = unitLine[i];

                        //if (currentPercentage[unit.Item1] < desiredUnitMix[unit.Item1])
                        //{
                            var availableUnits = unitList.Where(x => x.Type == unit.Item1).ToList();
                            var currentUnit = availableUnits.Single(x => x.Width == unit.Item2);
                            var currentIndex = availableUnits.IndexOf(currentUnit);

                            unitLine[i] = new Tuple<string, double>(availableUnits[currentIndex + 1].Type, availableUnits[currentIndex + 1].Width);
                            var addedLength = 36;
                            usedLength += addedLength;

                            foreach (var unitCheck in unitPriority)
                            {
                                var totalUnitLineLength = totalUnitList.Where(x => x.Item1 == unitCheck).Select(x => x.Item2).Sum();

                                currentPercentage[unitCheck] = totalUnitLineLength / usedLength;
                            }

                            corridorLengthRemaining -= addedLength;
                        //}

                        if (corridorLengthRemaining < 0.001)
                        {
                            break;
                        }
                    }

                }

                failInt += 1;

                if (failInt > 100)
                {
                    throw new Exception("Solution not found.");
                }
            }

            return unitLine;
        }
    }
}
