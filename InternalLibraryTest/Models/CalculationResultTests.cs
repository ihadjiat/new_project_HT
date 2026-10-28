using System.Collections.Generic;
using FluentAssertions;
using HTA.CableSizing.Models;
using Xunit;

namespace InternalLibraryTest.Models;

public sealed class CalculationResultTests
{
    [Fact]
    public void Constructor_ShouldPreserveProvidedValues()
    {
        var factor = new FactorResult("K1", FactorStatus.Applied, 0.91, "Applied.", "Table 52K");
        var warning = "Check the reference table conditions.";
        var factors = new[] { factor };
        var warnings = new[] { warning };

        var result = new CalculationResult(
            CalculationStatus.Partial,
            120.0,
            "Table 52G",
            100.0,
            1.2,
            120.0,
            25.0,
            factors,
            warnings);

        result.Status.Should().Be(CalculationStatus.Partial);
        result.Value.Should().Be(120.0);
        result.ReferenceTable.Should().Be("Table 52G");
        result.BaseCurrentAmps.Should().Be(100.0);
        result.GlobalCorrectionFactor.Should().Be(1.2);
        result.CorrectedCurrentAmps.Should().Be(120.0);
        result.SelectedCrossSectionMm2.Should().Be(25.0);
        result.Factors.Should().ContainSingle().Which.Should().BeSameAs(factor);
        result.Warnings.Should().ContainSingle().Which.Should().Be(warning);
    }

    [Fact]
    public void Constructor_ShouldUseEmptyListsWhenCollectionsAreNull()
    {
        var result = new CalculationResult(
            CalculationStatus.Impossible,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        result.Factors.Should().BeEmpty();
        result.Warnings.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ShouldCopySourceCollections()
    {
        var factor = new FactorResult("K1", FactorStatus.Applied, 0.91, "Applied.", null);
        var factors = new List<FactorResult> { factor };
        var warnings = new List<string> { "Initial warning." };

        var result = new CalculationResult(
            CalculationStatus.Partial,
            null,
            null,
            null,
            null,
            null,
            null,
            factors,
            warnings);

        factors.Add(new FactorResult("K2", FactorStatus.Missing, null, "Data is missing.", null));
        warnings.Add("Added after construction.");

        result.Factors.Should().ContainSingle().Which.Should().BeSameAs(factor);
        result.Warnings.Should().ContainSingle().Which.Should().Be("Initial warning.");
    }
}
