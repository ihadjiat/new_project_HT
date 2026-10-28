using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K2CorrectionFactorTests
{
    private readonly K2CorrectionFactor _factor = new();

    /// <summary>Verifies K2 is missing when direct sunlight exposure is unknown.</summary>
    [Fact]
    public void Evaluate_FreeAirExposureMissing_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext(Environment: InstallationEnvironment.FreeAir));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>Verifies K2 is not applicable when a free-air installation is not exposed to direct sunlight.</summary>
    [Fact]
    public void Evaluate_FreeAirNotExposedToDirectSunlight_ReturnsNotApplicable()
    {
        var result = _factor.Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsExposedToDirectSunlight: false));

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
    }

    /// <summary>Verifies exposed K2 is missing when the business data does not document its value or formula.</summary>
    [Fact]
    public void Evaluate_FreeAirExposedToDirectSunlight_ReturnsMissingWithUndocumentedDataExplanation()
    {
        var result = _factor.Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsExposedToDirectSunlight: true));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Explanation.Should().Contain("not documented in instruction-metier.md §6.2");
        result.Explanation.Should().Contain("IEC 60287");
    }

    /// <summary>Verifies K2 is not applicable to buried installations.</summary>
    [Fact]
    public void Evaluate_BuriedInstallation_ReturnsNotApplicable()
    {
        var result = _factor.Evaluate(new InstallationContext(Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
    }

    /// <summary>Verifies K2 is missing when the installation environment is absent.</summary>
    [Fact]
    public void Evaluate_EnvironmentMissing_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext());

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>Verifies K2 returns OutOfRange for an unrecognized installation environment.</summary>
    [Fact]
    public void Evaluate_EnvironmentUnrecognized_ReturnsOutOfRange()
    {
        var result = _factor.Evaluate(new InstallationContext(Environment: (InstallationEnvironment)int.MaxValue));

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }

    /// <summary>Verifies K2 evaluation throws when the installation context is null.</summary>
    [Fact]
    public void Evaluate_NullContext_ThrowsArgumentNullException()
    {
        var act = () => _factor.Evaluate(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
