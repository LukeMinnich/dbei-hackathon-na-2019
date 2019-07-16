using System;
using System.Collections.Generic;

namespace Kataclysm.Common.Extensions
{
    public static class ListExtensions
    {
        private static readonly System.Random _random = new Random();
        
        public static T Random<T>(this List<T> source)
        {
            return source[_random.Next(source.Count)];
        }
    }
}