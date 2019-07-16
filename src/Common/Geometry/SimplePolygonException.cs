using System;

namespace Kataclysm.Common.Geometry
{
    public class SimplePolygonException : Exception
    {
        public SimplePolygonException() : base()
        {
        }
        
        public SimplePolygonException(string message) : base(message)
        {
        }

        public SimplePolygonException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}


































