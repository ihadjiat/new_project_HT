using FluentAssertions;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public sealed class K17CorrectionFactorTests
{
    [Theory]
    [InlineData(2, 0.25, 0.74)]
    [InlineData(3, 0.5, 0.73)]
    [InlineData(6, 1.0, 0.73)]
    public void Calculate_ReturnsFactorForTabulatedCombination(int cableCount, double spacing, double expected)
    {
        var factor = new K17CorrectionFactor();
        var context = new InstallationContext(
            CablesInConduitCount: cableCount,
            CableSpacingInConduitMeters: spacing);

        factor.Calculate(context).Should().BeApproximately(expected, 0.0001);
    }

    [Fact]
    public void Calculate_ReturnsOneWhenConduitDataIsMissing()
    {
        new K17CorrectionFactor()
            .Calculate(new InstallationContext())
            .Should()
            .BeApproximately(1.0, 0.0001);
    }

    [Fact]
    public void Calculate_ThrowsArgumentExceptionForMissingCombination()
    {
        var context = new InstallationContext(
            CablesInConduitCount: 2,
            CableSpacingInConduitMeters: 0.75);

        Action act = () => new K17CorrectionFactor().Calculate(context);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*combination*is not defined in the k17 table.*");
    }
}
