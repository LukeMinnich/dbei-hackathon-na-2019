using Newtonsoft.Json;

namespace Kataclysm.Common.Reporting
{
    public class ReferenceStandard
    {
        public string ReferenceName { get; set; }
        public string PublishingBody { get; set; }
        public string Acronym { get; set; } = "";


        [JsonConstructor]
        public ReferenceStandard(string name, string publisher)
        {
            ReferenceName = name;
            PublishingBody = publisher;
        }

        public ReferenceStandard(string name, string publisher, string acronym) : this(name, publisher)
        {
            Acronym = acronym;
        }

        #region IBC

        public static ReferenceStandard IBC_2015 => new ReferenceStandard(GoverningCode.ICC_IBC_2015.ToString(),
            "International Building Code", "IBC-2015");

        #endregion
        
        #region Steel

        public static ReferenceStandard AISC_360_10 => new ReferenceStandard(SteelCode.AISC_360_10.ToString(),
            "American Institute Of Steel Construction", "AISC 360-10");

        #endregion
        
        #region CFS

        public static ReferenceStandard AISI_S100_16 => new ReferenceStandard(CFSCode.AISI_S100_16.ToString(),
            "American Iron and Steel Institute", "AISI S100-16");

        public static ReferenceStandard AISI_S240_15 => new ReferenceStandard(CFSCode.AISI_S240_15.ToString(),
            "American Iron and Steel Institute", "AISI S240-15");

        public static ReferenceStandard AISI_S400_15 => new ReferenceStandard(CFSCode.AISI_S400_15.ToString(),
            "American Iron and Steel Institute", "AISI S400-15");
        
        #endregion

        #region Wood

        public static ReferenceStandard NDS_2015 =>
            new ReferenceStandard(WoodCode.AWC_NDS_2015.ToString(), "American Wood Council", "NDS-2015");

        public static ReferenceStandard TR_14 => new ReferenceStandard(
            "Technical Report 14: Design for Lateral-Torsional Stability in Wood Members",
            "American Forest and Paper Association", "TR-14");

        #endregion

        #region Other

        public static ReferenceStandard Mechanics => new ReferenceStandard("Mechanics", "The Universe");

        #endregion

        #region ASCE

        public static ReferenceStandard ASCE_7_10 => new ReferenceStandard(GoverningCode.ASCE_7_10.ToString(),
            "American Society of Civil Engineers Minimum Design Loads for Buildings and Other Structures", "ASCE 7-10");

        #endregion
    }
}
