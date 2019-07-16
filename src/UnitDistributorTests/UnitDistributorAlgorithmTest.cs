using BuildingLayout;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tests
{
    public class Tests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void UnitDistributorTest()
        {
            var unitList = new List<BuildingUnit>();

            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var parent = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(path).FullName).FullName).FullName).FullName).FullName);
            var filePath = parent + @"\extern\units.csv";

            var csv = File.ReadAllLines(filePath);

            for (int i = 1; i < csv.Length; i++)
            {
                var lineSplit = csv[i].Split(",");

                unitList.Add(new BuildingUnit(lineSplit[0], lineSplit[1], Convert.ToDouble(lineSplit[2]), Convert.ToDouble(lineSplit[3]), Convert.ToDouble(lineSplit[4]), Convert.ToDouble(lineSplit[5])));

            }

            Assert.That(unitList.Count > 0);

            var desiredUnitMix = new BuildingUnitMix()
            {
                {"S", 0.15},
                {"A", 0.15},
                {"B", 0.40},
                {"C", 0.40}
            };

            double corridorLength = 300;
            double unitDepth = 27;

            double buildingAreaTotal = corridorLength * 2 * unitDepth;

            var currentPercentage = new BuildingUnitMix()
            {
                {"S", 0},
                {"A", 0},
                {"B", 0},
                {"C", 0}
            };

            var unitPriority = new List<string>()
            {
                "C",
                "B",
                "A",
                "S"
            };

            var totalUnitList = new List<Tuple<string, double>>();
            var usedLength = 0d;

            var unitLine = FittingAlgorithm.CreateUnitLine(unitList, desiredUnitMix, corridorLength, ref currentPercentage, unitPriority, ref totalUnitList, ref usedLength);

        }

        
    }
}