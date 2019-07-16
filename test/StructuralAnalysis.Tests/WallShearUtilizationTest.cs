using Kataclysm.Common.Units.Conversion;
using Kataclysm.StructuralAnalysis.Evaluation;
using Katerra.Apollo.Structures.Common.Units;
using NUnit.Framework;

namespace Kataclysm.StructuralAnalysis.Tests
{
    [TestFixture]
    public class WallShearUtilizationTest
    {
        [Test]
        public void Normalize_Succeeds()
        {
            var utilization = new WallShearUtilization(new ForcePerLength(1500, ForcePerLengthUnit.PoundPerFoot));

            var normalized = utilization.Normalize();
            
            Assert.That(normalized, Is.EqualTo(1.5).Within(1e-6));
        }

        [Test]
        public void CostModelAtZero_Succeeds()
        {
            var utilization = new WallShearUtilization(new ForcePerLength(0, ForcePerLengthUnit.PoundPerFoot));

            var cost = utilization.GetNormalizedCost();
            
            Assert.That(cost, Is.EqualTo(0.5).Within(1e-6));
        }

        [Test]
        public void CostModelBetweenZeroAndOne_Succeeds()
        {
            var utilization = new WallShearUtilization(new ForcePerLength(750, ForcePerLengthUnit.PoundPerFoot));

            var cost = utilization.GetNormalizedCost();
            
            Assert.That(cost, Is.EqualTo(0.875).Within(1e-6));
        }

        [Test]
        public void CostModelAtOne_Succeeds()
        {
            var utilization = new WallShearUtilization(new ForcePerLength(1000, ForcePerLengthUnit.PoundPerFoot));

            var cost = utilization.GetNormalizedCost();
            
            Assert.That(cost, Is.EqualTo(1.0).Within(1e-6));
        }

        [Test]
        public void CostBetweenOneAndMax_Succeeds()
        {
            var utilization = new WallShearUtilization(new ForcePerLength(1500, ForcePerLengthUnit.PoundPerFoot));

            var cost = utilization.GetNormalizedCost();
            
            Assert.That(cost, Is.EqualTo(1.5).Within(1e-6));
        }

        [Test]
        public void CostAtMax_Succeeds()
        {
            var utilization = new WallShearUtilization(new ForcePerLength(2000, ForcePerLengthUnit.PoundPerFoot));

            var cost = utilization.GetNormalizedCost();
            
            Assert.That(cost, Is.EqualTo(3.0).Within(1e-6));
        }
    }
}