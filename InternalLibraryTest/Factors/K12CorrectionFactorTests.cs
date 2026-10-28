using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K12CorrectionFactorTests
{
    private readonly K12CorrectionFactor _factor = new();

    /// <summary>Verifies K12 returns documented values for buried installations.</summary>
    [Theory]
    [InlineData(0, InsulationType.Pvc, 1.18)]
    [InlineData(45, InsulationType.PrEpr, 0.80)]
    public void Evaluate_BuriedWithTabulatedTemperature_ReturnsAppliedValue(
        double temperature, InsulationType insulation, double expected)
    {
        var context = new InstallationContext(
            SoilTemperatureCelsius: temperature,
            InsulationType: insulation,
            Environment: InstallationEnvironment.Buried);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(expected);
        result.Source.Should().Be("instruction-metier.md §6.12");
    }

    /// <summary>Verifies K12 is not applicable to free-air installations.</summary>
    [Fact]
    public void Evaluate_FreeAirInstallation_ReturnsNotApplicable()
    {
        var result = _factor.Evaluate(new InstallationContext(Environment: InstallationEnvironment.FreeAir));

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
    }

    /// <summary>Verifies K12 is missing when the installation environment is unknown.</summary>
    [Fact]
    public void Evaluate_EnvironmentMissing_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext());

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>Verifies ambient air temperature does not substitute for missing soil temperature.</summary>
    [Fact]
    public void Evaluate_BuriedWithOnlyAmbientTemperature_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext(
            AmbientAirTemperatureCelsius: 20,
            InsulationType: InsulationType.Pvc,
            Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>Verifies K12 is missing when the applicable soil temperature is absent.</summary>
    [Fact]
    public void Evaluate_BuriedTemperatureMissing_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext(
            InsulationType: InsulationType.Pvc,
            Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>Verifies K12 is missing when the applicable insulation type is absent.</summary>
    [Fact]
    public void Evaluate_BuriedInsulationMissing_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext(
            SoilTemperatureCelsius: 20,
            Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>Verifies K12 returns OutOfRange for a soil temperature absent from the table.</summary>
    [Fact]
    public void Evaluate_TemperatureNotInTable_ReturnsOutOfRange()
    {
        var context = new InstallationContext(
            SoilTemperatureCelsius: 12,
            InsulationType: InsulationType.Pvc,
            Environment: InstallationEnvironment.Buried);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }
}
