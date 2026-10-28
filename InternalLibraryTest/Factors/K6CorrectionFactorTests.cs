using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K6CorrectionFactorTests
{
    private readonly K6CorrectionFactor _factor = new();

    /// <summary>
    /// Evaluates K6 using the tabulated values for touching cables on a ladder, cleat, or mesh.
    /// </summary>
    [Theory]
    [InlineData(2, 0.88)]
    [InlineData(5, 0.80)]
    [InlineData(9, 0.78)]
    public void Evaluate_FreeAirWithTouchingCables_ReturnsTabulatedAppliedValue(int count, double expected)
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            TouchingCablesOnLadderOrCleatOrMeshCount: count);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().BeApproximately(expected, 0.001);
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §6.6");
    }

    /// <summary>
    /// Evaluates K6 as one when all documented non-touching clearance conditions are met.
    /// </summary>
    [Fact]
    public void Evaluate_FreeAirWithSufficientClearance_ReturnsOne()
    {
        var context = new InstallationContext(
            CableClearanceMeters: 0.1,
            LargestCableDiameterMeters: 0.05,
            VerticalCableClearanceMeters: 0.30,
            Environment: InstallationEnvironment.FreeAir,
            TouchingCablesOnLadderOrCleatOrMeshCount: 3);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(1.0);
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §6.6");
    }

    /// <summary>
    /// Evaluates K6 as not applicable for a buried installation.
    /// </summary>
    [Fact]
    public void Evaluate_Buried_ReturnsNotApplicable()
    {
        var context = new InstallationContext(Environment: InstallationEnvironment.Buried);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §6.6");
    }

    /// <summary>
    /// Evaluates K6 as missing when the installation environment is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingEnvironment_ReturnsMissing()
    {
        var context = new InstallationContext(TouchingCablesOnLadderOrCleatOrMeshCount: 3);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §6.6");
    }

    /// <summary>
    /// Evaluates K6 as missing when the ladder, cleat, or mesh touching count is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingTouchingCount_ReturnsMissing()
    {
        var context = new InstallationContext(Environment: InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §6.6");
    }

    /// <summary>
    /// Evaluates K6 as missing when only cable clearance measurements are provided.
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
        result.Source.Should().Be("instruction-metier.md §6.6");
    }

    /// <summary>
    /// Evaluates K6 as out of range when the touching count is outside the documented table.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void Evaluate_TouchingCountOutOfRange_ReturnsOutOfRange(int count)
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            TouchingCablesOnLadderOrCleatOrMeshCount: count);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §6.6");
    }
}