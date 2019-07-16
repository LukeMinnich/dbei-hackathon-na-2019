using System.Collections.Generic;
using System.Linq;
using BuildingLayout;
using Kataclysm.Common;
using Kataclysm.Common.Extensions;

namespace Kataclysm.Randomizer
{
    public static class Randomize
    {
        private static readonly List<UnitMix> UnitMixOptions = Enumeration.GetAll<UnitMix>().ToList();
        private static readonly List<Typology> Typologies = Enumeration.GetAll<Typology>().ToList();
        private static readonly List<Seismicity> SeismicityOptions = Enumeration.GetAll<Seismicity>().ToList();
        private static readonly List<int> NumbersOfStories = new List<int> {2, 3, 4, 5};
        private static readonly List<double> DriftLimits = new List<double> {1.5, 2.0, 2.5};

        public static RandomizedBuilding Random()
        {
            return new RandomizedBuilding()
            {
                UnitMix = UnitMixOptions.Random(),
                Typology = Typologies.Random(),
                Seismicity = SeismicityOptions.Random(),
                NumberOfStories = NumbersOfStories.Random(),
                DriftLimit = DriftLimits.Random()
            };
        }
    }
}