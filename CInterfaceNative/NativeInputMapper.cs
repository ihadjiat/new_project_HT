using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace CInterfaceNative;

/// <summary>Maps native input structs to .NET models. Testable without any native call.</summary>
public static class NativeInputMapper
{
    /// <summary>Maps a native installation context to the model; has_xxx = 0 gives null.</summary>
    /// <param name="n">The native context.</param>
    /// <returns>The installation context.</returns>
    /// <exception cref="NativeDataException">A flag or boolean is not 0 or 1.</exception>
    public static InstallationContext ToInstallationContext(in InstallationContextNative n)
    {
        return new InstallationContext(
            CablesInConduitCount: Int(n.has_cables_in_conduit_count, n.cables_in_conduit_count, "cables_in_conduit_count"),
            CableSpacingInConduitMeters: Dbl(n.has_cable_spacing_in_conduit_m, n.cable_spacing_in_conduit_m, "cable_spacing_in_conduit_m"),
            CablesPerPhase: Int(n.has_cables_per_phase, n.cables_per_phase, "cables_per_phase"),
            LayingDepthMeters: Dbl(n.has_laying_depth_m, n.laying_depth_m, "laying_depth_m"),
            TouchingConduitLayerCountInFreeAir: Int(n.has_touching_conduit_layer_count_in_free_air, n.touching_conduit_layer_count_in_free_air, "touching_conduit_layer_count_in_free_air"),
            TouchingConduitsPerLayerInFreeAir: Int(n.has_touching_conduits_per_layer_in_free_air, n.touching_conduits_per_layer_in_free_air, "touching_conduits_per_layer_in_free_air"),
            AmbientAirTemperatureCelsius: Dbl(n.has_ambient_air_temperature_c, n.ambient_air_temperature_c, "ambient_air_temperature_c"),
            InsulationType: NativeValueReader.ReadEnum<InsulationType>(n.has_insulation_type, n.insulation_type, "insulation_type"),
            ArrangementType: NativeValueReader.ToEnum<ArrangementType>(n.arrangement_type),
            TouchingCablesOnUnperforatedSurfaceCount: Int(n.has_touching_cables_on_unperforated_surface_count, n.touching_cables_on_unperforated_surface_count, "touching_cables_on_unperforated_surface_count"),
            CableClearanceMeters: Dbl(n.has_cable_clearance_m, n.cable_clearance_m, "cable_clearance_m"),
            LargestCableDiameterMeters: Dbl(n.has_largest_cable_diameter_m, n.largest_cable_diameter_m, "largest_cable_diameter_m"),
            TouchingCablesOnCeilingCount: Int(n.has_touching_cables_on_ceiling_count, n.touching_cables_on_ceiling_count, "touching_cables_on_ceiling_count"),
            VerticalCableClearanceMeters: Dbl(n.has_vertical_cable_clearance_m, n.vertical_cable_clearance_m, "vertical_cable_clearance_m"),
            TouchingCablesOnPerforatedTrayCount: Int(n.has_touching_cables_on_perforated_tray_count, n.touching_cables_on_perforated_tray_count, "touching_cables_on_perforated_tray_count"),
            TouchingCablesOnLadderOrCleatOrMeshCount: Int(n.has_touching_cables_on_ladder_or_cleat_or_mesh_count, n.touching_cables_on_ladder_or_cleat_or_mesh_count, "touching_cables_on_ladder_or_cleat_or_mesh_count"),
            TouchingCableLayerCountInFreeAir: Int(n.has_touching_cable_layer_count_in_free_air, n.touching_cable_layer_count_in_free_air, "touching_cable_layer_count_in_free_air"),
            Environment: NativeValueReader.ReadEnum<InstallationEnvironment>(n.has_environment, n.environment, "environment"),
            SoilTemperatureCelsius: Dbl(n.has_soil_temperature_c, n.soil_temperature_c, "soil_temperature_c"),
            BuriedCircuitOrCableCount: Int(n.has_buried_circuit_or_cable_count, n.buried_circuit_or_cable_count, "buried_circuit_or_cable_count"),
            BuriedCircuitSpacingMeters: Dbl(n.has_buried_circuit_spacing_m, n.buried_circuit_spacing_m, "buried_circuit_spacing_m"),
            BuriedConduitLayerCount: Int(n.has_buried_conduit_layer_count, n.buried_conduit_layer_count, "buried_conduit_layer_count"),
            BuriedConduitsPerLayer: Int(n.has_buried_conduits_per_layer, n.buried_conduits_per_layer, "buried_conduits_per_layer"),
            CablesPerBuriedConduit: Int(n.has_cables_per_buried_conduit, n.cables_per_buried_conduit, "cables_per_buried_conduit"),
            BuriedConcreteThermalResistivityKMW: Dbl(n.has_buried_concrete_thermal_resistivity_kmw, n.buried_concrete_thermal_resistivity_kmw, "buried_concrete_thermal_resistivity_kmw"),
            AreCablesEnclosed: NativeValueReader.ReadBool(n.has_are_cables_enclosed, n.are_cables_enclosed, "are_cables_enclosed"),
            EnclosedCableOrGroupCount: Int(n.has_enclosed_cable_or_group_count, n.enclosed_cable_or_group_count, "enclosed_cable_or_group_count"),
            IsCableOnMobileEquipmentDrum: NativeValueReader.ReadBool(n.has_is_cable_on_mobile_equipment_drum, n.is_cable_on_mobile_equipment_drum, "is_cable_on_mobile_equipment_drum"),
            DrumWindingType: NativeValueReader.ReadEnum<DrumWindingType>(n.has_drum_winding_type, n.drum_winding_type, "drum_winding_type"),
            DrumTurnsCount: Int(n.has_drum_turns_count, n.drum_turns_count, "drum_turns_count"),
            DrumCompletedLayerCount: Int(n.has_drum_completed_layer_count, n.drum_completed_layer_count, "drum_completed_layer_count"),
            IsExposedToDirectSunlight: NativeValueReader.ReadBool(n.has_is_exposed_to_direct_sunlight, n.is_exposed_to_direct_sunlight, "is_exposed_to_direct_sunlight"),
            SolarRadiationWattsPerSquareMeter: Dbl(n.has_solar_radiation_w_m2, n.solar_radiation_w_m2, "solar_radiation_w_m2"),
            CableSurfaceAbsorptivity: Dbl(n.has_cable_surface_absorptivity, n.cable_surface_absorptivity, "cable_surface_absorptivity"),
            CableSurfaceEmissivity: Dbl(n.has_cable_surface_emissivity, n.cable_surface_emissivity, "cable_surface_emissivity"),
            IsCableInClosedUnfilledTrenchFlushWithFloor: NativeValueReader.ReadBool(n.has_is_cable_in_closed_unfilled_trench_flush_with_floor, n.is_cable_in_closed_unfilled_trench_flush_with_floor, "is_cable_in_closed_unfilled_trench_flush_with_floor"),
            CableConstruction: NativeValueReader.ReadEnum<CableConstruction>(n.has_cable_construction, n.cable_construction, "cable_construction"),
            CableTrenchDimensionAMeters: Dbl(n.has_cable_trench_dimension_a_m, n.cable_trench_dimension_a_m, "cable_trench_dimension_a_m"),
            CableTrenchDimensionBMeters: Dbl(n.has_cable_trench_dimension_b_m, n.cable_trench_dimension_b_m, "cable_trench_dimension_b_m"),
            SumCableCircuitEncombrementsMeters: Dbl(n.has_sum_cable_circuit_encombrements_m, n.sum_cable_circuit_encombrements_m, "sum_cable_circuit_encombrements_m"),
            SoilThermalResistivityKMW: Dbl(n.has_soil_thermal_resistivity_kmw, n.soil_thermal_resistivity_kmw, "soil_thermal_resistivity_kmw"),
            AreCablePhaseArrangementsSymmetrical: NativeValueReader.ReadBool(n.has_are_cable_phase_arrangements_symmetrical, n.are_cable_phase_arrangements_symmetrical, "are_cable_phase_arrangements_symmetrical"));
    }

    /// <summary>Maps a native selection input to the model; has_xxx = 0 gives null.</summary>
    /// <param name="n">The native selection input.</param>
    /// <returns>The selection input.</returns>
    /// <exception cref="NativeDataException">A flag is not 0 or 1.</exception>
    public static ReferenceCurrentSelectionInput ToSelectionInput(in SelectionInputNative n)
    {
        var voltage = NativeValueReader.ReadFlag(n.has_assigned_voltage, "has_assigned_voltage")
            ? new AssignedVoltage(n.u0_kv, n.u_kv, n.um_kv)
            : null;

        return new ReferenceCurrentSelectionInput(
            NativeValueReader.ReadEnum<CableConstruction>(n.has_cable_construction, n.cable_construction, "cable_construction"),
            NativeValueReader.ReadEnum<ElectricFieldType>(n.has_electric_field_type, n.electric_field_type, "electric_field_type"),
            NativeValueReader.ReadEnum<InsulationType>(n.has_insulation_type, n.insulation_type, "insulation_type"),
            NativeValueReader.ReadEnum<ConductorMaterial>(n.has_conductor_material, n.conductor_material, "conductor_material"),
            voltage,
            NativeValueReader.ReadEnum<ArrangementType>(n.has_arrangement_type, n.arrangement_type, "arrangement_type"));
    }

    private static int? Int(int has, int value, string name) => NativeValueReader.ReadInt(has, value, name);

    private static double? Dbl(int has, double value, string name) => NativeValueReader.ReadDouble(has, value, name);
}
