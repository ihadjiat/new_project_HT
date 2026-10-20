using FluentAssertions;
using HTA.CableSizing;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests;

public sealed class AllowableCurrentCalculatorTests
{
    [Fact]
    public void CalculateGlobalCorrectionFactor_MultipliesAllRegisteredFactors()
    {
        var calculator = new AllowableCurrentCalculator(correctionFactors:
        [
            new FakeCorrectionFactor("factor-a", 0.5),
            new FakeCorrectionFactor("factor-b", 0.8)
        ]);

        calculator.CalculateGlobalCorrectionFactor(new InstallationContext())
            .Should()
            .BeApproximately(0.4, 0.0001);
    }

    [Fact]
    public void CalculateGlobalCorrectionFactor_UsesK1ThroughK17AndSymmetryByDefault()
    {
        var calculator = new AllowableCurrentCalculator();
        var context = new InstallationContext(
            CablesInConduitCount: 2,
            CableSpacingInConduitMeters: 0.25);

        calculator.CalculateGlobalCorrectionFactor(context)
            .Should()
            .BeApproximately(0.74, 0.0001);
    }

    [Fact]
    public void CalculateAllowableCurrent_MultipliesBaseCurrentByGlobalFactor()
    {
        var calculator = new AllowableCurrentCalculator(correctionFactors:
        [
            new FakeCorrectionFactor("test-factor", 0.5)
        ]);

        var current = calculator.CalculateAllowableCurrent(
            50,
            InstallationMethod.DirectBuried,
            CableType.Copper,
            new InstallationContext());

        current.Should().BeApproximately(50, 0.0001);
    }

    [Fact]
    public void CalculateMinimumCrossSection_ReturnsSmallestSufficientCrossSection()
    {
        var calculator = new AllowableCurrentCalculator();

        var crossSection = calculator.CalculateMinimumCrossSection(
            151,
            InstallationMethod.DirectBuried,
            CableType.Copper,
            new InstallationContext());

        crossSection.Should().Be(95);
    }

    private sealed class FakeCorrectionFactor(string name, double value) : ICorrectionFactor
    {
        public string Name => name;

        public double Calculate(InstallationContext context) => value;
    }
}
