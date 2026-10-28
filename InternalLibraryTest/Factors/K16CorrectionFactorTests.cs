using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public sealed class K16CorrectionFactorTests
{
    /// <summary>
    /// Returns exact factors for buried-conduit combinations listed in the K16 table.
    /// </summary>
    [Theory]
    [InlineData(1, 1, 0.80)]
    [InlineData(2, 3, 0.50)]
    [InlineData(6, 6, 0.26)]
    public void Evaluate_BuriedTabulatedCombination_ReturnsAppliedFactor(int layers, int conduitsPerLayer, double expected)
    {
        var result = new K16CorrectionFactor().Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.Buried,
            BuriedConduitLayerCount: layers,
            BuriedConduitsPerLayer: conduitsPerLayer,
            CablesPerBuriedConduit: 1,
            BuriedConcreteThermalResistivityKMW: 1.0));

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(expected);
        result.Source.Should().Be("instruction-metier.md §6.16");
    }

    /// <summary>
    /// Reports that K16 does not apply to a free-air installation.
    /// </summary>
    [Fact]
    public void Evaluate_FreeAir_ReturnsNotApplicable()
    {
        var result = new K16CorrectionFactor().Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.FreeAir));

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
        result.Source.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Reports out-of-range status when the installation environment is unknown.
    /// </summary>
    [Fact]
    public void Evaluate_UnknownEnvironment_ReturnsOutOfRange()
    {
        var result = new K16CorrectionFactor().Evaluate(new InstallationContext(
            Environment: (InstallationEnvironment)999,
            BuriedConduitLayerCount: 1,
            BuriedConduitsPerLayer: 1,
            CablesPerBuriedConduit: 1,
            BuriedConcreteThermalResistivityKMW: 1.0));

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Reports missing status when the installation environment is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingEnvironment_ReturnsMissing()
    {
        var result = new K16CorrectionFactor().Evaluate(new InstallationContext(
            BuriedConduitLayerCount: 1,
            BuriedConduitsPerLayer: 1,
            CablesPerBuriedConduit: 1,
            BuriedConcreteThermalResistivityKMW: 1.0));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Source.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Reports missing status when any required buried-conduit input is absent.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Evaluate_BuriedWithMissingInput_ReturnsMissing(int missingInput)
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.Buried,
            BuriedConduitLayerCount: 1,
            BuriedConduitsPerLayer: 1,
            CablesPerBuriedConduit: 1,
            BuriedConcreteThermalResistivityKMW: 1.0);

        context = missingInput switch
        {
            0 => context with { BuriedConduitLayerCount = null },
            1 => context with { BuriedConduitsPerLayer = null },
            2 => context with { CablesPerBuriedConduit = null },
            _ => context with { BuriedConcreteThermalResistivityKMW = null }
        };

        var result = new K16CorrectionFactor().Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Source.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Reports out-of-range status when the number of layers or conduits per layer is outside the table range.
    /// </summary>
    [Theory]
    [InlineData(0, 1)]
    [InlineData(7, 1)]
    [InlineData(1, 0)]
    [InlineData(1, 7)]
    public void Evaluate_BuriedWithInvalidLayerOrConduitCount_ReturnsOutOfRange(int layers, int conduitsPerLayer)
    {
        var result = new K16CorrectionFactor().Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.Buried,
            BuriedConduitLayerCount: layers,
            BuriedConduitsPerLayer: conduitsPerLayer,
            CablesPerBuriedConduit: 1,
            BuriedConcreteThermalResistivityKMW: 1.0));

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
        result.Source.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Reports out-of-range status when a conduit does not contain exactly one cable.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public void Evaluate_BuriedWithInvalidCableCount_ReturnsOutOfRange(int cablesPerConduit)
    {
        var result = new K16CorrectionFactor().Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.Buried,
            BuriedConduitLayerCount: 1,
            BuriedConduitsPerLayer: 1,
            CablesPerBuriedConduit: cablesPerConduit,
            BuriedConcreteThermalResistivityKMW: 1.0));

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
        result.Source.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Reports out-of-range status when concrete thermal resistivity differs from 1 K·m/W.
    /// </summary>
    [Theory]
    [InlineData(0.9)]
    [InlineData(1.1)]
    public void Evaluate_BuriedWithInvalidConcreteThermalResistivity_ReturnsOutOfRange(double resistivity)
    {
        var result = new K16CorrectionFactor().Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.Buried,
            BuriedConduitLayerCount: 1,
            BuriedConduitsPerLayer: 1,
            CablesPerBuriedConduit: 1,
            BuriedConcreteThermalResistivityKMW: resistivity));

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
        result.Source.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Rejects a null installation context during evaluation.
    /// </summary>
    [Fact]
    public void Evaluate_NullContext_ThrowsArgumentNullException()
    {
        Action act = () => new K16CorrectionFactor().Evaluate(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
