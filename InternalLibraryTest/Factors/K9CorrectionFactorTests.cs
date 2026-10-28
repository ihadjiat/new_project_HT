using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K9CorrectionFactorTests
{
    private readonly K9CorrectionFactor _factor = new();

    /// <summary>
    /// Verifies that K9 returns the documented values for tabulated enclosed cable counts.
    /// </summary>
    [Theory]
    [InlineData(1, 0.80)]
    [InlineData(3, 0.65)]
    [InlineData(20, 0.40)]
    public void Evaluate_TabulatedEnclosedCableCount_ReturnsAppliedResult(int count, double expected)
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            AreCablesEnclosed: true,
            EnclosedCableOrGroupCount: count);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(expected);
        result.Source.Should().Be("instruction-metier.md §6.9");
    }

    /// <summary>
    /// Verifies that K9 is not applicable to buried installations.
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
    /// Verifies that K9 is not applicable when cables are not enclosed.
    /// </summary>
    [Fact]
    public void Evaluate_CablesNotEnclosed_ReturnsNotApplicableResult()
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            AreCablesEnclosed: false);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
    }

    /// <summary>
    /// Verifies that K9 reports missing when the installation environment is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingEnvironment_ReturnsMissingResult()
    {
        var result = _factor.Evaluate(new InstallationContext());

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K9 reports missing when the enclosed-cable indicator is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingEnclosureIndicator_ReturnsMissingResult()
    {
        var context = new InstallationContext(Environment: InstallationEnvironment.FreeAir);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K9 reports missing when the enclosed cable count is absent.
    /// </summary>
    [Fact]
    public void Evaluate_MissingEnclosedCableCount_ReturnsMissingResult()
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            AreCablesEnclosed: true);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K9 does not interpolate or extrapolate counts absent from its table.
    /// </summary>
    [Fact]
    public void Evaluate_UntabulatedCount_ReturnsOutOfRangeResult()
    {
        var context = new InstallationContext(
            Environment: InstallationEnvironment.FreeAir,
            AreCablesEnclosed: true,
            EnclosedCableOrGroupCount: 10);

        var result = _factor.Evaluate(context);

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
    }

    /// <summary>
    /// Verifies that K9 rejects an undefined installation environment.
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
    /// Verifies that evaluating K9 with a null context throws an argument-null exception.
    /// </summary>
    [Fact]
    public void Evaluate_NullContext_ThrowsArgumentNullException()
    {
        var act = () => _factor.Evaluate(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
