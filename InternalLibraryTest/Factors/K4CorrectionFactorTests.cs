using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K4CorrectionFactorTests
{
    private readonly K4CorrectionFactor _factor = new();

    /// <summary>
    /// Verifies that Evaluate applies the documented K4 value for each touching-cable count.
    /// </summary>
    [Theory]
    [InlineData(1, 0.95)]
    [InlineData(2, 0.85)]
    [InlineData(3, 0.76)]
    [InlineData(4, 0.72)]
    [InlineData(5, 0.69)]
    [InlineData(6, 0.67)]
    [InlineData(7, 0.66)]
    [InlineData(8, 0.65)]
    [InlineData(9, 0.64)]
    public void Evaluate_FreeAirTouchingCables_ReturnsDocumentedValue(int count, double expected)
    {
        var context = new InstallationContext(
            TouchingCablesOnCeilingCount: count,
            Environment: InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(expected);
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Verifies that Evaluate marks K4 as not applicable to buried installations.
    /// </summary>
    [Fact]
    public void Evaluate_BuriedInstallation_ReturnsNotApplicable()
    {
        var result = _factor.Evaluate(new InstallationContext(Environment: InstallationEnvironment.Buried));

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Verifies that Evaluate reports a missing result when the installation environment is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingEnvironment_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext(TouchingCablesOnCeilingCount: 1));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Verifies that Evaluate reports missing when no K4-specific touching-cable count is provided.
    /// </summary>
    [Fact]
    public void Evaluate_FreeAirWithoutTouchingCableCount_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext(Environment: InstallationEnvironment.FreeAir));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Verifies that clearance and diameter alone do not identify the support required to evaluate K4.
    /// </summary>
    [Fact]
    public void Evaluate_ClearanceAndDiameterWithoutSupport_ReturnsMissing()
    {
        var context = new InstallationContext(
            CableClearanceMeters: 0.1,
            LargestCableDiameterMeters: 0.05,
            Environment: InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Explanation.Should().Contain("support");
    }

    /// <summary>
    /// Verifies that Evaluate returns an out-of-range result for counts outside the documented K4 table.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void Evaluate_FreeAirTouchingCableCountOutOfRange_ReturnsOutOfRange(int count)
    {
        var context = new InstallationContext(
            TouchingCablesOnCeilingCount: count,
            Environment: InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }
}