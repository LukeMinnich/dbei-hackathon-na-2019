using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Spatial.Euclidean;
using Newtonsoft.Json;

namespace Kataclysm.Common
{
    public abstract class AreaLoad : FloorLoad
    {
        public Polygon2D Boundary { get; set; }
        
        #region  Constructors
        public AreaLoad()
        {
        }

        public AreaLoad(Polygon2D boundary, BuildingLevel level, Projection projection, LoadPattern loadPattern)
        {
            Boundary = boundary;
            Level = level;
            Projection = projection;
            LoadPattern = loadPattern;
        }

        #endregion

        [JsonIgnore]
        public abstract Plane LoadPlane { get; }

        public abstract AreaLoad SubLoad(Polygon2D subBoundary);
    }
}