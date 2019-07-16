using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kataclysm.Common
{
    // Per https://lostechies.com/jimmybogard/2008/08/12/enumeration-classes/
    //   With modifications
    public abstract class Enumeration : IEquatable<Enumeration>
    {
        public int Value { get; }
        public string Description { get; }

        protected Enumeration(int value, string description)
        {
            Value = value;
            Description = description;
        }

        public override string ToString()
        {
            return Description;
        }

        #region API Methods

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public static IEnumerable<string> GetAllDescriptions<T>() where T : Enumeration
        {
            return GetAll<T>().Select(e => e.Description);
        }

        public static T FromValue<T>(int value) where T : Enumeration
        {
            return parse<T, int>(value, "value", item => item.Value == value);
        }

        public static T FromDescription<T>(string description) where T : Enumeration
        {
            return parse<T, string>(description, "description", item => item.Description == description);
        }

        public static T FromDescriptionCaseInsensitive<T>(string description) where T : Enumeration
        {
            return parse<T, string>(description, "description",
                item => StringComparer.InvariantCultureIgnoreCase.Equals(item.Description, description));
        }

        private static T parse<T, K>(K value, string characteristic, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                var message = $"'{value}' is not a valid {characteristic} in {typeof(T)}";

                throw new ArgumentException(message);
            }

            return matchingItem;
        }

        #endregion

        #region Equality Overrides

        public bool Equals(Enumeration other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            return obj is Enumeration other && this == other;
        }

        public static bool operator ==(Enumeration a, Enumeration b)
        {
            if (ReferenceEquals(null, a) ^ ReferenceEquals(null, b)) return false;
            if (ReferenceEquals(null, a)) return true;
            if (ReferenceEquals(a, b)) return true;

            return a.Value == b.Value && a.Description == b.Description;
        }

        public static bool operator !=(Enumeration a, Enumeration b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Value * 397) ^ (Description != null ? Description.GetHashCode() : 0);
            }
        }

        #endregion
    }
}