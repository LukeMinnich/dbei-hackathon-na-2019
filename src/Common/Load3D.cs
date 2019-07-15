namespace Kataclysm.Common
{
    public abstract class Load3D
    {
        public LoadPattern LoadPattern { get; set; } = LoadPattern.Dead;

        public abstract double TotalVerticalForce { get; }
    }
}