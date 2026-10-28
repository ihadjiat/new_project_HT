using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public sealed class K17CorrectionFactorTests
{
    /// <summary>
    /// Evaluates documented buried cable-count and spacing combinations.
    /// </summary>
    [Theory]
    [InlineData(2, 0.25, 0.74)]
    [InlineData(6, 1.0, 0.73)]
    public void Evaluate_BuriedTabulatedCombination_ReturnsAppliedFactor(int cableCount, double spacing, double expected)
    {
        var result = new K17CorrectionFactor().Evaluate(new InstallationContext(
            CablesInConduitCount: cableCount,
            CableSpacingInConduitMeters: spacing,
            Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(expected);
    }

    /// <summary>
    /// Reports that K17 does not apply to a free-air installation.
    /// </summary>
    [Fact]
    public void Evaluate_FreeAir_ReturnsNotApplicable()
    {
        var result = new K17CorrectionFactor().Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.FreeAir));

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
    }

    /// <summary>
    /// Reports missing status when the installation environment is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingEnvironment_ReturnsMissing()
    {
        var result = new K17CorrectionFactor().Evaluate(new InstallationContext(
            CablesInConduitCount: 2,
            CableSpacingInConduitMeters: 0.25));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Reports missing status when both required cable data values are absent.
    /// </summary>
    [Fact]
    public void Evaluate_BuriedWithoutCableData_ReturnsMissing()
    {
        var result = new K17CorrectionFactor().Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Reports missing status when only one required cable data value is supplied.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Evaluate_BuriedWithIncompleteCableData_ReturnsMissing(bool includeCableCount)
    {
        var context = includeCableCount
            ? new InstallationContext(CablesInConduitCount: 2, Environment: InstallationEnvironment.Buried)
            : new InstallationContext(CableSpacingInConduitMeters: 0.25, Environment: InstallationEnvironment.Buried);

        var result = new K17CorrectionFactor().Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Reports out-of-range status when the buried cable data does not match a table entry.
    /// </summary>
    [Fact]
    public void Evaluate_BuriedNonTabulatedCombination_ReturnsOutOfRange()
    {
        var result = new K17CorrectionFactor().Evaluate(new InstallationContext(
            CablesInConduitCount: 2,
            CableSpacingInConduitMeters: 0.75,
            Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }
}
