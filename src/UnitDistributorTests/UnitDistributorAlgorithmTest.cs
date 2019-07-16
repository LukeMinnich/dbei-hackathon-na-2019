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
            var parent = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(path).FullName).FullName).FullName).FullName);
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

            var unitLine = new List<Tuple<string,double>>();

            double usedLength = 0;

            var corridorLengthRemaining = corridorLength;

            var minWidth = unitList.Select(x => x.Width).Min();
            var failInt = 0;

            while (corridorLengthRemaining > 0)
            {
                foreach(var unit in unitPriority)
                {
                    if(currentPercentage[unit] < desiredUnitMix[unit])
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

                        corridorLengthRemaining -= unitLength;

                        break;
                    }
                }


                foreach (var unitCheck in unitPriority)
                {
                    var totalUnitLineLength = unitLine.Where(x => x.Item1 == unitCheck).Select(x => x.Item2).Sum();

                    currentPercentage[unitCheck] = totalUnitLineLength / usedLength;
                }


                if (corridorLengthRemaining<minWidth)
                {
                    for (int i = 0; i < unitLine.Count; i++)
                    {
                        var unit = unitLine[i];

                        if(currentPercentage[unit.Item1] < desiredUnitMix[unit.Item1])
                        {
                            var availableUnits = unitList.Where(x => x.Type == unit.Item1).ToList();
                            var currentUnit = availableUnits.Single(x => x.Width == unit.Item2);
                            var currentIndex = availableUnits.IndexOf(currentUnit);

                            unitLine[i] = new Tuple<string,double>(availableUnits[currentIndex + 1].Type, availableUnits[currentIndex+1].Width);
                            var addedLength = 3;
                            usedLength += addedLength;

                            foreach (var unitCheck in unitPriority)
                            {
                                var totalUnitLineLength = unitLine.Where(x => x.Item1 == unitCheck).Select(x => x.Item2).Sum();

                                currentPercentage[unitCheck] = totalUnitLineLength / usedLength;
                            }

                            corridorLengthRemaining -= addedLength;
                        }

                        if (corridorLengthRemaining == 0)
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







        }
    }
}