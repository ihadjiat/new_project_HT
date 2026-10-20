using FluentAssertions;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests;

public sealed class SymmetryFactorTests
{
    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Calculate_ReturnsTemporaryNeutralValueForTwoThreeOrFourTrefoilCables(int cableCount)
    {
        var factor = new SymmetryFactor();
        var context = new InstallationContext(CablesPerPhase: cableCount, ArrangementType: ArrangementType.Trefoil);

        factor.Calculate(context).Should().BeApproximately(1.0, 0.0001);
    }

    [Fact]
    public void Calculate_ReturnsOneForASingleCable()
    {
        var factor = new SymmetryFactor();
        var context = new InstallationContext(CablesPerPhase: 1, ArrangementType: ArrangementType.Trefoil);

        factor.Calculate(context).Should().BeApproximately(1.0, 0.0001);
    }

    [Fact]
    public void Calculate_ThrowsArgumentOutOfRangeExceptionForMoreThanFourTrefoilCables()
    {
        var factor = new SymmetryFactor();
        var context = new InstallationContext(CablesPerPhase: 5, ArrangementType: ArrangementType.Trefoil);

        Action act = () => factor.Calculate(context);

        act.Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Trefoil arrangement is not supported*");
    }
}
