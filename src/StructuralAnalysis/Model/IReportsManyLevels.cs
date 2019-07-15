using System.Collections.Generic;
using Kataclysm.Common;

namespace Kataclysm.StructuralAnalysis.Model
{
    public interface IReportsManyLevels
    {
        IReadOnlyList<BuildingLevel> Levels { get; }
    }
}