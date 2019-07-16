namespace Kataclysm.StructuralAnalysis.Model
{
    public class SiteParameters
    {
        public double T0 { get; set; }
        public double TL { get; set; } // long period transition period per ASCE 7 seismic maps
        public double TS { get; set; }

        // Mapped spectral acceleration parameters per ASCE 7-10 section 11.4.1
        public double Ss { get; set; }
        public double S1 { get; set; }

        // Site coefficients per ASCE 7-10 section 11.4.3
        public double Fa { get; set; }
        public double Fv { get; set; }

        // Design spectral acceleration parameters per ASCE 7-10 section 11.4.4
        public double SDS { get; set; }
        public double SD1 { get; set; }

        public double SMS { get; set; }
        public double SM1 { get; set; }

        public SiteParameters()
        {
        }

        public SiteParameters(double ss, double s1, double fa, double fv, double sms, double sm1, double sds,
            double sd1, double tl, double t0, double ts)
        {
            Ss = ss;
            S1 = s1;
            Fa = fa;
            Fv = fv;
            SMS = sms;
            SM1 = sm1;
            SDS = sds;
            SD1 = sd1;
            TL = tl;
            T0 = t0;
            TS = ts;
        }
    }
}