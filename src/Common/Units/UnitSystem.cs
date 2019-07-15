namespace Katerra.Apollo.Structures.Common.Units
{
    public struct UnitSystem
    {
        public ForceUnit ForceUnit { get; set; }
        public LengthUnit LengthUnit { get; set; }
        public StressUnit StressUnit { get; set; }

        public UnitSystem(ForceUnit forceUnit, LengthUnit lengthUnit, StressUnit stressUnit)
        {
            ForceUnit = forceUnit;
            LengthUnit = lengthUnit;
            StressUnit = stressUnit;
        }

        public static UnitSystem ImperialDefault => new UnitSystem()
        {
            ForceUnit = ForceUnit.Kip,
            LengthUnit = LengthUnit.Inch,
            StressUnit = StressUnit.ksi
        };
        
        public static UnitSystem ImperialKipInch => new UnitSystem()
        {
            ForceUnit =  ForceUnit.Kip,
            LengthUnit = LengthUnit.Inch,
            StressUnit = StressUnit.ksi
        };
        
        public static UnitSystem ImperialPoundFoot => new UnitSystem()
        {
            ForceUnit = ForceUnit.Pound,
            LengthUnit = LengthUnit.Foot,
            StressUnit = StressUnit.psf
        };
        
        public static UnitSystem ImperialPoundInch => new UnitSystem()
        {
            ForceUnit = ForceUnit.Pound,
            LengthUnit = LengthUnit.Inch,
            StressUnit = StressUnit.psi
        };
        
        public static UnitSystem ImperialKipFoot => new UnitSystem()
        {
            ForceUnit = ForceUnit.Kip,
            LengthUnit = LengthUnit.Foot,
            StressUnit = StressUnit.ksf
        };
    }
}