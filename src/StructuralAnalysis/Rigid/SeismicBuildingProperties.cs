using System;
using System.Collections.Generic;
using Kataclysm.Common;
using Kataclysm.StructuralAnalysis.Model;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class SeismicBuildingProperties
    {
        public SeismicParameters SeismicParameters { get; set; }
        public BuildingParameters BuildingParameters { get; set; }
        public double BuildingWeight { get; set; }
        public double RedundancyFactor { get; set; }
        public double k { get; set; }
        public double T { get; set; }

        private Dictionary<BuildingLevel, MassCenter> _levelMasses;

        public SeismicBuildingProperties(SeismicParameters seismicParameters, ModelSettings modelSettings,
            Dictionary<BuildingLevel,MassCenter> levelMasses)
        {
            SeismicParameters = seismicParameters;
            _levelMasses = levelMasses;
            BuildingWeight = CalculateBuildingMass(levelMasses);
            T = ApproximatePeriod(modelSettings.BuildingHeight);
            k = CalculateKfactor();
        }

        private double CalculateBuildingMass(Dictionary<BuildingLevel, MassCenter> levelMasses)
        {
            double mass = 0;
            foreach (KeyValuePair<BuildingLevel,MassCenter> KVP in levelMasses)
            {
                mass += KVP.Value.Weight;
            }

            return mass;
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