using FluentAssertions;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K7CorrectionFactorTests
{
    private readonly K7CorrectionFactor _factor = new();

    [Theory]
    [InlineData(1, 1.00)]
    [InlineData(2, 0.80)]
    [InlineData(3, 0.73)]
    [InlineData(4, 0.70)]
    public void Calculate_LayerCount_MatchesReferenceTable(int layerCount, double expected)
    {
        var context = new InstallationContext(TouchingCableLayerCountInFreeAir: layerCount);

        var result = _factor.Calculate(context);

        result.Should().BeApproximately(expected, 0.001);
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
    [InlineData(5)]
    public void Calculate_LayerCountOutOfRange_Throws(int layerCount)
    {
        var context = new InstallationContext(TouchingCableLayerCountInFreeAir: layerCount);

        var act = () => _factor.Calculate(context);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}