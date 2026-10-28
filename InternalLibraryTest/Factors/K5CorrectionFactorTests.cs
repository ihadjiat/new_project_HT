using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K5CorrectionFactorTests
{
    private readonly K5CorrectionFactor _factor = new();

    /// <summary>
    /// Verifies that Evaluate throws when the installation context is null.
    /// </summary>
    [Fact]
    public void Evaluate_NullContext_ThrowsArgumentNullException()
    {
        var act = () => _factor.Evaluate(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Evaluates K5 using the tabulated values for touching cables on a perforated tray.
    /// </summary>
    [Theory]
    [InlineData(2, 0.88)]
    [InlineData(5, 0.75)]
    [InlineData(9, 0.72)]
    public void Evaluate_FreeAirWithTouchingCables_ReturnsTabulatedAppliedValue(int count, double expected)
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            TouchingCablesOnPerforatedTrayCount: count);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().BeApproximately(expected, 0.001);
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §6.5");
    }

    /// <summary>
    /// Evaluates K5 as one when all documented non-touching clearance conditions are met.
    /// </summary>
    [Fact]
    public void Evaluate_FreeAirWithSufficientClearance_ReturnsOne()
    {
        var context = new InstallationContext(
            CableClearanceMeters: 0.1,
            LargestCableDiameterMeters: 0.05,
            VerticalCableClearanceMeters: 0.30,
            Environment: InstallationEnvironment.FreeAir,
            TouchingCablesOnPerforatedTrayCount: 3);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(1.0);
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §6.5");
    }

    /// <summary>
    /// Evaluates K5 as not applicable for a buried installation.
    /// </summary>
    [Fact]
    public void Evaluate_Buried_ReturnsNotApplicable()
    {
        var context = new InstallationContext(Environment: InstallationEnvironment.Buried);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §6.5");
    }

    /// <summary>
    /// Evaluates K5 as missing when the installation environment is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingEnvironment_ReturnsMissing()
    {
        var context = new InstallationContext(TouchingCablesOnPerforatedTrayCount: 3);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §6.5");
    }

    /// <summary>
    /// Evaluates K5 as missing when the perforated-tray touching count is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingTouchingCount_ReturnsMissing()
    {
        var context = new InstallationContext(Environment: InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §6.5");
    }

    /// <summary>
    /// Evaluates K5 as missing when only cable clearance measurements are provided.
    /// </summary>
    [Fact]
    public void Evaluate_OnlyClearanceMeasurementsProvided_ReturnsMissing()
    {
        var context = new InstallationContext(
            CableClearanceMeters: 0.1,
            LargestCableDiameterMeters: 0.05,
            VerticalCableClearanceMeters: 0.30,
            Environment: InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §6.5");
    }

    /// <summary>
    /// Evaluates K5 as out of range when the touching count is outside the documented table.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void Evaluate_TouchingCountOutOfRange_ReturnsOutOfRange(int count)
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            TouchingCablesOnPerforatedTrayCount: count);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §6.5");
    }
}