namespace Kataclysm.Common
{
    public class SiteParameters
    {
        public double TL { get; set; } // long period transition period per ASCE 7 seismic maps

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
    }
}