using FluentAssertions;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K8CorrectionFactorTests
{
    private readonly K8CorrectionFactor _factor = new();

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
    /// Verifies that Evaluate applies the documented K8 value for a valid layer and conduit combination.
    /// </summary>
    [Fact]
    public void Evaluate_FreeAirLayerAndConduitCounts_ReturnsAppliedResult()
    {
        var context = new InstallationContext(
            TouchingConduitLayerCountInFreeAir: 3,
            TouchingConduitsPerLayerInFreeAir: 4,
            Environment: HTA.CableSizing.Enums.InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(0.76);
        result.Source.Should().Be("instruction-metier.md §6.8");
    }

    /// <summary>
    /// Verifies that Evaluate reports K8 as not applicable for a buried installation.
    /// </summary>
    [Fact]
    public void Evaluate_BuriedInstallation_ReturnsNotApplicableResult()
    {
        var context = new InstallationContext(Environment: HTA.CableSizing.Enums.InstallationEnvironment.Buried);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
    }

    /// <summary>
    /// Verifies that Evaluate reports missing data when the installation environment is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingEnvironment_ReturnsMissingResult()
    {
        var context = new InstallationContext(
            TouchingConduitLayerCountInFreeAir: 2,
            TouchingConduitsPerLayerInFreeAir: 3);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that Evaluate reports missing data when the required layer count is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingLayerCount_ReturnsMissingResult()
    {
        var context = new InstallationContext(
            TouchingConduitsPerLayerInFreeAir: 3,
            Environment: HTA.CableSizing.Enums.InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that Evaluate reports missing data when the required conduit count is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingConduitCount_ReturnsMissingResult()
    {
        var context = new InstallationContext(
            TouchingConduitLayerCountInFreeAir: 2,
            Environment: HTA.CableSizing.Enums.InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that Evaluate reports an out-of-range result for layer or conduit counts outside the K8 table.
    /// </summary>
    [Theory]
    [InlineData(0, 3)]
    [InlineData(3, 7)]
    public void Evaluate_CountOutOfRange_ReturnsOutOfRangeResult(int layers, int conduits)
    {
        var context = new InstallationContext(
            TouchingConduitLayerCountInFreeAir: layers,
            TouchingConduitsPerLayerInFreeAir: conduits,
            Environment: HTA.CableSizing.Enums.InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }
}