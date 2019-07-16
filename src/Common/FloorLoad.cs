namespace Kataclysm.Common
{
    public abstract class FloorLoad : IReportsLevel
    {
        public BuildingLevel Level { get; set; }
        public LoadPattern LoadPattern { get; set; }
        public Projection Projection { get; set; }
        
        public FloorLoad()
        {

        }

        public FloorLoad(BuildingLevel level, Projection projection, LoadPattern loadPattern)
        {
            Level = level;
            Projection = projection;
            LoadPattern = loadPattern;
        }
    }
}