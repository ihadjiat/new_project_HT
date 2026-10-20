using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K1CorrectionFactorTests
{
    private readonly K1CorrectionFactor _factor = new();

    [Theory]
    [InlineData(10, InsulationType.Pvc, 1.22)]
    [InlineData(15, InsulationType.Pvc, 1.17)]
    [InlineData(20, InsulationType.Pvc, 1.12)]
    [InlineData(25, InsulationType.Pvc, 1.06)]
    [InlineData(30, InsulationType.Pvc, 1.00)]
    [InlineData(35, InsulationType.Pvc, 0.94)]
    [InlineData(40, InsulationType.Pvc, 0.87)]
    [InlineData(45, InsulationType.Pvc, 0.79)]
    [InlineData(50, InsulationType.Pvc, 0.71)]
    [InlineData(55, InsulationType.Pvc, 0.61)]
    public void Calculate_Pvc_MatchesReferenceTable(double temperature, InsulationType insulation, double expected)
    {
        var context = new InstallationContext(
            AmbientAirTemperatureCelsius: temperature,
            InsulationType: insulation);

        var result = _factor.Calculate(context);

        result.Should().BeApproximately(expected, 0.01);
    }

    [Theory]
    [InlineData(10, InsulationType.PrEpr, 1.15)]
    [InlineData(20, InsulationType.PrEpr, 1.08)]
    [InlineData(30, InsulationType.PrEpr, 1.00)]
    [InlineData(40, InsulationType.PrEpr, 0.91)]
    [InlineData(55, InsulationType.PrEpr, 0.76)]
    public void Calculate_PrEpr_MatchesReferenceTable(double temperature, InsulationType insulation, double expected)
    {
        var context = new InstallationContext(
            AmbientAirTemperatureCelsius: temperature,
            InsulationType: insulation);

        var result = _factor.Calculate(context);

        result.Should().BeApproximately(expected, 0.01);
    }

    [Fact]
    public void Calculate_NoDataProvided_ReturnsNeutralFactor()
    {
        var context = new InstallationContext();

        var result = _factor.Calculate(context);

        result.Should().Be(1.0);
    }

    [Fact]
    public void Calculate_TemperatureAboveMaxCoreTemperature_Throws()
    {
        var context = new InstallationContext(
            AmbientAirTemperatureCelsius: 95,
            InsulationType: InsulationType.Pvc);

        var act = () => _factor.Calculate(context);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}