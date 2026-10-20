using FluentAssertions;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K3CorrectionFactorTests
{
    private readonly K3CorrectionFactor _factor = new();

    [Theory]
    [InlineData(1, 1.00)]
    [InlineData(2, 0.85)]
    [InlineData(3, 0.79)]
    [InlineData(4, 0.75)]
    [InlineData(5, 0.73)]
    [InlineData(6, 0.72)]
    [InlineData(7, 0.72)]
    [InlineData(8, 0.71)]
    [InlineData(9, 0.70)]
    public void Calculate_TouchingCables_MatchesReferenceTable(int count, double expected)
    {
        var context = new InstallationContext(TouchingCablesOnUnperforatedSurfaceCount: count);

        var result = _factor.Calculate(context);

        result.Should().BeApproximately(expected, 0.001);
    }

    [Fact]
    public void Calculate_NonTouchingCablesWithEnoughClearance_ReturnsOne()
    {
        var context = new InstallationContext(
            CableClearanceMeters: 0.1,
            LargestCableDiameterMeters: 0.05); // e = 2d exactly

        var result = _factor.Calculate(context);

        result.Should().Be(1.0);
    }

    [Fact]
    public void Calculate_NoDataProvided_ReturnsNeutralFactor()
    {
        var context = new InstallationContext();

        var result = _factor.Calculate(context);

        result.Should().Be(1.0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void Calculate_CountOutOfRange_Throws(int count)
    {
        var context = new InstallationContext(TouchingCablesOnUnperforatedSurfaceCount: count);

        var act = () => _factor.Calculate(context);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}