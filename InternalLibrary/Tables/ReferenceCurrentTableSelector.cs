using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Tables;

/// <summary>Selects a reference current table using the documented criteria in instruction-metier §8.2.</summary>
public static class ReferenceCurrentTableSelector
{
    private static readonly IReferenceCurrentTable Table52G = new Table52G();
    private static readonly IReferenceCurrentTable Table52H = new Table52H();
    private static readonly IReferenceCurrentTable Table52I = new Table52I();
    private static readonly IReferenceCurrentTable Table52J = new Table52J();

    /// <summary>Selects the matching reference current table or returns an impossible result with an explanation.</summary>
    /// <param name="input">The cable construction, electric field, insulation, conductor, assigned voltage, and optional table 52J arrangement.</param>
    /// <returns>A structured selection result containing the selected table, or an explanation when selection is impossible.</returns>
    public static ReferenceCurrentTableSelectionResult Select(ReferenceCurrentSelectionInput? input)
    {
        if (input is null)
        {
            return Impossible("Selection input is missing.");
        }

        var missingInformation = new List<string>();
        if (input.CableConstruction is null)
        {
            missingInformation.Add("cable construction");
        }

        if (input.ElectricFieldType is null)
        {
            missingInformation.Add("electric field type");
        }

        if (input.InsulationType is null)
        {
            missingInformation.Add("insulation type");
        }

        if (input.ConductorMaterial is null)
        {
            missingInformation.Add("conductor material");
        }

        if (input.AssignedVoltage is null)
        {
            missingInformation.Add("assigned voltage");
        }

        if (missingInformation.Count > 0)
        {
            return Impossible($"Required selection information is missing: {string.Join(", ", missingInformation)}.");
        }

        if (!Enum.IsDefined(input.CableConstruction!.Value))
        {
            return Impossible("Cable construction has an unknown enum value.");
        }

        if (!Enum.IsDefined(input.ElectricFieldType!.Value))
        {
            return Impossible("Electric field type has an unknown enum value.");
        }

        if (!Enum.IsDefined(input.InsulationType!.Value))
        {
            return Impossible("Insulation type has an unknown enum value.");
        }

        if (!Enum.IsDefined(input.ConductorMaterial!.Value))
        {
            return Impossible("Conductor material has an unknown enum value.");
        }

        if (input.ArrangementType is ArrangementType arrangement && !Enum.IsDefined(arrangement))
        {
            return Impossible("Arrangement type has an unknown enum value.");
        }

        var voltage = input.AssignedVoltage!;
        if (!double.IsFinite(voltage.U0Kilovolts)
            || !double.IsFinite(voltage.UKilovolts)
            || !double.IsFinite(voltage.UmKilovolts))
        {
            return Impossible("Assigned voltage must contain finite U₀, U, and Uₘ values.");
        }

        if (input.CableConstruction == CableConstruction.ThreeCoreCable
            && input.ElectricFieldType == ElectricFieldType.Radial
            && input.InsulationType == InsulationType.PrEpr
            && IsWithinVoltageRange(voltage, 3.6, 6, 7.2, 18, 30, 36))
        {
            return Selected(Table52G, "Table 52G applies to three-core cables with a radial field, PR/EPR insulation, and assigned voltage from 3.6/6 (7.2) kV through 18/30 (36) kV.");
        }

        if (input.CableConstruction == CableConstruction.ThreeCoreCable
            && input.ElectricFieldType == ElectricFieldType.NonRadial
            && input.InsulationType == InsulationType.PrEpr
            && IsExactVoltage(voltage, 6, 6, 7.2))
        {
            return Selected(Table52H, "Table 52H applies to three-core cables with a non-radial field, PR/EPR insulation, and assigned voltage 6/6 (7.2) kV.");
        }

        if (input.CableConstruction == CableConstruction.ThreeCoreCable
            && input.ElectricFieldType == ElectricFieldType.NonRadial
            && input.InsulationType == InsulationType.Pvc
            && IsExactVoltage(voltage, 6, 6, 7.2))
        {
            return Selected(Table52I, "Table 52I applies to three-core cables with a non-radial field, PVC insulation, and assigned voltage 6/6 (7.2) kV.");
        }

        if (input.CableConstruction == CableConstruction.ThreeSingleCoreCables
            && input.ElectricFieldType == ElectricFieldType.Radial
            && input.InsulationType == InsulationType.PrEpr
            && IsWithinVoltageRange(voltage, 3.6, 6, 7.2, 18, 30, 36))
        {
            if (input.ArrangementType != ArrangementType.Trefoil && input.ArrangementType != ArrangementType.Flat)
            {
                return Impossible("Table 52J requires a trefoil or flat arrangement.");
            }

            return Selected(Table52J, "Table 52J applies to three single-core cables with a radial field, PR/EPR insulation, and assigned voltage from 3.6/6 (7.2) kV through 18/30 (36) kV.");
        }

        return Impossible("The supplied cable configuration and assigned voltage are not covered by the documented selection rules in instruction-metier §8.2.");
    }

    private static bool IsWithinVoltageRange(
        AssignedVoltage voltage,
        double minimumU0,
        double minimumU,
        double minimumUm,
        double maximumU0,
        double maximumU,
        double maximumUm)
    {
        return voltage.U0Kilovolts >= minimumU0 && voltage.U0Kilovolts <= maximumU0
            && voltage.UKilovolts >= minimumU && voltage.UKilovolts <= maximumU
            && voltage.UmKilovolts >= minimumUm && voltage.UmKilovolts <= maximumUm;
    }

    private static bool IsExactVoltage(AssignedVoltage voltage, double u0, double u, double um)
    {
        return voltage.U0Kilovolts == u0
            && voltage.UKilovolts == u
            && voltage.UmKilovolts == um;
    }

    private static ReferenceCurrentTableSelectionResult Selected(IReferenceCurrentTable table, string explanation)
    {
        return new ReferenceCurrentTableSelectionResult(ReferenceCurrentTableSelectionStatus.Selected, table, explanation);
    }

    private static ReferenceCurrentTableSelectionResult Impossible(string explanation)
    {
        return new ReferenceCurrentTableSelectionResult(ReferenceCurrentTableSelectionStatus.Impossible, null, explanation);
    }
}
