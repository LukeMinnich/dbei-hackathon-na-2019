using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Kataclysm.Common.Extensions;
using Katerra.Apollo.Structures.Common.Units;

namespace Kataclysm.Common.Units.Conversion
{
    public abstract class Result
    {
        public FLT FLT { get; }
        public double Value { get; }
        public Unit DisplayUnit { get; set; }

        protected Result(FLT flt, double value, Unit displayUnit)
        {
            FLT = flt;
            Value = value;
            DisplayUnit = displayUnit;
        }
        
        public abstract string ToLaTexString();
        public abstract double ConvertTo(Unit targetUnit);

        #region Overload muliplication(*) operator

        public static Result operator *(Result a, Result b)
        {
            return BuildTypedResult(a.FLT + b.FLT, a.Value * b.Value);
        }

        public static Result operator *(double a, Result b)
        {
            return BuildTypedResult(b.FLT, a * b.Value);
        }

        public static Result operator *(Result a, double b)
        {
            return b * a;
        }

        #endregion

        #region Overload division(/) operator

        public static Result operator /(Result a, Result b)
        {
            return BuildTypedResult(a.FLT - b.FLT, a.Value / b.Value);
        }

        public static Result operator /(double a, Result b)
        {
            return BuildTypedResult(-b.FLT, a / b.Value);
        }

        public static Result operator /(Result a, double b)
        {
            return BuildTypedResult(a.FLT, a.Value / b);
        }

        #endregion

        #region Overload plus(+) operator

        public static Result operator +(Result a, Result b)
        {
            ConfirmUnitsMatch(a, b);

            return BuildTypedResult(a.FLT, a.Value + b.Value);
        }

        public static Result operator +(double a, Result b)
        {
            ConfirmUnitless(b);

            return BuildTypedResult(b.FLT, a + b.Value);
        }

        public static Result operator +(Result a, double b)
        {
            return b + a;
        }

        #endregion

        #region Overload minus(-) operator

        public static Result operator -(Result a, Result b)
        {
            ConfirmUnitsMatch(a, b);

            return BuildTypedResult(a.FLT, a.Value - b.Value);
        }

        public static Result operator -(double a, Result b)
        {
            ConfirmUnitless(b);

            return BuildTypedResult(b.FLT, a - b.Value);
        }

        public static Result operator -(Result a, double b)
        {
            ConfirmUnitless(a);

            return BuildTypedResult(a.FLT, a.Value - b);
        }

        #endregion

        #region Overload unary minus(-) operator

        public static Result operator -(Result a)
        {
            return BuildTypedResult(a.FLT, -a.Value);
        }

        #endregion
        
        #region Overload comparison (>, >=, <, <=) operators

        public static bool operator >(Result a, Result b)
        {
            ConfirmUnitsMatch(a, b);

            return a.Value > b.Value;
        }

        public static bool operator >=(Result a, Result b)
        {
            ConfirmUnitsMatch(a, b);

            return a.Value >= b.Value;
        }

        public static bool operator <(Result a, Result b)
        {
            return !(a >= b);
        }

        public static bool operator <=(Result a, Result b)
        {
            return !(a > b);
        }
        
        #endregion

        #region Create exponentiation(^) operator

        public static Result operator ^(Result a, Result exp)
        {
            ConfirmUnitless(exp);

            return a ^ exp.Value;
        }

        public static Result operator ^(double a, Result exp)
        {
            ConfirmUnitless(exp);

            return BuildTypedResult(FLT.Unitless, Math.Pow(a, exp.Value));
        }

        public static Result operator ^(Result a, double exp)
        {
            if (exp.IsBasicallyAnInteger(out int expAsInteger))
            {
                // Result can be returned safely with augmented FLT
                return a ^ expAsInteger;
            }

            // Result must morph to unitless - partial FLT degrees not allowed
            return BuildTypedResult(FLT.Unitless, Math.Pow(a.Value, exp));
        }

        public static Result operator ^(Result a, int exp)
        {
            return BuildTypedResult(a.FLT * exp, Math.Pow(a.Value, exp));
        }

        #endregion

        #region Root functions

        public static Result Sqrt(Result a)
        {
            return BuildTypedResult(a.FLT / 2, Math.Pow(a.Value, 1d / 2));
        }

        public static Result ThirdRoot(Result a)
        {
            return BuildTypedResult(a.FLT / 3, Math.Pow(a.Value, 1d / 3));
        }

        public static Result FourthRoot(Result a)
        {
            return BuildTypedResult(a.FLT / 4, Math.Pow(a.Value, 1d / 4));
        }

        #endregion

        #region Absolute function

        public static Result Abs(Result a)
        {
            return BuildTypedResult(a.FLT, Math.Abs(a.Value));
        }

        public static double Abs(double a)
        {
            return Math.Abs(a);
        }

        #endregion

        #region Min/Max functions

        public static Result Min(Result a, Result b)
        {
            ConfirmUnitsMatch(a, b);

            return BuildTypedResult(a.FLT, Math.Min(a.Value, b.Value));
        }

        public static Result Min(IEnumerable<Result> results)
        {
            var allResults = results.ToList();
            
            ValidateCollection(allResults);

            var min = allResults[0];

            for (var i = 1; i < allResults.Count; i++)
            {
                if (allResults[i].Value < min.Value) min = allResults[i];
            }

            return min;
        }

        public static Result Max(Result a, Result b)
        {
            ConfirmUnitsMatch(a, b);

            return BuildTypedResult(a.FLT, Math.Max(a.Value, b.Value));
        }

        public static Result Max(IEnumerable<Result> results)
        {
            var allResults = results.ToList();
            
            ValidateCollection(allResults);

            var max = allResults[0];

            for (var i = 1; i < allResults.Count; i++)
            {
                if (allResults[i].Value > max.Value) max = allResults[i];
            }

            return max;
        }
        
        #endregion
        
        #region Envelope functions

        public static Result AbsoluteValueEnvelope(Result a, Result b)
        {
            ConfirmUnitsMatch(a, b);

            return Max(Abs(a), Abs(b));
        }

        public static Result MinValueEnvelope(Result a, Result b)
        {
            ConfirmUnitsMatch(a, b);

            return Min(a, b);
        }

        public static Result MaxValueEnvelope(Result a, Result b)
        {
            ConfirmUnitsMatch(a, b);

            return Max(a, b);
        }
        
        #endregion
        
        #region Trigonometric Functions

        public static Result Sin(Result a)
        {
            ConfirmUnitless(a);

            return BuildTypedResult(a.FLT, Math.Sin(a.Value));
        }

        public static Result Cos(Result a)
        {
            ConfirmUnitless(a);

            return BuildTypedResult(a.FLT, Math.Cos(a.Value));
        }

        public static Result Tan(Result a)
        {
            ConfirmUnitless(a);

            return BuildTypedResult(a.FLT, Math.Tan(a.Value));
        }

        public static Result Asin(Result a)
        {
            ConfirmUnitless(a);

            return BuildTypedResult(a.FLT, Math.Asin(a.Value));
        }

        public static Result Acos(Result a)
        {
            ConfirmUnitless(a);

            return BuildTypedResult(a.FLT, Math.Acos(a.Value));
        }

        public static Result Atan(Result a)
        {
            ConfirmUnitless(a);

            return BuildTypedResult(a.FLT, Math.Atan(a.Value));
        }
        
        #endregion
        
        #region Combinatorial functions

        public static Result SRSS(Result a, Result b)
        {
            ConfirmUnitsMatch(a, b);

            return BuildTypedResult(a.FLT * 2, Math.Sqrt(a.Value * a.Value + b.Value * b.Value));
        }
        
        #endregion

        private static Result BuildTypedResult(FLT flt, double result)
        {
            switch (flt.GetType())
            {
                case UnitType.Area:
                    return Area.CreateWithStandardUnits(result);
                case UnitType.Density:
                    return Density.CreateWithStandardUnits(result);
                case UnitType.Force:
                    return Force.CreateWithStandardUnits(result);
                case UnitType.ForcePerLength:
                    return ForcePerLength.CreateWithStandardUnits(result);
                case UnitType.FlexuralStiffness:
                    return FlexuralStiffness.CreateWithStandardUnits(result);
                case UnitType.Length:
                    return Length.CreateWithStandardUnits(result);
                case UnitType.LengthCubed:
                    return LengthCubed.CreateWithStandardUnits(result);
                case UnitType.LengthToThe4th:
                    return LengthToThe4th.CreateWithStandardUnits(result);
                case UnitType.LengthToThe6th:
                    return LengthToThe6th.CreateWithStandardUnits(result);
                case UnitType.Moment:
                    return Moment.CreateWithStandardUnits(result);
                case UnitType.Stress:
                    return Stress.CreateWithStandardUnits(result);
                case UnitType.Time:
                    return Time.CreateWithStandardUnits(result);
                case UnitType.Undefined:
                    return new Undefined(flt, result);
                case UnitType.Unitless:
                    return new Unitless(result);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        private static void ValidateCollection(IEnumerable<Result> results)
        {
            var allResults = results.ToList();
            
            if (allResults.Count == 0) throw new ArgumentException("expected at least one value");

            if (allResults.GroupBy(r => r.FLT).Count() != 1)
            {
                throw new ArgumentException("expected units to match");
            }
        }

        private static void ConfirmUnitsMatch(Result a, Result b)
        {
            if (a.FLT != b.FLT) throw new ArgumentException("expected units to match");
        }

        private static void ConfirmUnitless(Result a)
        {
            if (a.FLT != FLT.Unitless)
                throw new ArgumentException("expected unitless argument");
        }
    }
}