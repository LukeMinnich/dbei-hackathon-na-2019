using System;
using System.Collections.Generic;
using Kataclysm.Common.Extensions;

namespace Kataclysm.Common.Units.Conversion
{
    public class FLT : IEquatable<FLT>
    {
        public int ForceDegree { get; }
        public int LengthDegree { get; }
        public int TimeDegree { get; }

        private static readonly Dictionary<FLT, UnitType> _types;

        static FLT()
        {
            _types = new Dictionary<FLT, UnitType>
            {
                {Area, UnitType.Area},
                {Density, UnitType.Density},
                {Force, UnitType.Force},
                {ForcePerLength, UnitType.ForcePerLength},
                {FlexuralStiffness, UnitType.FlexuralStiffness},
                {Length, UnitType.Length},
                {LengthCubed, UnitType.LengthCubed},
                {LengthToThe4th, UnitType.LengthToThe4th},
                {LengthToThe6th, UnitType.LengthToThe6th},
                {Moment, UnitType.Moment},
                {Stress, UnitType.Stress},
                {Time, UnitType.Time},
                {Unitless, UnitType.Unitless}
            };
        }

        public FLT(int forceDegree, int lengthDegree, int timeDegree)
        {
            ForceDegree = forceDegree;
            LengthDegree = lengthDegree;
            TimeDegree = timeDegree;
        }

        public UnitType GetType()
        {
            return _types.ContainsKey(this)
                ? _types[this]
                : UnitType.Undefined;
        }
        
        public static FLT Area => new FLT(0, 2, 0);
        public static FLT Density => new FLT(1, -3, 0);
        public static FLT Force => new FLT(1, 0, 0);
        public static FLT ForcePerLength => new FLT(1, -1, 0);
        public static FLT FlexuralStiffness => new FLT(1, 2, 0);
        public static FLT Length => new FLT(0, 1, 0);
        public static FLT LengthCubed => new FLT(0, 3, 0);
        public static FLT LengthToThe4th => new FLT(0, 4, 0);
        public static FLT LengthToThe6th => new FLT(0, 6, 0);
        public static FLT Moment => new FLT(1, 1, 0);
        public static FLT Stress => new FLT(1, -2, 0);
        public static FLT Time => new FLT(0, 0, 1);
        public static FLT Unitless => new FLT(0, 0, 0);

        public bool Equals(FLT other)
        {
            return this == other;
        }
        
        #region Equality Overrides

        public override bool Equals(object obj)
        {
            return obj is FLT other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ForceDegree.GetHashCode() ^ LengthDegree.GetHashCode() ^ TimeDegree.GetHashCode();
        }

        public static bool operator ==(FLT a, FLT b)
        {
            return a.ForceDegree == b.ForceDegree &&
                   a.LengthDegree == b.LengthDegree &&
                   a.TimeDegree == b.TimeDegree;
        }

        public static bool operator !=(FLT a, FLT b)
        {
            return !(a == b);
        }

        public static FLT operator +(FLT a, FLT b)
        {
            var f = a.ForceDegree + b.ForceDegree;
            var l = a.LengthDegree + b.LengthDegree;
            var t = a.TimeDegree + b.TimeDegree;

            return new FLT(f, l, t);
        }
        
        #endregion
        
        #region Math Operator Overloads

        public static FLT operator -(FLT a, FLT b)
        {
            var f = a.ForceDegree - b.ForceDegree;
            var l = a.LengthDegree - b.LengthDegree;
            var t = a.TimeDegree - b.TimeDegree;

            return new FLT(f, l, t);
        }

        public static FLT operator -(FLT a)
        {
            var f = -a.ForceDegree;
            var l = -a.LengthDegree;
            var t = -a.TimeDegree;

            return new FLT(f, l, t);
        }

        public static FLT operator *(FLT a, int b)
        {
            var f = a.ForceDegree * b;
            var l = a.LengthDegree * b;
            var t = a.TimeDegree * b;
            
            return new FLT(f, l, t);
        }

        public static FLT operator /(FLT a, double b)
        {
            var f = a.ForceDegree / b;
            var l = a.LengthDegree / b;
            var t = a.TimeDegree / b;

            if (!f.IsBasicallyAnInteger(out var nearestF) ||
                !l.IsBasicallyAnInteger(out var nearestL) ||
                !t.IsBasicallyAnInteger(out var nearestT))
            {
                return Unitless;
            }
            
            return new FLT(nearestF, nearestL, nearestT);
        }
        
        #endregion
    }
}