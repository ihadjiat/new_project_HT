using Xunit;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;


namespace HTA.CableSizing.Tests.Factors;

public class K14CorrectionFactorTests
{
    private readonly K14CorrectionFactor _factor = new();

    /// <summary>
    /// Evaluates a documented buried laying depth as an applied correction factor.
    /// </summary>
    [Fact]
    public void Evaluate_BuriedTabulatedDepth_ReturnsAppliedFactor()
    {
        var result = _factor.Evaluate(new InstallationContext(
            LayingDepthMeters: 0.80,
            Environment: InstallationEnvironment.Buried));

        Assert.Equal(FactorStatus.Applied, result.Status);
        Assert.Equal(1.00, result.Value);
    }

    /// <summary>
    /// Reports that K14 does not apply to a free-air installation.
    /// </summary>
    [Fact]
    public void Evaluate_FreeAir_ReturnsNotApplicable()
    {
        var result = _factor.Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.FreeAir));

        Assert.Equal(FactorStatus.NotApplicable, result.Status);
        Assert.Equal(1.0, result.Value);
    }

    /// <summary>
    /// Reports missing status when the installation environment is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingEnvironment_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext(LayingDepthMeters: 0.80));

        Assert.Equal(FactorStatus.Missing, result.Status);
        Assert.Null(result.Value);
    }

    /// <summary>
    /// Reports missing status when a buried installation has no laying depth.
    /// </summary>
    [Fact]
    public void Evaluate_BuriedWithoutDepth_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.Buried));

        Assert.Equal(FactorStatus.Missing, result.Status);
        Assert.Null(result.Value);
    }

    /// <summary>
    /// Reports out-of-range status for a laying depth that is not tabulated.
    /// </summary>
    [Theory]
    [InlineData(0.70)]
    [InlineData(0.30)]
    [InlineData(3.00)]
    public void Evaluate_BuriedDepthNotTabulated_ReturnsOutOfRange(double depth)
    {
        var result = _factor.Evaluate(new InstallationContext(
            LayingDepthMeters: depth,
            Environment: InstallationEnvironment.Buried));

        Assert.Equal(FactorStatus.OutOfRange, result.Status);
        Assert.Null(result.Value);
        Assert.Contains("does not match a tabulated entry", result.Explanation);
    }
}