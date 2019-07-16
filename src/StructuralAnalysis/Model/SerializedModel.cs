using System;
using System.Collections.Generic;
using System.IO;
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

        public SerializedModel()
        {

        }

        public SerializedModel(double buildingHeight, int numberOfStories, Seismicity seismicity, List<Line2D> wallLines, List<Line2D> corridorLines, Polygon2D outline, List<Polygon2D> unitBoundaries, RandomizedBuilding randomizedBuilding, double randomizedPercent)
        {
            var levels = new List<BuildingLevel>();

            for (int i = 1; i <= numberOfStories; i++)
            {
                levels.Add(new BuildingLevel("LEVEL_" + i, buildingHeight * i));
            }

            ModelSettings = new ModelSettings(buildingHeight);

            var systemParameters = new SystemParameters();

            var seismicBaseLevel = new BuildingLevel("BASE", 0);

            var buildingParameters = new BuildingParameters(seismicBaseLevel);

            SeismicParameters = new SeismicParameters(systemParameters, buildingParameters, seismicity);

            var bearingWalls = new List<BearingWall>();
            var oneWayDecks = new List<OneWayDeck>();

            Random random = new Random();
            int test = random.Next(0, 2);

            foreach (var level in levels)
            {
                var wallLineID = 1;
                foreach (var wallLine in wallLines)
                {
                    if (random.NextDouble() < randomizedPercent)
                    {
                        var wallId = level.Name + "_" + wallLineID;
                        bearingWalls.Add(new BearingWall(wallId, wallLine, level));
                        wallLineID += 1;
                    }
                }

                foreach (var wallLine in corridorLines)
                {
                    var wallId = level.Name + "_" + wallLineID;

                    bearingWalls.Add(new BearingWall(wallId, wallLine, level));

                    wallLineID += 1;
                }

                oneWayDecks.Add(new OneWayDeck(outline, level));
            }

            OneWayDecks = oneWayDecks;

            UnitBoundaries = unitBoundaries;
        }

        public static SerializedModel CreateSerializedModel(string unitDefCSVPath, BuildingUnitMix desiredUnitMix, Seismicity seismicity, double buildingHeight, int numberOfStories, Line2D mainCorridor, Line2D leftLeg, Line2D middleLeg, Line2D rightLeg, double unitDepth, double hallWidth, RandomizedBuilding randomizedBuilding)
        {
            double randomizedPercent = 0.75;
            var importedUnitList = new List<BuildingUnit>();

            var csv = File.ReadAllLines(unitDefCSVPath);

            for (int i = 1; i < csv.Length; i++)
            {
                var lineSplit = csv[i].Split(',');

                importedUnitList.Add(new BuildingUnit(lineSplit[0], lineSplit[1], Convert.ToDouble(lineSplit[2]), Convert.ToDouble(lineSplit[3]), Convert.ToDouble(lineSplit[4]), Convert.ToDouble(lineSplit[5])));
            }
                       
            List<Polygon2D> polyUnits = new List<Polygon2D>();
            List<string> unitNames = new List<string>();
            List<Line2D> shearWallLines = new List<Line2D>();
            List<Line2D> lineList = new List<Line2D>();
            MainCorridor mainCorridorObject = new MainCorridor();

            FittingAlgorithm.GetBuildingLayout(importedUnitList, desiredUnitMix, mainCorridor, leftLeg, middleLeg, rightLeg, unitDepth, hallWidth, out polyUnits, out unitNames, out shearWallLines, out lineList, out mainCorridorObject);
            var outlinePoints = FittingAlgorithm.GetOutlinePoints(unitDepth, hallWidth, mainCorridorObject);
            var outlinePolygon = new Polygon2D(outlinePoints);

            return new SerializedModel(buildingHeight, numberOfStories, seismicity, shearWallLines, lineList, outlinePolygon, polyUnits, randomizedBuilding, randomizedPercent);
        }

    }
}