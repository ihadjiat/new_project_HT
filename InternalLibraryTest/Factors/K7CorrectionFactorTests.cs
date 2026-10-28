using FluentAssertions;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K7CorrectionFactorTests
{
    private readonly K7CorrectionFactor _factor = new();

    /// <summary>
    /// Verifies that Evaluate applies the documented K7 value for a valid free-air layer count.
    /// </summary>
    [Fact]
    public void Evaluate_FreeAirLayerCount_ReturnsAppliedResult()
    {
        var context = new InstallationContext(
            TouchingCableLayerCountInFreeAir: 3,
            Environment: HTA.CableSizing.Enums.InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(0.73);
        result.Source.Should().Be("instruction-metier.md §6.7");
    }

    /// <summary>
    /// Verifies that Evaluate reports K7 as not applicable for a buried installation.
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
        var result = _factor.Evaluate(new InstallationContext(TouchingCableLayerCountInFreeAir: 2));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that Evaluate reports missing data when the required layer count is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingLayerCount_ReturnsMissingResult()
    {
        var context = new InstallationContext(Environment: HTA.CableSizing.Enums.InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that Evaluate reports an out-of-range result for layer counts outside the K7 table.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    public void Evaluate_LayerCountOutOfRange_ReturnsOutOfRangeResult(int layerCount)
    {
        var context = new InstallationContext(
            TouchingCableLayerCountInFreeAir: layerCount,
            Environment: HTA.CableSizing.Enums.InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }
}