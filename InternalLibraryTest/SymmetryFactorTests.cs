using FluentAssertions;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests;

public sealed class SymmetryFactorTests
{
    /// <summary>
    /// Verifies that the symmetry factor is not applicable to a three-core cable.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsNotApplicableForThreeCoreCable()
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext(CableConstruction: CableConstruction.ThreeCoreCable));

        AssertResult(result, FactorStatus.NotApplicable, 1.0);
    }

    /// <summary>
    /// Verifies that the symmetry factor is not applicable to one single-core cable per phase.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsNotApplicableForOneCablePerPhase()
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext(
            CableConstruction: CableConstruction.ThreeSingleCoreCables,
            CablesPerPhase: 1));

        AssertResult(result, FactorStatus.NotApplicable, 1.0);
    }

    /// <summary>
    /// Verifies the documented values for symmetric single-core cable arrangements.
    /// </summary>
    [Theory]
    [InlineData(2, ArrangementType.Trefoil, 1.0)]
    [InlineData(3, ArrangementType.Flat, 0.8)]
    [InlineData(4, ArrangementType.Trefoil, 1.0)]
    public void Evaluate_ReturnsDocumentedValueForSymmetricalArrangement(
        int cablesPerPhase,
        ArrangementType arrangementType,
        double expectedValue)
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext(
            CablesPerPhase: cablesPerPhase,
            ArrangementType: arrangementType,
            CableConstruction: CableConstruction.ThreeSingleCoreCables,
            AreCablePhaseArrangementsSymmetrical: true));

        AssertResult(result, FactorStatus.Applied, expectedValue);
    }

    /// <summary>
    /// Verifies that a non-symmetrical arrangement uses the documented factor of 0.8.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsPointEightWhenSymmetryConditionsAreNotMet()
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext(
            CablesPerPhase: 2,
            CableConstruction: CableConstruction.ThreeSingleCoreCables,
            AreCablePhaseArrangementsSymmetrical: false));

        AssertResult(result, FactorStatus.Applied, 0.8);
    }

    /// <summary>
    /// Verifies that missing cable construction produces a missing result.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsMissingWhenCableConstructionIsAbsent()
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext());

        AssertResult(result, FactorStatus.Missing, null);
    }

    /// <summary>
    /// Verifies that a missing cable count produces a missing result.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsMissingWhenCableCountIsAbsent()
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext(
            CableConstruction: CableConstruction.ThreeSingleCoreCables));

        AssertResult(result, FactorStatus.Missing, null);
    }

    /// <summary>
    /// Verifies that missing symmetry information produces a missing result.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsMissingWhenSymmetryInformationIsAbsent()
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext(
            CablesPerPhase: 2,
            CableConstruction: CableConstruction.ThreeSingleCoreCables,
            ArrangementType: ArrangementType.Trefoil));

        AssertResult(result, FactorStatus.Missing, null);
    }

    /// <summary>
    /// Verifies that an undocumented arrangement with five to eight cables per phase is missing.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsMissingForUndocumentedArrangementWithFiveToEightCables()
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext(
            CablesPerPhase: 5,
            ArrangementType: ArrangementType.Flat,
            CableConstruction: CableConstruction.ThreeSingleCoreCables));

        AssertResult(result, FactorStatus.Missing, null);
    }

    /// <summary>
    /// Verifies that a symmetric unspecified arrangement produces a missing result.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsMissingForSymmetricUnspecifiedArrangement()
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext(
            CablesPerPhase: 2,
            CableConstruction: CableConstruction.ThreeSingleCoreCables,
            AreCablePhaseArrangementsSymmetrical: true));

        AssertResult(result, FactorStatus.Missing, null);
    }

    /// <summary>
    /// Verifies that trefoil arrangements with more than four cables per phase are out of range.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsOutOfRangeForMoreThanFourTrefoilCables()
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext(
            CablesPerPhase: 5,
            ArrangementType: ArrangementType.Trefoil,
            CableConstruction: CableConstruction.ThreeSingleCoreCables));

        AssertResult(result, FactorStatus.OutOfRange, null);
    }

    /// <summary>
    /// Verifies that more than eight cables per phase are out of range.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsOutOfRangeForMoreThanEightCablesPerPhase()
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext(
            CablesPerPhase: 9,
            CableConstruction: CableConstruction.ThreeSingleCoreCables));

        AssertResult(result, FactorStatus.OutOfRange, null);
    }

    /// <summary>
    /// Verifies that non-positive cable counts are out of range.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Evaluate_ReturnsOutOfRangeForNonPositiveCableCount(int cablesPerPhase)
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext(
            CablesPerPhase: cablesPerPhase,
            CableConstruction: CableConstruction.ThreeSingleCoreCables));

        AssertResult(result, FactorStatus.OutOfRange, null);
    }

    /// <summary>
    /// Verifies that an unknown cable construction value is out of range.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsOutOfRangeForUnknownCableConstruction()
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext(CableConstruction: (CableConstruction)999));

        AssertResult(result, FactorStatus.OutOfRange, null);
    }

    /// <summary>
    /// Verifies that an unknown arrangement value is out of range.
    /// </summary>
    [Fact]
    public void Evaluate_ReturnsOutOfRangeForUnknownArrangement()
    {
        var result = new SymmetryFactor().Evaluate(new InstallationContext(
            CablesPerPhase: 2,
            ArrangementType: (ArrangementType)999,
            CableConstruction: CableConstruction.ThreeSingleCoreCables));

        AssertResult(result, FactorStatus.OutOfRange, null);
    }

    /// <summary>
    /// Verifies that evaluating a null context throws an argument null exception.
    /// </summary>
    [Fact]
    public void Evaluate_ThrowsArgumentNullExceptionForNullContext()
    {
        Action act = () => new SymmetryFactor().Evaluate(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    private static void AssertResult(FactorResult result, FactorStatus status, double? value)
    {
        result.Status.Should().Be(status);
        result.Value.Should().Be(value);
        result.Explanation.Should().NotBeNullOrWhiteSpace();
        result.Source.Should().Be("instruction-metier.md §7.2–7.3");
    }
}
