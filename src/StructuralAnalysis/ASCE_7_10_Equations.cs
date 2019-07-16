using System;
using System.Collections.Generic;
using Kataclysm.Common.Reporting;
using Kataclysm.Common.Units.Conversion;
using Katerra.Apollo.Structures.Common.Units;

namespace Kataclysm.StructuralAnalysis
{
    public static class ASCE7_10_Equations
    {
        private static readonly ReferenceStandard _standard = ReferenceStandard.ASCE_7_10;

        public static class Chapter_12
        {
            public static readonly string Chapter = "Chapter 12";

            public static class Section_12_8
            {
                private static readonly string SectionNumber = "12.8";

                /// <summary>
                /// Returns the seismic base shear of the building
                /// </summary>
                /// <param name="C_s">Seismic response coefficient determined in accordance with Section 12.8.1.1</param>
                /// <param name="W">Effective seismic weight per Section 12.7.2</param>
                /// <returns></returns>
                public static Force Eqn_12_8_1(Unitless C_s, Force W, ref RecordedCalculation calc)
                {
                    string equationNumber = "12.8-1";
                    string expression = @"C_s W";
                    string resultDisplay = @"V";

                    SubCalculation subCalc = new SubCalculation(_standard, "Calculate seismic base shear of the building", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable("C_s", C_s, UnitlessUnit.Unitless);
                    subCalc.AddVariable("W", W, ForceUnit.Kip);

                    subCalc.Result = C_s * W;

                    calc.AddCalculation(subCalc);

                    return (Force) subCalc.Result;
                }

                /// <summary>
                /// Calculates the seismic response coefficient
                /// </summary>
                /// <param name="S_DS">Design spectral response acceleration parameter in the short period range as determined from Section 11.4.4 or 11.4.7</param>
                /// <param name="R">Response modification factor in Table 12.2-1</param>
                /// <param name="I_e">Importance factor determined in accordance with Section 11.5.1</param>
                /// <returns></returns>
                public static Unitless Eqn_12_8_2(double S_DS, double R, double I_e, ref RecordedCalculation Calculation)
                {
                    string equationNumber = "12.8-2";
                    string expression = @"\frac{S_{DS}}{\left({\frac{R}{I_e}}\right)}";
                    string resultDisplay = @"C_s";

                    SubCalculation subCalc = new SubCalculation(_standard, "Calculate seismic response coefficient", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable(@"S_{DS}", S_DS));
                    subCalc.AddVariable(new LaTexVariable("R", R));
                    subCalc.AddVariable(new LaTexVariable("I_e", I_e));

                    subCalc.Result = new Unitless(S_DS / (R / I_e));

                    Calculation.AddCalculation(subCalc);

                    return (Unitless) subCalc.Result;
                }

                /// <summary>
                /// Calculates the seismic response coefficient limit where T <= T_L
                /// </summary>
                /// <param name="S_D1">Design spectral response acceleration parameter at a period of 1.0 s, as determined from Section 11.4.4 or 11.4.7</param>
                /// <param name="T">Fundamental period of the structure(s) determined in Section 12.8.2</param>
                /// <param name="R">Response modification factor in Table 12.2-1</param>
                /// <param name="I_e">Importance factor determined in accordance with Section 11.5.1</param>
                /// <returns></returns>
                public static Unitless Eqn_12_8_3(double S_D1, Time T, double R, double I_e, ref RecordedCalculation Calculation)
                {
                    string equationNumber = "12.8-3";
                    string expression = @"\frac{S_{D1}}{T \left({\frac{R}{I_e}}\right)}";
                    string resultDisplay = @"C_s";

                    SubCalculation subCalc = new SubCalculation(_standard, @"Calculate seismic response coefficient limit where $T \le T_L$", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable(@"S_{D1}", S_D1));
                    subCalc.AddVariable(new LaTexVariable("T", T, TimeUnit.Seconds));
                    subCalc.AddVariable(new LaTexVariable("R", R));
                    subCalc.AddVariable(new LaTexVariable("I_e", I_e));

                    subCalc.Result = new Unitless((S_D1 / (T * R / I_e)).Value);

                    Calculation.AddCalculation(subCalc);

                    return (Unitless) subCalc.Result;
                }

                /// <summary>
                /// Calculates the seismic response coefficient limit where T > T_L
                /// </summary>
                /// <param name="S_D1">Design spectral response acceleration parameter at a period of 1.0 s, as determined from Section 11.4.4 or 11.4.7</param>
                /// <param name="T_L">Long-period transition period(s) determined in Section 11.4.5</param>
                /// <param name="T">Fundamental period of the structure(s) determined in Section 12.8.2</param>
                /// <param name="R">Response modification factor in Table 12.2-1</param>
                /// <param name="I_e">Importance factor determined in accordance with Section 11.5.1</param>
                /// <returns></returns>
                public static Unitless Eqn_12_8_4(double S_D1, Time T_L, Time T, double R, double I_e, ref RecordedCalculation Calculation)
                {
                    string equationNumber = "12.8-4";
                    string expression = @"\frac{S_{D1} T_L}{T^2 \left({\frac{R}{I_e}}\right)}";
                    string resultDisplay = @"C_s";

                    SubCalculation subCalc = new SubCalculation(_standard, @"Calculate seismic response coefficient limit where $T > T_L$", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable(@"S_{D1}", S_D1));
                    subCalc.AddVariable(new LaTexVariable("T_L", T_L, TimeUnit.Seconds));
                    subCalc.AddVariable(new LaTexVariable("T", T, TimeUnit.Seconds));
                    subCalc.AddVariable(new LaTexVariable("R", R));
                    subCalc.AddVariable(new LaTexVariable("I_e", I_e));

                    subCalc.Result = new Unitless((S_D1 * T_L / ((T ^ 2) * (R / I_e))).Value);

                    Calculation.AddCalculation(subCalc);

                    return (Unitless) subCalc.Result;
                }

                /// <summary>
                /// Calculates the seismic response coefficient minimum
                /// </summary>
                /// <param name="S_DS">Design spectral response acceleration parameter in the short period range as determined from Section 11.4.4 or 11.4.7</param>
                /// <param name="I_e">Importance factor determined in accordance with Section 11.5.1</param>
                /// <returns></returns>
                public static Unitless Eqn_12_8_5(double S_DS, double I_e, ref RecordedCalculation Calculation)
                {
                    string equationNumber = "12.8-5";
                    string expression = @"0.044 S_{DS} I_e";
                    string resultDisplay = @"C_s";

                    SubCalculation subCalc = new SubCalculation(_standard, @"Calculate seismic response coefficient minimum where $C_s \ge 0.01$", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable(@"S_{DS}", S_DS));
                    subCalc.AddVariable(new LaTexVariable("I_e", I_e));

                    subCalc.Result = new Unitless(0.044 * S_DS * I_e);
                    subCalc.Limit = new ResultLimit(new Unitless(0.01), ResultLimitType.MaximumOf);

                    Calculation.AddCalculation(subCalc);

                    return (Unitless) subCalc.Result;
                }

                /// <summary>
                /// Calculates the seismic response coefficient where S_1 >= 0.6g
                /// </summary>
                /// <param name="S_1">Mapped maximum considered earthquake spectral response aceleration parameter determined in accordance with Section 11.4.1 or 11.4.7</param>
                /// <param name="R">Response modification factor in Table 12.2-1</param>
                /// <param name="I_e">Importance factor determined in accordance with Section 11.5.1</param>
                /// <returns></returns>
                public static Unitless Eqn_12_8_6(double S_1, double R, double I_e, ref RecordedCalculation Calculation)
                {
                    string equationNumber = "12.8-6";
                    string expression = @"0.5 S_1/(R/I_e)";
                    string resultDisplay = @"C_s";

                    SubCalculation subCalc = new SubCalculation(_standard, @"Calculate seismic response coefficient minimum where $S_1 \ge 0.6 g$", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable("S_1", S_1);
                    subCalc.AddVariable("R", R);
                    subCalc.AddVariable("I_e", I_e);

                    subCalc.Result = new Unitless(0.5 * S_1 / (R / I_e));

                    Calculation.AddCalculation(subCalc);

                    return (Unitless) subCalc.Result;
                }

                /// <summary>
                /// Calculates approximate fundamental period (T_a), in s
                /// </summary>
                /// <param name="C_t">Coefficient determined from Table 12.8-2</param>
                /// <param name="h_n">Structural height as defined in Section 11.2</param>
                /// <param name="x">Coefficient determined from Table 12.8-2</param>
                /// <returns></returns>
                public static Time Eqn_12_8_7(Length h_n, double C_t, double x, ref RecordedCalculation Calculation)
                {
                    string equationNumber = "12.8-7";
                    string expression = @"C_t h_n^{x}";
                    string resultDisplay = @"T_a";

                    SubCalculation subCalc = new SubCalculation(_standard, @"Calculate the approximate fundamental period", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable(@"C_t", C_t));
                    subCalc.AddVariable(new LaTexVariable("h_n", h_n, LengthUnit.Foot));
                    subCalc.AddVariable(new LaTexVariable("x", x));

                    subCalc.Result = new Time(C_t * Math.Pow(h_n.ConvertTo(LengthUnit.Foot), x));

                    Calculation.AddCalculation(subCalc);

                    return (Time) subCalc.Result;
                }

                /// <summary>
                /// Calculates approximate fundamental period (T_a) where the seismic force-resisting system consists entirely of concrete or steel moment resisting frames
                /// </summary>
                /// <param name="T_a">Approximate fundamental period, in s</param>
                /// <param name="N">Number of stories above the base</param>
                /// <param name="note"></param>
                /// <returns></returns>
                public static double Eqn_12_8_8(int N, ref RecordedCalculation Calculation, string note = null)
                {
                    string equationNumber = "12.8-8";
                    string expression = @"0.1 N";
                    string resultDisplay = @"T_a";

                    SubCalculation subCalc = new SubCalculation(_standard, @"Calculate the approximate fundamental period", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable("N", N));

                    double result = 0.1 * N;

                    subCalc.Result = new Time(result);

                    if (note != null)
                    {
                        subCalc.AddNote(note);
                    }

                    Calculation.AddCalculation(subCalc);

                    return result;
                }

                /// <summary>
                /// Calculates approximate fundamental period (T_a) for masonry or concrete shear wall structures
                /// </summary>
                /// <param name="C_w"></param>                
                /// <param name="h_n"></param>
                /// <returns></returns>
                public static Time Eqn_12_8_9(Unitless C_w, Length h_n, ref RecordedCalculation Calculation)
                {
                    string equationNumber = "12.8-9";
                    string expression = @"\frac{0.0019}{\sqrt{C_w}} h_n";
                    string resultDisplay = @"T_a";

                    SubCalculation subCalc = new SubCalculation(_standard, @"Calculate the approximate fundamental period", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable("C_w", C_w, UnitlessUnit.Unitless));
                    subCalc.AddVariable(new LaTexVariable("h_n", h_n, LengthUnit.Foot));

                    subCalc.Result = new Time(0.0019 / Math.Sqrt(C_w.Value) * h_n.ConvertTo(LengthUnit.Foot));

                    Calculation.AddCalculation(subCalc);

                    return (Time) subCalc.Result;
                }

                /// <summary>
                /// Calculates coeffiction for equation 12.8-9
                /// </summary>
                /// <param name="A_B">Area of base of structure, in ft2</param>                
                /// <param name="x">Number of shear walls in the building effective in resisting lateral forces in the direction under consideration</param>
                /// <param name="h_n">Structural height of building</param>
                /// <param name="h">List of shear wall heights</param>
                /// <param name="A">List of shear wall areas</param>
                /// <param name="D">List of shear wall lengths</param>
                /// <returns></returns>
                public static Unitless Eqn_12_8_10(Area A_B, int x, Length h_n, List<double> h, List<double> A, List<double> D, ref RecordedCalculation Calculation)
                {
                    string equationNumber = "12.8-10";
                    string expression = @"\frac{100}{A_B} \sum\limits_{i=1}^{x} \left(\frac{h_n}{h_i}\right)^2 \frac{A_i}{\left[1+0.83 \left(\frac{h_i}{D_i}\right)^2\right]}";
                    string resultDisplay = @"C_w";

                    SubCalculation subCalc = new SubCalculation(_standard, @"Calculate the approximate fundamental period", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable("A_B", A_B, AreaUnit.SquareFoot));

                    Area sum = new Area(0, AreaUnit.SquareInch);

                    for (var i = 0; i < x; i++)
                    {
                        var h_i = new Length(h[i], LengthUnit.Inch);
                        var A_i = new Area(A[i], AreaUnit.SquareInch);
                        var D_i = new Length(D[i], LengthUnit.Inch);

                        sum = (Area) (sum + ((h_n / h_i) ^ 2) * A_i / (1 + 0.83 * ((h_i / D_i) ^ 2)));
                    }

                    subCalc.AddVariable(new LaTexVariable(@"\sum\limits_{i=1}^{x} \left(\frac{h_n}{h_i}\right)^2 \frac{A_i}{\left[1+0.83 \left(\frac{h_i}{D_i}\right)^2\right]}", sum, AreaUnit.SquareFoot));

                    subCalc.Result = 100 * sum / A_B;

                    Calculation.AddCalculation(subCalc);

                    return (Unitless) subCalc.Result;
                }

                /// <summary>
                /// Calculate the lateral seismic force induced at any level
                /// </summary>
                /// <param name="C_vx">Vertical distribution factor</param>                
                /// <param name="V">Total design lateral force or shear at the base of the structure</param>
                /// <returns></returns>
                public static Force Eqn_12_8_11(Unitless C_vx, Force V, ref RecordedCalculation calc)
                {
                    var equationNumber = "12.8-11";
                    var expression = @"C_{vx} V";
                    var resultDisplay = @"F_x";

                    SubCalculation subCalc = new SubCalculation(_standard, $"Calculate the lateral seismic force induced",
                        Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable("C_{vx}", C_vx, UnitlessUnit.Unitless));
                    subCalc.AddVariable(new LaTexVariable("V", V, ForceUnit.Kip));

                    subCalc.Result = C_vx * V;

                    calc.AddCalculation(subCalc);

                    return (Force) subCalc.Result;
                }

                /// <summary>
                /// Calculate the vertical distribution factor
                /// </summary>
                /// <param name="w_x">The portion of total effective seismic weight of the structure (W) located or assigned to Level x</param>                
                /// <param name="h_x">The height (ft) from the base to Level</param>
                /// <param name="k">Exponent related to the structural period</param>
                /// <param name="w">List of level seismic weights</param>
                /// <param name="h">List of level heights</param>
                /// <returns></returns>
                public static Unitless Eqn_12_8_12(Force w_x, Length h_x, double k, int n, List<Force> w,
                    List<Length> h, ref RecordedCalculation calc)
                {
                    string equationNumber = "12.8-12";
                    string expression = @"\frac{w_x h_x^k}{\sum\limits_{i=1}^{n} w_i h_i^k}";
                    string resultDisplay = @"C_{vx}";

                    SubCalculation subCalc = new SubCalculation(_standard, $"Calculate the vertical distribution factor", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    var h_xTok = new Length(Math.Pow(h_x.ConvertTo(LengthUnit.Inch), k), LengthUnit.Inch);
                    
                    subCalc.AddVariable("w_x", w_x, ForceUnit.Kip);
                    subCalc.AddVariable("h_x^k", h_xTok, LengthUnit.Foot);
                    
                    var sum = new Moment(0, MomentUnit.KipInch);

                    for (var i = 0; i < n; i++)
                    {
                        double h_i = h[i].ConvertTo(LengthUnit.Inch);
                        Force w_i = w[i];

                        sum = (Moment) (sum + w_i * new Length(Math.Pow(h_i, k), LengthUnit.Inch));
                    }

                    subCalc.AddVariable(@"\sum\limits_{i=1}^{n} w_i h_i^k", sum, MomentUnit.KipFoot);

                    subCalc.Result = w_x * (h_x ^ new Unitless(k)) / sum; 

                    calc.AddCalculation(subCalc);

                    return (Unitless) subCalc.Result;
                }

                /// <summary>
                /// Calculate the horizontal distribution of forces
                /// </summary>
                /// <param name="F">Ordered list of all story forces, with lowest level first</param>                
                /// <returns></returns>
                public static Force Eqn_12_8_13(List<Force> F, int x, ref RecordedCalculation Calculation)
                {
                    string equationNumber = "12.8-13";
                    string expression = @"\sum\limits_{i=x}^{n} F_i";
                    string resultDisplay = @"V_x";

                    SubCalculation subCalc = new SubCalculation(_standard, $"Calculate the horizontal distribution of forces",
                        Chapter, SectionNumber, equationNumber, expression, resultDisplay);
                    
                    Force sum = new Force(0, ForceUnit.Kip);

                    for (var i = F.Count - 1; i >= x; i--)
                    {
                        sum = (Force) (sum + F[i]);
                    }

                    subCalc.Result = sum;

                    Calculation.AddCalculation(subCalc);

                    return (Force) subCalc.Result;
                }

                /// <summary>
                /// Calculate the torsional amplification factor
                /// </summary>
                /// <param name="delta_max">The maximum displacement at Level x computed assuming Ax = 1 (in)</param>     
                /// <param name="delta_avg">The average of the displacements at the extreme points of the structure at Level x computed assuming Ax = 1</param>
                /// <param name="calc"></param>
                /// <returns></returns>
                public static Unitless Eqn_12_8_14(Length delta_max, Length delta_avg, ref RecordedCalculation calc)
                {
                    string equationNumber = "12.8-14";
                    string expression = @"\left(\frac{\delta_{max}}{1.2 \delta_{avg}}\right)^2";

                    var subCalc = new SubCalculation(_standard, $"Calculate Torsional Amplification Factor",
                        Chapter, "12.8.4.3", equationNumber, expression, @"A_x");

                    subCalc.AddVariable(new LaTexVariable(@"\delta_{max}", delta_max, LengthUnit.Inch));
                    subCalc.AddVariable(new LaTexVariable(@"\delta_{avg}", delta_avg, LengthUnit.Inch));

                    subCalc.Result = (delta_max / (1.2 * delta_avg)) ^ 2;

                    if (subCalc.Result.Value < 1.0)
                    {
                        subCalc.Limit = new ResultLimit(new Unitless(1.0), ResultLimitType.Override);
                    }
                    else if (subCalc.Result.Value > 3.0)
                    {
                        subCalc.Limit = new ResultLimit(new Unitless(3.0), ResultLimitType.Override);
                    }

                    calc.AddCalculation(subCalc);

                    return (Unitless) subCalc.Result;
                }

                /// <summary>
                /// Calculate the deflection at Level x
                /// </summary>
                /// <param name="C_d">The deflection amplification factor in Table 12.2-1</param>   
                /// <param name="d_xe">The deflection at the location required by this section determined by elastic analysis</param>   
                /// <param name="I_e">The importance factor determined in accordance with Section 11.5.1</param>           
                /// <returns></returns>
                public static double Eqn_12_8_15(double C_d, double d_xe, double I_e, ref RecordedCalculation Calculation, string note = null)
                {
                    string equationNumber = "12.8-15";
                    string expression = @"\frac{C_d \delta_{xe}}{I_e}";
                    string resultDisplay = @"\delta_x";

                    SubCalculation subCalc = new SubCalculation(_standard, $"Calculate the deflection", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable(@"C_d", C_d));
                    subCalc.AddVariable(new LaTexVariable(@"\delta_{xe}", new Length(d_xe, LengthUnit.Inch), LengthUnit.Inch));
                    subCalc.AddVariable(new LaTexVariable(@"I_e", I_e));

                    double result = C_d * d_xe / I_e;

                    subCalc.Result = new Length(result, LengthUnit.Inch);

                    Calculation.AddCalculation(subCalc);

                    return result;
                }

                /// <summary>
                /// Calculate the stability coefficient considered for P-Delta Effects
                /// </summary>
                /// <param name="P_x">The total vertical design load at and above Level x (kip); where computing Px, no individual load factor need exceed 1.0</param>   
                /// <param name="D">The design story drift as defined in Section 12.8.6 occurring simultaneously with Vx (in)</param>   
                /// <param name="I_e">The importance factor determined in accordance with Section 11.5.1</param>           
                /// <param name="V_x">The seismic shear force acting between Levels x and x-1 (kip)</param>               
                /// <param name="h_sx">The story height below Level x (in)</param>           
                /// <param name="C_d">The deflection amplification factor in Table 12.2-1</param>    
                /// <returns></returns>
                public static double Eqn_12_8_16(double P_x, double D, double I_e, double V_x, double h_sx, double C_d, ref RecordedCalculation Calculation, string note = null)
                {
                    string equationNumber = "12.8-16";
                    string expression = @"\frac{P_x \Delta I_e}{V_x h_{sx} C_d}";
                    string resultDisplay = @"\theta";

                    SubCalculation subCalc = new SubCalculation(_standard, $"Calculate the stability coefficient", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable(@"P_x", new Force(P_x, ForceUnit.Kip), ForceUnit.Kip));
                    subCalc.AddVariable(new LaTexVariable(@"\Delta", new Length(D, LengthUnit.Inch), LengthUnit.Inch));
                    subCalc.AddVariable(new LaTexVariable(@"I_e", I_e));
                    subCalc.AddVariable(new LaTexVariable(@"V_x", new Force(V_x, ForceUnit.Kip), ForceUnit.Kip));
                    subCalc.AddVariable(new LaTexVariable(@"h_{sx}", new Length(h_sx, LengthUnit.Inch), LengthUnit.Inch));
                    subCalc.AddVariable(new LaTexVariable(@"C_d", C_d));

                    double result = (P_x * D * I_e) / (V_x * h_sx * C_d);

                    subCalc.Result = new Unitless(result);

                    Calculation.AddCalculation(subCalc);

                    return result;
                }

                /// <summary>
                /// Calculate the maximum stability coefficient
                /// </summary>
                /// <param name="B">The ratio of shear demand to shear capacity for the story between levels x and x-1</param>       
                /// <param name="C_d">The deflection amplification factor in Table 12.2-1</param>    
                /// <returns></returns>
                public static double Eqn_12_8_17(double B, double C_d, ref RecordedCalculation Calculation, string note = null)
                {
                    string equationNumber = "12.8-17";
                    string expression = @"\frac{0.5}{\beta C_d}";
                    string resultDisplay = @"\theta_{max}";

                    SubCalculation subCalc = new SubCalculation(_standard, $"Calculate the maximum stability coefficient", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable(@"\beta", B));
                    subCalc.AddVariable(new LaTexVariable(@"C_d", C_d));

                    double result = 0.5 / (B * C_d);

                    if (result > 0.25)
                    {
                        result = 0.25;
                    }

                    subCalc.Result = new Unitless(result);

                    Calculation.AddCalculation(subCalc);

                    return result;
                }

                public static Unitless CalculateCs(double I_e, double S_1, double S_D1, double S_DS, Time T_L, double R, Time T, ref RecordedCalculation calculation)
                {
                    Unitless C_s = Eqn_12_8_2(S_DS, R, I_e, ref calculation);

                    if (T <= T_L)
                    {
                        Unitless C_s_12_8_3 = Eqn_12_8_3(S_D1, T, R, I_e, ref calculation);
                        C_s = (Unitless) Result.Min(C_s_12_8_3, C_s);
                    }
                    else if (T > T_L)
                    {
                        Unitless C_s_12_8_4 = Eqn_12_8_4(S_D1, T_L, T, R, I_e, ref calculation);
                        C_s = (Unitless) Result.Min(C_s_12_8_4, C_s);
                    }

                    Unitless C_s_12_8_5 = Eqn_12_8_5(S_DS, I_e, ref calculation);

                    C_s = (Unitless) Result.Max(C_s, C_s_12_8_5);

                    if (S_1 >= 0.6)
                    {
                        Unitless C_s_12_8_6 = Eqn_12_8_6(S_1, R, I_e, ref calculation);
                        C_s = (Unitless) Result.Max(C_s, C_s_12_8_6);
                    }

                    return C_s;
                }
            }

            public static class Section_12_10
            {
                private static readonly string SectionNumber = "12.10";

                /// <summary>
                /// Calculate diaphragm design forces
                /// </summary>   
                /// <param name="F">List of the applied design forces at each Level</param>           
                /// <param name="w">List of the tributary weights at each Level</param>            
                /// <param name="w_px">The tributary weight of the diaphragm at Level x</param>
                /// <returns></returns>
                public static Force Eqn_12_10_1(Force w_px, List<Force> F, List<Force> w, int x, ref RecordedCalculation Calculation)
                {
                    string equationNumber = "12.10-1";
                    string expression = @"\frac{\sum\limits_{i=x}^{n} F_i}{\sum\limits_{i=x}^{n} w_i} w_{px}";
                    string resultDisplay = @"F_{px}";

                    SubCalculation subCalc = new SubCalculation(_standard, $"Calculate the diaphragm design force", Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    Force sum_F = new Force(0, ForceUnit.Kip);
                    Force sum_w = new Force(0, ForceUnit.Kip);

                    subCalc.AddVariable(new LaTexVariable(@"w_{px}", w_px, ForceUnit.Kip));

                    for (var i = x; i >= 0; i--)
                    {
                        sum_F = (Force) (sum_F + F[i]);
                        sum_w = (Force) (sum_w + w[i]);
                    }

                    subCalc.AddVariable(@"\sum\limits_{i=x}^{n} F_i", sum_F, ForceUnit.Kip);
                    subCalc.AddVariable(@"\sum\limits_{i=x}^{n} w_i", sum_w, ForceUnit.Kip);

                    subCalc.Result = sum_F / sum_w * w_px;

                    Calculation.AddCalculation(subCalc);

                    return (Force) subCalc.Result;
                }

                /// <summary>
                /// Calculate minimum diaphragm design forces
                /// </summary>   
                /// <param name="S_DS">Design spectral response acceleration parameter in the short period range as determined from Section 11.4.4 or 11.4.7</param>         
                /// <param name="I_e">The importance factor determined in accordance with Section 11.5.1</param>              
                /// <param name="w_px">The tributary weight of the diaphragm at Level x</param>    
                /// <returns></returns>
                public static Force Eqn_12_10_2(double S_DS, double I_e, Force w_px, ref RecordedCalculation calc)
                {
                    string equationNumber = "12.10-2";
                    string expression = @"0.2 S_{DS} I_e w_{px}";
                    string resultDisplay = @"F_{px}";

                    SubCalculation subCalc = new SubCalculation(_standard, $"Calculate the minimum diaphragm design force",
                        Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable(@"w_{px}", w_px, ForceUnit.Kip));
                    subCalc.AddVariable(new LaTexVariable(@"I_e", I_e));
                    subCalc.AddVariable(new LaTexVariable(@"S_{DS}", S_DS));

                    subCalc.Result = 0.2 * S_DS * I_e * w_px;

                    calc.AddCalculation(subCalc);

                    return (Force) subCalc.Result;
                }

                /// <summary>
                /// Calculate maximum diaphragm design forces
                /// </summary>   
                /// <param name="S_DS">Design spectral response acceleration parameter in the short period range as determined from Section 11.4.4 or 11.4.7</param>         
                /// <param name="I_e">The importance factor determined in accordance with Section 11.5.1</param>              
                /// <param name="w_px">The tributary weight of the diaphragm at Level x</param>    
                /// <returns></returns>
                public static Force Eqn_12_10_3(double S_DS, double I_e, Force w_px, ref RecordedCalculation calc)
                {
                    string equationNumber = "12.10-3";
                    string expression = @"0.4 S_{DS} I_e w_{px}";
                    string resultDisplay = @"F_{px}";

                    SubCalculation subCalc = new SubCalculation(_standard, $"Calculate the maximum diaphragm design force",
                        Chapter, SectionNumber, equationNumber, expression, resultDisplay);

                    subCalc.AddVariable(new LaTexVariable(@"w_{px}", w_px, ForceUnit.Kip));
                    subCalc.AddVariable(new LaTexVariable(@"I_e", I_e));
                    subCalc.AddVariable(new LaTexVariable(@"S_{DS}", S_DS));

                    subCalc.Result = 0.4 * S_DS * I_e * w_px;

                    calc.AddCalculation(subCalc);

                    return (Force) subCalc.Result;
                }
            }
        }
    }
}