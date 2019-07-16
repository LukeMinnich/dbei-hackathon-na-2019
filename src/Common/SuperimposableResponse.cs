namespace Kataclysm.Common
{
    public abstract class SuperimposableResponse<T> : IReportsLoadPattern, IReportsElementId
    {
        public string ElementId { get; internal set; }
        public LoadPattern LoadPattern { get; internal set; }
        public LoadCase LoadCase { get; internal set; }

        protected SuperimposableResponse() {}
        
        protected SuperimposableResponse(string elementId, LoadPattern loadPattern, LoadCase loadCase)
        {
            ElementId = elementId;
            LoadPattern = loadPattern;
            LoadCase = loadCase;
        }

        public abstract T ApplyLoadFactor(double loadFactor);
        public abstract T Superimpose(T other);
    }
}