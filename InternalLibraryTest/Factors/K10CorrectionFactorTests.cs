using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K10CorrectionFactorTests
{
    private const string Source = "instruction-metier.md §6.10";
    private readonly K10CorrectionFactor _factor = new();

    /// <summary>
    /// Verifies that K10 uses p = 2a + b and returns documented values for exact occupation coefficients.
    /// </summary>
    [Theory]
    [InlineData(2, 1, 1, 5, 0.60)]
    [InlineData(3, 8, 2, 7, 0.66)]
    [InlineData(4, 2, 1, 10, 0.72)]
    [InlineData(9, 2, 1, 20, 0.85)]
    [InlineData(24, 2, 1, 50, 0.92)]
    public void Evaluate_ExactOccupationCoefficient_ReturnsAppliedResult(
        double dimensionA,
        double dimensionB,
        double sumCableCircuitSpaceRequirements,
        double occupationCoefficient,
        double expected)
    {
        var context = CreateApplicableContext(dimensionA, dimensionB, sumCableCircuitSpaceRequirements);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(expected);
        result.Source.Should().Be(Source);
        ((2 * dimensionA + dimensionB) / sumCableCircuitSpaceRequirements).Should().Be(occupationCoefficient);
    }

    /// <summary>
    /// Verifies that K10 is not applicable to buried installations.
    /// </summary>
    [Fact]
    public void Evaluate_BuriedInstallation_ReturnsNotApplicableResult()
    {
        var result = _factor.Evaluate(new InstallationContext(Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
        result.Source.Should().Be(Source);
    }

    /// <summary>
    /// Verifies that K10 is not applicable when the cable is not in the specified trench.
    /// </summary>
    [Fact]
    public void Evaluate_CableNotInSpecifiedTrench_ReturnsNotApplicableResult()
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsCableInClosedUnfilledTrenchFlushWithFloor: false);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
    }

    /// <summary>
    /// Verifies that K10 reports missing when the installation environment is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingEnvironment_ReturnsMissingResult()
    {
        var result = _factor.Evaluate(new InstallationContext());

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K10 reports missing when the trench installation indicator is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingTrenchIndicator_ReturnsMissingResult()
    {
        var result = _factor.Evaluate(new InstallationContext(Environment: InstallationEnvironment.FreeAir));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K10 reports missing when the cable construction is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingCableConstruction_ReturnsMissingResult()
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsCableInClosedUnfilledTrenchFlushWithFloor: true);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K10 does not calculate the ambiguous rule for three single-core cables.
    /// </summary>
    [Fact]
    public void Evaluate_ThreeSingleCoreCables_ReturnsMissingResult()
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsCableInClosedUnfilledTrenchFlushWithFloor: true,
            CableConstruction: CableConstruction.ThreeSingleCoreCables,
            CableTrenchDimensionAMeters: 2,
            CableTrenchDimensionBMeters: 1,
            SumCableCircuitEncombrementsMeters: 1);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K10 reports missing when any required measurement is absent.
    /// </summary>
    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public void Evaluate_MissingMeasurement_ReturnsMissingResult(
        bool missingDimensionA,
        bool missingDimensionB,
        bool missingSpaceRequirements)
    {
        var context = CreateApplicableContext(
            missingDimensionA ? null : 2,
            missingDimensionB ? null : 1,
            missingSpaceRequirements ? null : 1);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K10 reports out of range for an unrecognized installation environment.
    /// </summary>
    [Fact]
    public void Evaluate_UnknownEnvironment_ReturnsOutOfRangeResult()
    {
        var context = new InstallationContext(Environment: (InstallationEnvironment)999);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K10 reports out of range for an unrecognized cable construction.
    /// </summary>
    [Fact]
    public void Evaluate_UnknownCableConstruction_ReturnsOutOfRangeResult()
    {
        var context = CreateApplicableContext(cableConstruction: (CableConstruction)999);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K10 reports out of range for non-finite or non-positive measurements.
    /// </summary>
    [Theory]
    [InlineData(0, double.NaN)]
    [InlineData(1, double.PositiveInfinity)]
    [InlineData(2, 0)]
    [InlineData(2, -1)]
    public void Evaluate_InvalidMeasurement_ReturnsOutOfRangeResult(int measurementIndex, double invalidMeasurement)
    {
        var context = CreateApplicableContext(
            dimensionA: measurementIndex == 0 ? invalidMeasurement : 2,
            dimensionB: measurementIndex == 1 ? invalidMeasurement : 1,
            sumCableCircuitSpaceRequirements: measurementIndex == 2 ? invalidMeasurement : 1);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K10 reports out of range for an occupation coefficient not listed in the table.
    /// </summary>
    [Fact]
    public void Evaluate_UntabulatedOccupationCoefficient_ReturnsOutOfRangeResult()
    {
        var result = _factor.Evaluate(CreateApplicableContext(dimensionA: 1, dimensionB: 2, sumCableCircuitSpaceRequirements: 1));

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K10 throws when the installation context is null.
    /// </summary>
    [Fact]
    public void Evaluate_NullContext_ThrowsArgumentNullException()
    {
        var action = () => _factor.Evaluate(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    private static InstallationContext CreateApplicableContext(
        double? dimensionA = 2,
        double? dimensionB = 1,
        double? sumCableCircuitSpaceRequirements = 1,
        CableConstruction? cableConstruction = CableConstruction.ThreeCoreCable)
    {
        return new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsCableInClosedUnfilledTrenchFlushWithFloor: true,
            CableConstruction: cableConstruction,
            CableTrenchDimensionAMeters: dimensionA,
            CableTrenchDimensionBMeters: dimensionB,
            SumCableCircuitEncombrementsMeters: sumCableCircuitSpaceRequirements);
    }
}
