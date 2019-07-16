using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildingLayout
{
    public static class FittingAlgorithm
    {
        public static List<Tuple<string, double>> CreateUnitLine(List<BuildingUnit> unitList, BuildingUnitMix desiredUnitMix, double corridorLength, ref BuildingUnitMix currentPercentage, List<string> unitPriority, ref List<Tuple<string, double>> totalUnitList, ref double usedLength)
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
                            var addedLength = 3;
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
