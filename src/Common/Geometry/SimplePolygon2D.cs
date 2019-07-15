using System.Collections.Generic;
using MathNet.Spatial.Euclidean;

namespace Kataclysm.Common.Geometry
{
    public class SimplePolygon2D : Polygon2D
    {
        #region Constructors

        public SimplePolygon2D(IEnumerable<Point2D> vertices)
            : base(vertices)
        {
            CheckForSimplePolygon();
        }

        #endregion

        #region API Functions

        private void CheckForSimplePolygon()
        {
            if (!IsSimplePolygon())
            {
                throw new SimplePolygonException("Polygon must be simple. " +
                                                 "No polygon edges can intersect.");
            }
        }

        #endregion

        #region Private Functions

        private bool IsSimplePolygon()
        {
            return IsTriangularPolygon() || IsSimplePolygonWithFourOrMoreSides();
        }

        private bool IsTriangularPolygon()
        {
            return VertexCount == 3;
        }

        private bool IsSimplePolygonWithFourOrMoreSides()
        {
            return VertexCount >= 4 && NoEdgesIntersect();
        }

        private bool NoEdgesIntersect()
        {
            return this.SelfIntersections().Count == 0;
        }

        #endregion
    }
}