using System;
using System.ComponentModel;
using System.Reflection;

namespace Kataclysm.Common.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns the description associated with an enum value.  Returns null if the enum does not have a description.
        /// </summary>
        /// <param name="value">The enum value for which the description is desired</param>
        /// <returns>The enum description as a string</returns>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = 
                        Attribute.GetCustomAttribute(field, 
                            typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
        
        public static T GetEnumFromDescription<T>(this string description)
        {
            var type = typeof(T);
            if(!type.IsEnum) throw new InvalidOperationException();
            foreach(var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if(attribute != null)
                {
                    if(attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if(field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
            // or return default(T);
        }
    }
}