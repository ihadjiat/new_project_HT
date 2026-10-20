using FluentAssertions;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K5CorrectionFactorTests
{
    private readonly K5CorrectionFactor _factor = new();

    [Theory]
    [InlineData(1, 1.00)]
    [InlineData(2, 0.88)]
    [InlineData(3, 0.82)]
    [InlineData(4, 0.77)]
    [InlineData(5, 0.75)]
    [InlineData(6, 0.73)]
    [InlineData(7, 0.73)]
    [InlineData(8, 0.72)]
    [InlineData(9, 0.72)]
    public void Calculate_TouchingCables_MatchesReferenceTable(int count, double expected)
    {
        var context = new InstallationContext(TouchingCablesOnPerforatedTrayCount: count);

        var result = _factor.Calculate(context);

        result.Should().BeApproximately(expected, 0.001);
    }

    [Fact]
    public void Calculate_NonTouchingCablesWithEnoughClearance_ReturnsOne()
    {
        var context = new InstallationContext(
            CableClearanceMeters: 0.1,
            LargestCableDiameterMeters: 0.05,
            VerticalCableClearanceMeters: 0.30);

        var result = _factor.Calculate(context);

        result.Should().Be(1.0);
    }

    [Fact]
    public void Calculate_NonTouchingHorizontallyButInsufficientVerticalClearance_FallsBackToTable()
    {
        var context = new InstallationContext(
            CableClearanceMeters: 0.1,
            LargestCableDiameterMeters: 0.05,
            VerticalCableClearanceMeters: 0.10, // < 30 cm
            TouchingCablesOnPerforatedTrayCount: 3);

        var result = _factor.Calculate(context);

        result.Should().Be(0.82);
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
        var context = new InstallationContext(TouchingCablesOnPerforatedTrayCount: count);

        var act = () => _factor.Calculate(context);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}