using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K3CorrectionFactorTests
{
    private readonly K3CorrectionFactor _factor = new();

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
    /// Verifies that Evaluate applies the documented K3 value for each touching-cable count.
    /// </summary>
    [Theory]
    [InlineData(1, 1.00)]
    [InlineData(2, 0.85)]
    [InlineData(3, 0.79)]
    [InlineData(4, 0.75)]
    [InlineData(5, 0.73)]
    [InlineData(6, 0.72)]
    [InlineData(7, 0.72)]
    [InlineData(8, 0.71)]
    [InlineData(9, 0.70)]
    public void Evaluate_FreeAirTouchingCables_ReturnsDocumentedValue(int count, double expected)
    {
        var context = new InstallationContext(
            TouchingCablesOnUnperforatedSurfaceCount: count,
            Environment: InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(expected);
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Evaluate_NullContext_ThrowsArgumentNullException_ForNullInput()
    {
        var act = () => _factor.Evaluate(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that Evaluate marks K3 as not applicable to buried installations.
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
        var result = _factor.Evaluate(new InstallationContext(TouchingCablesOnUnperforatedSurfaceCount: 1));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>
    /// Verifies that Evaluate reports missing when no K3-specific touching-cable count is provided.
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
    /// Verifies that clearance and diameter alone do not identify the support required to evaluate K3.
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
    /// Verifies that Evaluate returns an out-of-range result for counts outside the documented K3 table.
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void Evaluate_FreeAirTouchingCableCountOutOfRange_ReturnsOutOfRange(int count)
    {
        var context = new InstallationContext(
            TouchingCablesOnUnperforatedSurfaceCount: count,
            Environment: InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }
}