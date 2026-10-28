using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K1CorrectionFactorTests
{
    private readonly K1CorrectionFactor _factor = new();

    /// <summary>Verifies K1 returns documented values for free-air installations.</summary>
    [Theory]
    [InlineData(10, InsulationType.Pvc, 1.22)]
    [InlineData(40, InsulationType.PrEpr, 0.91)]
    public void Evaluate_FreeAirWithTabulatedTemperature_ReturnsAppliedValue(
        double temperature, InsulationType insulation, double expected)
    {
        var context = new InstallationContext(
            AmbientAirTemperatureCelsius: temperature,
            InsulationType: insulation,
            Environment: InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(expected);
        result.Source.Should().Be("instruction-metier.md §6.1");
    }

    /// <summary>Verifies K1 is not applicable to buried installations.</summary>
    [Fact]
    public void Evaluate_BuriedInstallation_ReturnsNotApplicable()
    {
        var result = _factor.Evaluate(new InstallationContext(Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
    }

    /// <summary>Verifies K1 is missing when the installation environment is unknown.</summary>
    [Fact]
    public void Evaluate_EnvironmentMissing_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext());

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>Verifies K1 is missing when the applicable ambient temperature is absent.</summary>
    [Fact]
    public void Evaluate_FreeAirTemperatureMissing_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext(
            InsulationType: InsulationType.Pvc,
            Environment: InstallationEnvironment.FreeAir));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>Verifies K1 is missing when the applicable insulation type is absent.</summary>
    [Fact]
    public void Evaluate_FreeAirInsulationMissing_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext(
            AmbientAirTemperatureCelsius: 20,
            Environment: InstallationEnvironment.FreeAir));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>Verifies K1 returns OutOfRange for an ambient temperature absent from the table.</summary>
    [Fact]
    public void Evaluate_TemperatureNotInTable_ReturnsOutOfRange()
    {
        var context = new InstallationContext(
            AmbientAirTemperatureCelsius: 12,
            InsulationType: InsulationType.Pvc,
            Environment: InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }
}