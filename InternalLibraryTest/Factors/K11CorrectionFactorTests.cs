using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K11CorrectionFactorTests
{
    private readonly K11CorrectionFactor _factor = new();

    /// <summary>
    /// Verifies that K11 returns documented values for tabulated single-layer drum turns.
    /// </summary>
    [Theory]
    [InlineData(2, 0.88)]
    [InlineData(6, 0.73)]
    [InlineData(14, 0.65)]
    public void Evaluate_TabulatedSingleLayerTurns_ReturnsAppliedResult(int turns, double expected)
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsCableOnMobileEquipmentDrum: true,
            DrumWindingType: DrumWindingType.SingleLayer,
            DrumTurnsCount: turns);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(expected);
        result.Source.Should().Be("instruction-metier.md §6.11");
    }

    /// <summary>
    /// Verifies that K11 returns the documented values for multiple-layer drum categories, including more than three layers.
    /// </summary>
    [Theory]
    [InlineData(1, 0.80)]
    [InlineData(2, 0.60)]
    [InlineData(3, 0.50)]
    [InlineData(4, 0.35)]
    [InlineData(7, 0.35)]
    public void Evaluate_MultipleLayerCategory_ReturnsAppliedResult(int layers, double expected)
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsCableOnMobileEquipmentDrum: true,
            DrumWindingType: DrumWindingType.MultipleLayers,
            DrumCompletedLayerCount: layers);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(expected);
        result.Source.Should().Be("instruction-metier.md §6.11");
    }

    /// <summary>
    /// Verifies that K11 is not applicable to buried installations.
    /// </summary>
    [Fact]
    public void Evaluate_BuriedInstallation_ReturnsNotApplicableResult()
    {
        var context = new InstallationContext(Environment: InstallationEnvironment.Buried);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
    }

    /// <summary>
    /// Verifies that K11 is not applicable when the cable is not on a mobile-equipment drum.
    /// </summary>
    [Fact]
    public void Evaluate_CableNotOnMobileEquipmentDrum_ReturnsNotApplicableResult()
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsCableOnMobileEquipmentDrum: false);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
    }

    /// <summary>
    /// Verifies that K11 reports missing when the installation environment is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingEnvironment_ReturnsMissingResult()
    {
        var result = _factor.Evaluate(new InstallationContext());

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K11 reports missing when the mobile-equipment drum indicator is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingDrumIndicator_ReturnsMissingResult()
    {
        var context = new InstallationContext(Environment: InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K11 reports missing when the winding type is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingWindingType_ReturnsMissingResult()
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsCableOnMobileEquipmentDrum: true);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K11 reports missing when single-layer drum turns are absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingSingleLayerTurns_ReturnsMissingResult()
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsCableOnMobileEquipmentDrum: true,
            DrumWindingType: DrumWindingType.SingleLayer);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K11 reports missing when completed multiple-layer counts are absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingMultipleLayerCount_ReturnsMissingResult()
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsCableOnMobileEquipmentDrum: true,
            DrumWindingType: DrumWindingType.MultipleLayers);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K11 reports out of range for single-layer turns absent from the table.
    /// </summary>
    [Fact]
    public void Evaluate_UntabulatedSingleLayerTurns_ReturnsOutOfRangeResult()
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsCableOnMobileEquipmentDrum: true,
            DrumWindingType: DrumWindingType.SingleLayer,
            DrumTurnsCount: 7);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K11 rejects an undefined installation environment.
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
    /// Verifies that K11 rejects zero or negative completed multiple-layer counts.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Evaluate_NonPositiveMultipleLayerCount_ReturnsOutOfRangeResult(int layers)
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            IsCableOnMobileEquipmentDrum: true,
            DrumWindingType: DrumWindingType.MultipleLayers,
            DrumCompletedLayerCount: layers);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that evaluating K11 with a null context throws an argument-null exception.
    /// </summary>
    [Fact]
    public void Evaluate_NullContext_ThrowsArgumentNullException()
    {
        var act = () => _factor.Evaluate(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
