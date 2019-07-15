using System;
using Kataclysm.Common;
using Newtonsoft.Json;

namespace Kataclysm.StructuralAnalysis.Rigid
{
    public class NodalForce
    {
        [JsonIgnore]
        public double Fx { get; set; }
        public double Fy { get; set; }
        public double Mz { get; set; }
        public LoadPattern LoadPattern { get; set; }

        public NodalForce()
        {
        }

        public NodalForce(double fx, double fy, double mz, LoadPattern loadPattern)
        {
            Fx = fx;
            Fy = fy;
            Mz = mz;
            LoadPattern = loadPattern;
        }

        public double SRSS()
        {
            return Math.Sqrt(Math.Pow(Fx, 2) + Math.Pow(Fy, 2));
        }
    }
}
