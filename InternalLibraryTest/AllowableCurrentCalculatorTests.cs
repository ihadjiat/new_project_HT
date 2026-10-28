using FluentAssertions;
using HTA.CableSizing;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests;

public sealed class AllowableCurrentCalculatorTests
{
    private static readonly InstallationContext FreeAir = new(Environment: InstallationEnvironment.FreeAir);

    private static ReferenceCurrentSelectionInput Table52GInput() => new(
        CableConstruction.ThreeCoreCable,
        ElectricFieldType.Radial,
        InsulationType.PrEpr,
        ConductorMaterial.Copper,
        new AssignedVoltage(6, 10, 12));

    private static AllowableCurrentCalculator CalculatorWith(params IResultCorrectionFactor[] factors) => new(factors);

    [Fact]
    public void CalculateGlobalCorrectionFactor_MultipliesAppliedFactors()
    {
        var result = CalculatorWith(Fake("a", FactorStatus.Applied, 0.5), Fake("b", FactorStatus.Applied, 0.8))
            .CalculateGlobalCorrectionFactor(FreeAir);

        result.Status.Should().Be(CalculationStatus.Complete);
        result.GlobalCorrectionFactor.Should().BeApproximately(0.4, 0.0001);
        result.Factors.Should().HaveCount(2);
    }

    [Fact]
    public void CalculateAllowableCurrent_CompleteCase_UsesTable52GBaseCurrent()
    {
        var result = CalculatorWith(Fake("a", FactorStatus.Applied, 0.5), Fake("b", FactorStatus.Applied, 0.8))
            .CalculateAllowableCurrent(95, Table52GInput(), FreeAir);

        result.Status.Should().Be(CalculationStatus.Complete);
        result.BaseCurrentAmps.Should().Be(345);
        result.GlobalCorrectionFactor.Should().BeApproximately(0.4, 0.0001);
        result.CorrectedCurrentAmps.Should().BeApproximately(138, 0.0001);
        result.Value.Should().BeApproximately(138, 0.0001);
        result.ReferenceTable.Should().NotBeNullOrEmpty();
        result.Factors.Should().HaveCount(2);
    }

    [Fact]
    public void CalculateAllowableCurrent_BuriedUsesBuriedColumn()
    {
        var result = CalculatorWith(Fake("a", FactorStatus.Applied, 0.5))
            .CalculateAllowableCurrent(95, Table52GInput(), new InstallationContext(Environment: InstallationEnvironment.Buried));

        result.BaseCurrentAmps.Should().Be(331);
        result.CorrectedCurrentAmps.Should().BeApproximately(165.5, 0.0001);
    }

    [Fact]
    public void CalculateAllowableCurrent_MissingFactor_IsPartialAndNeverReplacedByOne()
    {
        var result = CalculatorWith(Fake("a", FactorStatus.Applied, 0.5), Fake("b", FactorStatus.Missing, null))
            .CalculateAllowableCurrent(95, Table52GInput(), FreeAir);

        result.Status.Should().Be(CalculationStatus.Partial);
        result.CorrectedCurrentAmps.Should().BeApproximately(172.5, 0.0001);
        result.Factors.Single(f => f.Name == "b").Value.Should().BeNull();
        result.Warnings.Should().Contain(w => w.StartsWith("b is missing"));
    }

    [Fact]
    public void CalculateAllowableCurrent_OutOfRangeFactor_IsPartialWithWarning()
    {
        var result = CalculatorWith(Fake("a", FactorStatus.Applied, 0.5), Fake("b", FactorStatus.OutOfRange, null))
            .CalculateAllowableCurrent(95, Table52GInput(), FreeAir);

        result.Status.Should().Be(CalculationStatus.Partial);
        result.Factors.Single(f => f.Name == "b").Value.Should().BeNull();
        result.Warnings.Should().Contain(w => w.StartsWith("b is out of range"));
    }

    [Fact]
    public void CalculateGlobalCorrectionFactor_OnlyMissingFactor_IsImpossible()
    {
        var result = CalculatorWith(Fake("a", FactorStatus.Missing, null)).CalculateGlobalCorrectionFactor(FreeAir);

        result.Status.Should().Be(CalculationStatus.Impossible);
        result.Value.Should().BeNull();
    }

    [Fact]
    public void CalculateAllowableCurrent_NotApplicableFactor_MultipliesByOne()
    {
        var result = CalculatorWith(Fake("a", FactorStatus.NotApplicable, 0.3))
            .CalculateAllowableCurrent(95, Table52GInput(), FreeAir);

        result.Status.Should().Be(CalculationStatus.Complete);
        result.GlobalCorrectionFactor.Should().Be(1.0);
        result.CorrectedCurrentAmps.Should().Be(345);
    }

    [Fact]
    public void CalculateAllowableCurrent_UnselectableTable_IsImpossible()
    {
        var input = new ReferenceCurrentSelectionInput(
            CableConstruction.ThreeCoreCable,
            ElectricFieldType.NonRadial,
            InsulationType.Pvc,
            ConductorMaterial.Copper,
            new AssignedVoltage(6, 10, 12));

        var result = CalculatorWith().CalculateAllowableCurrent(95, input, FreeAir);

        result.Status.Should().Be(CalculationStatus.Impossible);
        result.Warnings.Should().NotBeEmpty();
    }

    [Fact]
    public void CalculateAllowableCurrent_MissingEnvironment_IsImpossible()
    {
        CalculatorWith().CalculateAllowableCurrent(95, Table52GInput(), new InstallationContext())
            .Status.Should().Be(CalculationStatus.Impossible);
    }

    [Fact]
    public void CalculateAllowableCurrent_CrossSectionAbsentFromTable_IsImpossible()
    {
        var result = CalculatorWith().CalculateAllowableCurrent(100, Table52GInput(), FreeAir);

        result.Status.Should().Be(CalculationStatus.Impossible);
        result.BaseCurrentAmps.Should().BeNull();
        result.Warnings.Should().Contain(w => w.Contains("not present"));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(0)]
    [InlineData(-5)]
    public void CalculateMinimumCrossSection_InvalidCurrent_IsImpossible(double current)
    {
        CalculatorWith().CalculateMinimumCrossSection(current, Table52GInput(), FreeAir)
            .Status.Should().Be(CalculationStatus.Impossible);
    }

    [Fact]
    public void CalculateMinimumCrossSection_ReturnsSmallestSufficientSectionAndNeighbors()
    {
        var result = CalculatorWith(Fake("a", FactorStatus.Applied, 0.5))
            .CalculateMinimumCrossSection(151, Table52GInput(), FreeAir);

        result.Status.Should().Be(CalculationStatus.Complete);
        result.Value.Should().Be(95);
        result.SelectedCrossSectionMm2.Should().Be(95);
        result.CorrectedCurrentAmps.Should().BeApproximately(172.5, 0.0001);
        result.Warnings.Should().Contain(w => w.Contains("70 mm² (141 A)") && w.Contains("120 mm² (200 A)"));
    }

    [Fact]
    public void CalculateMinimumCrossSection_NoSufficientSection_IsImpossible()
    {
        var result = CalculatorWith(Fake("a", FactorStatus.Applied, 0.5))
            .CalculateMinimumCrossSection(1000, Table52GInput(), FreeAir);

        result.Status.Should().Be(CalculationStatus.Impossible);
        result.Value.Should().BeNull();
        result.Warnings.Should().Contain(w => w.Contains("300 mm²"));
    }

    [Fact]
    public void CalculateMinimumCrossSection_PartialFactors_ReturnsPartial()
    {
        var result = CalculatorWith(Fake("a", FactorStatus.Missing, null))
            .CalculateMinimumCrossSection(300, Table52GInput(), FreeAir);

        result.Status.Should().Be(CalculationStatus.Partial);
        result.Value.Should().Be(95);
    }

    [Fact]
    public void NullInputs_ReturnImpossibleWithoutThrowing()
    {
        var calculator = CalculatorWith();

        calculator.CalculateGlobalCorrectionFactor(null).Status.Should().Be(CalculationStatus.Impossible);
        calculator.CalculateAllowableCurrent(95, null, FreeAir).Status.Should().Be(CalculationStatus.Impossible);
        calculator.CalculateAllowableCurrent(95, Table52GInput(), null).Status.Should().Be(CalculationStatus.Impossible);
        calculator.CalculateMinimumCrossSection(100, null, FreeAir).Status.Should().Be(CalculationStatus.Impossible);
        calculator.CalculateMinimumCrossSection(100, Table52GInput(), null).Status.Should().Be(CalculationStatus.Impossible);
    }

    [Fact]
    public void CalculateGlobalCorrectionFactor_K7WithSingleSupportFactor_MultipliesBoth()
    {
        var result = CalculatorWith(
                Fake("k3", FactorStatus.Missing, null),
                Fake("k5", FactorStatus.Applied, 0.75),
                Fake("k7", FactorStatus.Applied, 0.73))
            .CalculateGlobalCorrectionFactor(FreeAir);

        result.Status.Should().Be(CalculationStatus.Complete);
        result.GlobalCorrectionFactor.Should().BeApproximately(0.5475, 0.0001);
    }

    [Fact]
    public void CalculateGlobalCorrectionFactor_K7WithoutSupportFactor_IsPartial()
    {
        var result = CalculatorWith(Fake("k7", FactorStatus.Applied, 0.8)).CalculateGlobalCorrectionFactor(FreeAir);

        result.Status.Should().Be(CalculationStatus.Partial);
        result.Warnings.Should().Contain(w => w.Contains("K7"));
    }

    [Fact]
    public void CalculateGlobalCorrectionFactor_TwoSupportFactors_IsPartialAndAppliesNone()
    {
        var result = CalculatorWith(
                Fake("k3", FactorStatus.Applied, 0.9),
                Fake("k5", FactorStatus.Applied, 0.75),
                Fake("k7", FactorStatus.Applied, 0.8))
            .CalculateGlobalCorrectionFactor(FreeAir);

        result.Status.Should().Be(CalculationStatus.Partial);
        result.GlobalCorrectionFactor.Should().BeApproximately(0.8, 0.0001);
    }

    [Fact]
    public void CalculateGlobalCorrectionFactor_SymmetryFactor_AddsAssumptionWarning()
    {
        var result = CalculatorWith(Fake("fs", FactorStatus.Applied, 0.8)).CalculateGlobalCorrectionFactor(FreeAir);

        result.Status.Should().Be(CalculationStatus.Complete);
        result.Warnings.Should().Contain(w => w.Contains("fs"));
    }

    [Fact]
    public void CalculateAllowableCurrent_DirectSunlight_IsPartialBecauseK2IsMissing()
    {
        var context = new InstallationContext(Environment: InstallationEnvironment.FreeAir, IsExposedToDirectSunlight: true);

        var result = CalculatorWith(new K2CorrectionFactor()).CalculateAllowableCurrent(95, Table52GInput(), context);

        result.Status.Should().Be(CalculationStatus.Partial);
        result.Factors.Single().Status.Should().Be(FactorStatus.Missing);
        result.Factors.Single().Value.Should().BeNull();
    }

    [Fact]
    public void CalculateAllowableCurrent_DefaultFactors_EvaluatesAllFactorsAndKeepsMissingOnesPartial()
    {
        var result = new AllowableCurrentCalculator().CalculateAllowableCurrent(95, Table52GInput(), FreeAir);

        result.Status.Should().Be(CalculationStatus.Partial);
        result.Factors.Should().HaveCount(18);
        result.BaseCurrentAmps.Should().Be(345);
        result.CorrectedCurrentAmps.Should().BeApproximately(345 * result.GlobalCorrectionFactor!.Value, 0.0001);
    }

    private static FakeFactor Fake(string name, FactorStatus status, double? value) => new(name, status, value);

    private sealed class FakeFactor(string name, FactorStatus status, double? value) : IResultCorrectionFactor
    {
        public FactorResult Evaluate(InstallationContext context) =>
            new(name, status, value, $"Fake {name}.", "test");
    }
}
