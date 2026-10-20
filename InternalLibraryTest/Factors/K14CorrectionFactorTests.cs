using Xunit;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;


namespace HTA.CableSizing.Tests.Factors;

public class K14CorrectionFactorTests
{
    private readonly K14CorrectionFactor _factor = new();

    [Theory]
    [InlineData(0.50, 1.04)]
    [InlineData(0.60, 1.02)]
    [InlineData(0.80, 1.00)]
    [InlineData(1.00, 0.97)]
    [InlineData(1.25, 0.95)]
    [InlineData(1.50, 0.93)]
    [InlineData(1.75, 0.91)]
    [InlineData(2.00, 0.90)]
    public void Calculate_ExactTableValue_ReturnsExpectedCoefficient(double depth, double expected)
    {
        var context = new InstallationContext(LayingDepthMeters: depth);

        var result = _factor.Calculate(context);

        Assert.Equal(expected, result, precision: 3);
    }

    [Fact]
    public void Calculate_IntermediateValue_InterpolatesLinearly()
    {
        // Between 0.60 (1.02) and 0.80 (1.00) -> at 0.70m, expected = 1.01
        var context = new InstallationContext(LayingDepthMeters: 0.70);

        var result = _factor.Calculate(context);

        Assert.Equal(1.01, result, precision: 3);
    }

    [Fact]
    public void Calculate_DepthBelowMinimum_ReturnsMinimumCoefficient()
    {
        var context = new InstallationContext(LayingDepthMeters: 0.30);

        var result = _factor.Calculate(context);

        Assert.Equal(1.04, result);
    }

    [Fact]
    public void Calculate_DepthAboveMaximum_ReturnsMaximumCoefficient()
    {
        var context = new InstallationContext(LayingDepthMeters: 3.00);

        var result = _factor.Calculate(context);

        Assert.Equal(0.90, result);
    }

    [Fact]
    public void Calculate_NoDepthProvided_ReturnsNeutralFactor()
    {
        var context = new InstallationContext();

        var result = _factor.Calculate(context);

        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Calculate_ReferenceDepth_ReturnsOne()
    {
        // The base current tables (52G-52J) are valid for a reference laying depth of 0.80m,
        // which corresponds to a neutral correction factor of 1.00 [1].
        var context = new InstallationContext(LayingDepthMeters: 0.80);

        var result = _factor.Calculate(context);

        Assert.Equal(1.00, result);
    }
}