using FluentAssertions;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tests.Factors;

public class K13CorrectionFactorTests
{
    private readonly K13CorrectionFactor _factor = new();

    /// <summary>Verifies K13 returns every tabulated value for both cable constructions.</summary>
    [Theory]
    [InlineData(0.50, CableConstruction.ThreeSingleCoreCables, 1.25)]
    [InlineData(0.70, CableConstruction.ThreeSingleCoreCables, 1.14)]
    [InlineData(0.85, CableConstruction.ThreeSingleCoreCables, 1.06)]
    [InlineData(1.00, CableConstruction.ThreeSingleCoreCables, 1.00)]
    [InlineData(1.20, CableConstruction.ThreeSingleCoreCables, 0.93)]
    [InlineData(1.50, CableConstruction.ThreeSingleCoreCables, 0.85)]
    [InlineData(2.00, CableConstruction.ThreeSingleCoreCables, 0.75)]
    [InlineData(2.50, CableConstruction.ThreeSingleCoreCables, 0.68)]
    [InlineData(3.00, CableConstruction.ThreeSingleCoreCables, 0.62)]
    [InlineData(0.50, CableConstruction.ThreeCoreCable, 1.20)]
    [InlineData(0.70, CableConstruction.ThreeCoreCable, 1.10)]
    [InlineData(0.85, CableConstruction.ThreeCoreCable, 1.05)]
    [InlineData(1.00, CableConstruction.ThreeCoreCable, 1.00)]
    [InlineData(1.20, CableConstruction.ThreeCoreCable, 0.95)]
    [InlineData(1.50, CableConstruction.ThreeCoreCable, 0.88)]
    [InlineData(2.00, CableConstruction.ThreeCoreCable, 0.79)]
    [InlineData(2.50, CableConstruction.ThreeCoreCable, 0.72)]
    [InlineData(3.00, CableConstruction.ThreeCoreCable, 0.68)]
    public void Evaluate_BuriedWithTabulatedInputs_ReturnsAppliedValue(
        double resistivity, CableConstruction construction, double expected)
    {
        var result = _factor.Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.Buried,
            CableConstruction: construction,
            SoilThermalResistivityKMW: resistivity));

        result.Status.Should().Be(FactorStatus.Applied);
        result.Value.Should().Be(expected);
        result.Source.Should().Be("instruction-metier.md §6.13");
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>Verifies K13 is not applicable to free-air installations.</summary>
    [Fact]
    public void Evaluate_FreeAirInstallation_ReturnsNotApplicable()
    {
        var result = _factor.Evaluate(new InstallationContext(Environment: InstallationEnvironment.FreeAir));

        result.Status.Should().Be(FactorStatus.NotApplicable);
        result.Value.Should().Be(1.0);
        result.Source.Should().Be("instruction-metier.md §6.13");
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>Verifies K13 is missing when the installation environment is absent.</summary>
    [Fact]
    public void Evaluate_EnvironmentMissing_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext());

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Source.Should().Be("instruction-metier.md §6.13");
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>Verifies K13 is missing when soil thermal resistivity is absent.</summary>
    [Fact]
    public void Evaluate_BuriedResistivityMissing_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.Buried,
            CableConstruction: CableConstruction.ThreeCoreCable));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Source.Should().Be("instruction-metier.md §6.13");
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>Verifies K13 is missing when cable construction is absent.</summary>
    [Fact]
    public void Evaluate_BuriedConstructionMissing_ReturnsMissing()
    {
        var result = _factor.Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.Buried,
            SoilThermalResistivityKMW: 1.00));

        result.Status.Should().Be(FactorStatus.Missing);
        result.Value.Should().BeNull();
        result.Source.Should().Be("instruction-metier.md §6.13");
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>Verifies K13 rejects an unrecognized installation environment.</summary>
    [Fact]
    public void Evaluate_UnknownEnvironment_ReturnsOutOfRange()
    {
        var result = _factor.Evaluate(new InstallationContext(Environment: (InstallationEnvironment)999));

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
        result.Source.Should().Be("instruction-metier.md §6.13");
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>Verifies K13 rejects an unrecognized cable construction.</summary>
    [Fact]
    public void Evaluate_UnknownConstruction_ReturnsOutOfRange()
    {
        var result = _factor.Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.Buried,
            CableConstruction: (CableConstruction)999,
            SoilThermalResistivityKMW: 1.00));

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
        result.Source.Should().Be("instruction-metier.md §6.13");
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>Verifies K13 rejects a finite soil thermal resistivity absent from the table.</summary>
    [Fact]
    public void Evaluate_ResistivityNotTabulated_ReturnsOutOfRange()
    {
        var result = _factor.Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.Buried,
            CableConstruction: CableConstruction.ThreeCoreCable,
            SoilThermalResistivityKMW: 1.10));

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
        result.Source.Should().Be("instruction-metier.md §6.13");
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>Verifies K13 rejects non-finite soil thermal resistivity values.</summary>
    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Evaluate_NonFiniteResistivity_ReturnsOutOfRange(double resistivity)
    {
        var result = _factor.Evaluate(new InstallationContext(
            Environment: InstallationEnvironment.Buried,
            CableConstruction: CableConstruction.ThreeCoreCable,
            SoilThermalResistivityKMW: resistivity));

        result.Status.Should().Be(FactorStatus.OutOfRange);
        result.Value.Should().BeNull();
        result.Source.Should().Be("instruction-metier.md §6.13");
        result.Explanation.Should().NotBeNullOrWhiteSpace();
    }

    /// <summary>Verifies K13 throws when the installation context is null.</summary>
    [Fact]
    public void Evaluate_NullContext_ThrowsArgumentNullException()
    {
        var action = () => _factor.Evaluate(null!);

        action.Should().Throw<ArgumentNullException>();
    }
}
