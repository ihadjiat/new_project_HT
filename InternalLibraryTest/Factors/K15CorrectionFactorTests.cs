using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public sealed class K15CorrectionFactorTests
{
    /// <summary>
    /// Returns exact factors for the first and last circuit-count rows in the K15 table.
    /// </summary>
    [Theory]
    [InlineData(2, 0.25, 0.84)]
    [InlineData(12, 0.25, 0.52)]
    public void Evaluate_BuriedFirstOrLastTableRow_ReturnsAppliedFactor(int count, double spacing, double expected)
    {
        var result = new K15CorrectionFactor().Evaluate(new InstallationContext(
            BuriedCircuitOrCableCount: count,
            BuriedCircuitSpacingMeters: spacing,
            Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(expected);
    }

    /// <summary>
    /// Returns the tabulated factor for touching buried circuits with zero spacing.
    /// </summary>
    [Fact]
    public void Evaluate_BuriedTouchingCircuits_ReturnsAppliedFactor()
    {
        var result = new K15CorrectionFactor().Evaluate(new InstallationContext(
            BuriedCircuitOrCableCount: 2,
            BuriedCircuitSpacingMeters: 0.0,
            Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(0.76);
    }

    /// <summary>
    /// Reports out-of-range when the supplied buried combination is not tabulated.
    /// </summary>
    [Fact]
    public void Evaluate_BuriedNonTabulatedCell_ReturnsOutOfRange()
    {
        var result = new K15CorrectionFactor().Evaluate(new InstallationContext(
            BuriedCircuitOrCableCount: 8,
            BuriedCircuitSpacingMeters: 1.0,
            Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
        result.Explanation.Should().Contain("not tabulated");
    }

    /// <summary>
    /// Reports missing when both required buried K15 inputs are absent.
    /// </summary>
    [Fact]
    public void Evaluate_BuriedWithoutInputs_ReturnsMissing()
    {
        var result = new K15CorrectionFactor().Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Explanation.Should().Contain("missing");
    }

    /// <summary>
    /// Reports missing when either required buried K15 input is absent.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Evaluate_BuriedWithOneMissingInput_ReturnsMissing(bool includeCount)
    {
        var context = includeCount
            ? new InstallationContext(BuriedCircuitOrCableCount: 2, Environment: InstallationEnvironment.Buried)
            : new InstallationContext(BuriedCircuitSpacingMeters: 0.25, Environment: InstallationEnvironment.Buried);

        var result = new K15CorrectionFactor().Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Explanation.Should().Contain("missing");
    }

    /// <summary>
    /// Reports missing when the installation environment is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingEnvironment_ReturnsMissing()
    {
        var result = new K15CorrectionFactor().Evaluate(new InstallationContext(
            BuriedCircuitOrCableCount: 2,
            BuriedCircuitSpacingMeters: 0.25));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Reports that K15 does not apply to a free-air installation.
    /// </summary>
    [Fact]
    public void Evaluate_FreeAir_ReturnsNotApplicable()
    {
        var result = new K15CorrectionFactor().Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.FreeAir));

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
    }

    /// <summary>
    /// Reports out-of-range when a buried spacing is outside the tabulated spacings.
    /// </summary>
    [Fact]
    public void Evaluate_BuriedUnsupportedSpacing_ReturnsOutOfRange()
    {
        var result = new K15CorrectionFactor().Evaluate(new InstallationContext(
            BuriedCircuitOrCableCount: 2,
            BuriedCircuitSpacingMeters: 0.75,
            Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
        result.Explanation.Should().Contain("not tabulated");
    }
}
