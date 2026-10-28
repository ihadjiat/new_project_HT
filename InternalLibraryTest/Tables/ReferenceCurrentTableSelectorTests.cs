using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;
using HTA.CableSizing.Tables;
using Xunit;

namespace InternalLibraryTest.Tables;

/// <summary>Tests the selection rules for reference current tables.</summary>
public sealed class ReferenceCurrentTableSelectorTests
{
    /// <summary>Selects table 52G for a three-core radial PR/EPR cable at both voltage range boundaries.</summary>
    [Theory]
    [InlineData(3.6, 6, 7.2)]
    [InlineData(18, 30, 36)]
    public void SelectsTable52GAtVoltageRangeBoundaries(double u0, double u, double um)
    {
        var result = ReferenceCurrentTableSelector.Select(CreateInput(
            CableConstruction.ThreeCoreCable,
            ElectricFieldType.Radial,
            InsulationType.PrEpr,
            new AssignedVoltage(u0, u, um)));

        AssertSelectedTable(result, "52G");
    }

    /// <summary>Selects table 52H for a three-core non-radial PR/EPR cable at exactly 6/6 (7.2) kV.</summary>
    [Fact]
    public void SelectsTable52HForDocumentedConfiguration()
    {
        var result = ReferenceCurrentTableSelector.Select(CreateInput(
            CableConstruction.ThreeCoreCable,
            ElectricFieldType.NonRadial,
            InsulationType.PrEpr,
            new AssignedVoltage(6, 6, 7.2)));

        AssertSelectedTable(result, "52H");
    }

    /// <summary>Selects table 52I for a three-core non-radial PVC cable at exactly 6/6 (7.2) kV.</summary>
    [Fact]
    public void SelectsTable52IForDocumentedConfiguration()
    {
        var result = ReferenceCurrentTableSelector.Select(CreateInput(
            CableConstruction.ThreeCoreCable,
            ElectricFieldType.NonRadial,
            InsulationType.Pvc,
            new AssignedVoltage(6, 6, 7.2)));

        AssertSelectedTable(result, "52I");
    }

    /// <summary>Selects table 52J for three single-core radial PR/EPR cables at both voltage range boundaries and arrangements.</summary>
    [Theory]
    [InlineData(3.6, 6, 7.2, ArrangementType.Trefoil)]
    [InlineData(3.6, 6, 7.2, ArrangementType.Flat)]
    [InlineData(18, 30, 36, ArrangementType.Trefoil)]
    [InlineData(18, 30, 36, ArrangementType.Flat)]
    public void SelectsTable52JAtVoltageRangeBoundaries(
        double u0,
        double u,
        double um,
        ArrangementType arrangementType)
    {
        var result = ReferenceCurrentTableSelector.Select(CreateInput(
            CableConstruction.ThreeSingleCoreCables,
            ElectricFieldType.Radial,
            InsulationType.PrEpr,
            new AssignedVoltage(u0, u, um),
            arrangementType: arrangementType));

        AssertSelectedTable(result, "52J");
    }

    /// <summary>Rejects voltage values outside each inclusive component range for tables 52G and 52J.</summary>
    [Theory]
    [InlineData(3.59, 6, 7.2)]
    [InlineData(3.6, 5.99, 7.2)]
    [InlineData(3.6, 6, 7.19)]
    [InlineData(18.01, 30, 36)]
    [InlineData(18, 30.01, 36)]
    [InlineData(18, 30, 36.01)]
    public void RejectsVoltageOutsideDocumentedRange(double u0, double u, double um)
    {
        var radialThreeCore = ReferenceCurrentTableSelector.Select(CreateInput(
            CableConstruction.ThreeCoreCable,
            ElectricFieldType.Radial,
            InsulationType.PrEpr,
            new AssignedVoltage(u0, u, um)));
        var radialSingleCore = ReferenceCurrentTableSelector.Select(CreateInput(
            CableConstruction.ThreeSingleCoreCables,
            ElectricFieldType.Radial,
            InsulationType.PrEpr,
            new AssignedVoltage(u0, u, um),
            arrangementType: ArrangementType.Trefoil));

        AssertImpossible(radialThreeCore);
        AssertImpossible(radialSingleCore);
    }

    /// <summary>Rejects a 52H or 52I voltage that differs from the exact documented value.</summary>
    [Theory]
    [InlineData(6, 6, 7.19)]
    [InlineData(6.01, 6, 7.2)]
    [InlineData(6, 6.01, 7.2)]
    public void RejectsNonExactVoltageForTables52HAnd52I(double u0, double u, double um)
    {
        var prEprResult = ReferenceCurrentTableSelector.Select(CreateInput(
            CableConstruction.ThreeCoreCable,
            ElectricFieldType.NonRadial,
            InsulationType.PrEpr,
            new AssignedVoltage(u0, u, um)));
        var pvcResult = ReferenceCurrentTableSelector.Select(CreateInput(
            CableConstruction.ThreeCoreCable,
            ElectricFieldType.NonRadial,
            InsulationType.Pvc,
            new AssignedVoltage(u0, u, um)));

        AssertImpossible(prEprResult);
        AssertImpossible(pvcResult);
    }

    /// <summary>Returns an impossible result with an explanation when any required selection field is missing.</summary>
    [Theory]
    [InlineData("construction")]
    [InlineData("field")]
    [InlineData("insulation")]
    [InlineData("material")]
    [InlineData("voltage")]
    public void ReturnsImpossibleForMissingRequiredInformation(string missingField)
    {
        var input = new ReferenceCurrentSelectionInput(
            missingField == "construction" ? null : CableConstruction.ThreeCoreCable,
            missingField == "field" ? null : ElectricFieldType.Radial,
            missingField == "insulation" ? null : InsulationType.PrEpr,
            missingField == "material" ? null : ConductorMaterial.Copper,
            missingField == "voltage" ? null : new AssignedVoltage(6, 6, 7.2));

        var result = ReferenceCurrentTableSelector.Select(input);

        AssertImpossible(result);
        Assert.Contains(missingField switch
        {
            "construction" => "cable construction",
            "field" => "electric field type",
            "insulation" => "insulation type",
            "material" => "conductor material",
            _ => "assigned voltage"
        }, result.Explanation);
    }

    /// <summary>Returns an impossible result with an explanation for a null selection input.</summary>
    [Fact]
    public void ReturnsImpossibleForNullInput()
    {
        var result = ReferenceCurrentTableSelector.Select(null);

        AssertImpossible(result);
        Assert.Contains("input is missing", result.Explanation);
    }

    /// <summary>Requires an explicit trefoil or flat arrangement when selecting table 52J.</summary>
    [Theory]
    [InlineData(null)]
    [InlineData(ArrangementType.Unspecified)]
    public void RequiresArrangementForTable52J(ArrangementType? arrangementType)
    {
        var result = ReferenceCurrentTableSelector.Select(CreateInput(
            CableConstruction.ThreeSingleCoreCables,
            ElectricFieldType.Radial,
            InsulationType.PrEpr,
            new AssignedVoltage(6, 6, 7.2),
            arrangementType: arrangementType));

        AssertImpossible(result);
        Assert.Contains("requires a trefoil or flat arrangement", result.Explanation);
    }

    /// <summary>Rejects every unknown enum value instead of selecting a table by default.</summary>
    [Theory]
    [InlineData("construction")]
    [InlineData("field")]
    [InlineData("insulation")]
    [InlineData("material")]
    [InlineData("arrangement")]
    public void ReturnsImpossibleForUnknownEnumValues(string enumName)
    {
        var input = CreateInput(
            enumName == "construction" ? (CableConstruction)99 : CableConstruction.ThreeCoreCable,
            enumName == "field" ? (ElectricFieldType)99 : ElectricFieldType.Radial,
            enumName == "insulation" ? (InsulationType)99 : InsulationType.PrEpr,
            new AssignedVoltage(6, 6, 7.2),
            enumName == "material" ? (ConductorMaterial)99 : ConductorMaterial.Copper,
            enumName == "arrangement" ? (ArrangementType)99 : null);

        var result = ReferenceCurrentTableSelector.Select(input);

        AssertImpossible(result);
        Assert.Contains("unknown enum value", result.Explanation);
    }

    /// <summary>Rejects cable combinations that do not match any documented table rule.</summary>
    [Theory]
    [InlineData(CableConstruction.ThreeCoreCable, ElectricFieldType.Radial, InsulationType.Pvc, 6, 6, 7.2)]
    [InlineData(CableConstruction.ThreeCoreCable, ElectricFieldType.NonRadial, InsulationType.PrEpr, 18, 30, 36)]
    [InlineData(CableConstruction.ThreeSingleCoreCables, ElectricFieldType.NonRadial, InsulationType.PrEpr, 6, 6, 7.2)]
    public void ReturnsImpossibleForUncoveredCombinations(
        CableConstruction construction,
        ElectricFieldType fieldType,
        InsulationType insulationType,
        double u0,
        double u,
        double um)
    {
        var result = ReferenceCurrentTableSelector.Select(CreateInput(
            construction,
            fieldType,
            insulationType,
            new AssignedVoltage(u0, u, um),
            arrangementType: ArrangementType.Trefoil));

        AssertImpossible(result);
    }

    /// <summary>Rejects a non-finite assigned voltage with a structured explanation.</summary>
    [Fact]
    public void ReturnsImpossibleForNonFiniteAssignedVoltage()
    {
        var result = ReferenceCurrentTableSelector.Select(CreateInput(
            CableConstruction.ThreeCoreCable,
            ElectricFieldType.Radial,
            InsulationType.PrEpr,
            new AssignedVoltage(double.NaN, 6, 7.2)));

        AssertImpossible(result);
        Assert.Contains("finite", result.Explanation);
    }

    private static ReferenceCurrentSelectionInput CreateInput(
        CableConstruction? construction,
        ElectricFieldType? fieldType,
        InsulationType? insulationType,
        AssignedVoltage? voltage,
        ConductorMaterial? conductorMaterial = ConductorMaterial.Copper,
        ArrangementType? arrangementType = null)
    {
        return new ReferenceCurrentSelectionInput(
            construction,
            fieldType,
            insulationType,
            conductorMaterial,
            voltage,
            arrangementType);
    }

    private static void AssertSelectedTable(ReferenceCurrentTableSelectionResult result, string tableNumber)
    {
        Assert.Equal(ReferenceCurrentTableSelectionStatus.Selected, result.Status);
        Assert.NotNull(result.Table);
        Assert.Equal(tableNumber, result.Table!.TableNumber);
        Assert.False(string.IsNullOrWhiteSpace(result.Explanation));
    }

    private static void AssertImpossible(ReferenceCurrentTableSelectionResult result)
    {
        Assert.Equal(ReferenceCurrentTableSelectionStatus.Impossible, result.Status);
        Assert.Null(result.Table);
        Assert.False(string.IsNullOrWhiteSpace(result.Explanation));
    }
}
