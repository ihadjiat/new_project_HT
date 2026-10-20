using FluentAssertions;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K8CorrectionFactorTests
{
    private readonly K8CorrectionFactor _factor = new();

    [Theory]
    [InlineData(1, 1, 0.96)]
    [InlineData(3, 4, 0.76)]
    [InlineData(6, 6, 0.68)]
    [InlineData(4, 5, 0.72)]
    public void Calculate_ValidCombination_ReturnsExpectedValue(int layers, int conduits, double expected)
    {
        var context = new InstallationContext(
            TouchingConduitLayerCountInFreeAir: layers,
            TouchingConduitsPerLayerInFreeAir: conduits);

        var result = _factor.Calculate(context);

        result.Should().Be(expected);
    }

    [Fact]
    public void Calculate_NoDataProvided_ReturnsNeutralFactor()
    {
        var context = new InstallationContext();

        var result = _factor.Calculate(context);

        result.Should().Be(1.0);
    }

    [Fact]
    public void Calculate_OutOfTableRange_ThrowsArgumentException()
    {
        var context = new InstallationContext(
            TouchingConduitLayerCountInFreeAir: 10,
            TouchingConduitsPerLayerInFreeAir: 10);

        var act = () => _factor.Calculate(context);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}