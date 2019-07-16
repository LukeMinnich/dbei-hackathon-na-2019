using System;
using System.Collections.Generic;
using System.Linq;
using Kataclysm.Common;
using Kataclysm.Common.Units.Conversion;
using Kataclysm.StructuralAnalysis.Model;
using Katerra.Apollo.Structures.Common.Units;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class SeismicBuildingProperties
    {
        public SeismicParameters SeismicParameters { get; set; }
//        public BuildingParameters BuildingParameters { get; set; }
        public double BuildingWeight { get; set; }
        public double k { get; set; }
        public double T { get; set; }

        public SeismicBuildingProperties(SeismicParameters seismicParameters, ModelSettings modelSettings, List<BuildingLevelLateral2> levels)
        {
            SeismicParameters = seismicParameters;
            BuildingWeight = CalculateBuildingWeight(levels).ConvertTo(ForceUnit.Kip);
            T = ApproximatePeriod(modelSettings.BuildingHeight);
            k = CalculateKfactor();
        }

        private Force CalculateBuildingWeight(List<BuildingLevelLateral2> levels)
        {
            Force weight = new Force(0, ForceUnit.Kip);

            foreach (BuildingLevelLateral2 level in levels)
            {
                weight = (Force) (weight + level.SeismicWeight);
            }

            return weight;
        }
        
        public double CalculateKfactor()
        // k exponent for ASCE 7 equation 12.8-12
        {
            double k;
            if (T <= 0.5)
            {
                k = 1d;
            }
            else if (T > 2.5)
            {
                k = 2d;
            }
            else
            {
                k = (T - .5) / 2d + 1d;
            }
            return k;
        }

        public double ApproximatePeriod(double height)
        {
            // units assumed to come in as inches, ASCE 7 expects feet in this equation
            double hn = height / 12;
            double x = SeismicParameters.SystemParameters.X;
            double Ct = SeismicParameters.SystemParameters.Ct;

            double Ta = Ct * Math.Pow(hn, x);

            return Ta;
        }

        public double SeismicResponseCoefficent()
        {
            double R = SeismicParameters.SystemParameters.R;
            double Cs = SeismicParameters.SiteParameters.SDS / R * SeismicParameters.BuildingParameters.ImportanceFactor;

            double Csmax;
            double Csmin;
            double Cstemp = 0;
            double Csused;

            if (T < SeismicParameters.SiteParameters.TL)
            {
                Csmax = Math.Min(Cs, SeismicParameters.SiteParameters.SD1 / T / R * SeismicParameters.BuildingParameters.ImportanceFactor);
            }
            else
            {
                Csmax = Math.Min(Cs, SeismicParameters.SiteParameters.SD1 * SeismicParameters.SiteParameters.TL / Math.Pow(SeismicParameters.SiteParameters.TL, 2) / R * SeismicParameters.BuildingParameters.ImportanceFactor);
            }

            if (SeismicParameters.SiteParameters.S1 > 0.6)
            {
                Cstemp = Math.Max(Cs, 0.5 * SeismicParameters.SiteParameters.S1 / R * SeismicParameters.BuildingParameters.ImportanceFactor);
            }

            Csmin = Math.Max(Math.Max(0.044 * SeismicParameters.SiteParameters.SDS * SeismicParameters.BuildingParameters.ImportanceFactor, 0.01), Cstemp);

            Csused = Math.Max(Math.Min(Cs, Csmax), Csmin);

            return Csused;
        }

        public double BaseShear()
        {
            double WBuilding = BuildingWeight;
            double Cs = SeismicResponseCoefficent();
            double Vbase = WBuilding * Cs;

            return Vbase;
        }
    }
}