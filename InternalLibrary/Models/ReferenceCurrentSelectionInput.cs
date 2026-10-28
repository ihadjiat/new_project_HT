using HTA.CableSizing.Enums;

namespace HTA.CableSizing.Models;

/// <summary>Contains the cable information required to select a reference current table.</summary>
public sealed class ReferenceCurrentSelectionInput
{
    /// <summary>Initializes reference current table selection input.</summary>
    /// <param name="cableConstruction">The cable construction, or null when it is unknown.</param>
    /// <param name="electricFieldType">The electric field type, or null when it is unknown.</param>
    /// <param name="insulationType">The insulation type, or null when it is unknown.</param>
    /// <param name="conductorMaterial">The conductor material, or null when it is unknown.</param>
    /// <param name="assignedVoltage">The assigned voltage, or null when it is unknown.</param>
    /// <param name="arrangementType">The cable arrangement used by table 52J, or null when it is unknown.</param>
    public ReferenceCurrentSelectionInput(
        CableConstruction? cableConstruction,
        ElectricFieldType? electricFieldType,
        InsulationType? insulationType,
        ConductorMaterial? conductorMaterial,
        AssignedVoltage? assignedVoltage,
        ArrangementType? arrangementType = null)
    {
        CableConstruction = cableConstruction;
        ElectricFieldType = electricFieldType;
        InsulationType = insulationType;
        ConductorMaterial = conductorMaterial;
        AssignedVoltage = assignedVoltage;
        ArrangementType = arrangementType;
    }

    /// <summary>Gets the cable construction.</summary>
    public CableConstruction? CableConstruction { get; }

    /// <summary>Gets the electric field type.</summary>
    public ElectricFieldType? ElectricFieldType { get; }

    /// <summary>Gets the insulation type.</summary>
    public InsulationType? InsulationType { get; }

    /// <summary>Gets the conductor material.</summary>
    public ConductorMaterial? ConductorMaterial { get; }

    /// <summary>Gets the assigned voltage.</summary>
    public AssignedVoltage? AssignedVoltage { get; }

    /// <summary>Gets the arrangement type, required only for table 52J.</summary>
    public ArrangementType? ArrangementType { get; }
}
