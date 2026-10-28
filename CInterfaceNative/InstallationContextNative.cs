using System.Runtime.InteropServices;

namespace CInterfaceNative;

/// <summary>
/// Blittable native mirror of <c>InstallationContext</c> (sequential, Pack = 8).
/// Each optional value is a <c>has_xxx</c> flag (0 or 1) followed by its value; when the flag is 0 the value is ignored and the model receives null.
/// Enums are passed as their numeric value; booleans are passed as int (0 or 1).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct InstallationContextNative
{
    public int has_cables_in_conduit_count;
    public int cables_in_conduit_count;
    public int has_cable_spacing_in_conduit_m;
    public double cable_spacing_in_conduit_m;
    public int has_cables_per_phase;
    public int cables_per_phase;
    public int has_laying_depth_m;
    public double laying_depth_m;
    public int has_touching_conduit_layer_count_in_free_air;
    public int touching_conduit_layer_count_in_free_air;
    public int has_touching_conduits_per_layer_in_free_air;
    public int touching_conduits_per_layer_in_free_air;
    public int has_ambient_air_temperature_c;
    public double ambient_air_temperature_c;
    public int has_insulation_type;
    public int insulation_type;

    /// <summary>Numeric ArrangementType (0 = Unspecified); always present, as in the model.</summary>
    public int arrangement_type;

    public int has_touching_cables_on_unperforated_surface_count;
    public int touching_cables_on_unperforated_surface_count;
    public int has_cable_clearance_m;
    public double cable_clearance_m;
    public int has_largest_cable_diameter_m;
    public double largest_cable_diameter_m;
    public int has_touching_cables_on_ceiling_count;
    public int touching_cables_on_ceiling_count;
    public int has_vertical_cable_clearance_m;
    public double vertical_cable_clearance_m;
    public int has_touching_cables_on_perforated_tray_count;
    public int touching_cables_on_perforated_tray_count;
    public int has_touching_cables_on_ladder_or_cleat_or_mesh_count;
    public int touching_cables_on_ladder_or_cleat_or_mesh_count;
    public int has_touching_cable_layer_count_in_free_air;
    public int touching_cable_layer_count_in_free_air;
    public int has_environment;
    public int environment;
    public int has_soil_temperature_c;
    public double soil_temperature_c;
    public int has_buried_circuit_or_cable_count;
    public int buried_circuit_or_cable_count;
    public int has_buried_circuit_spacing_m;
    public double buried_circuit_spacing_m;
    public int has_buried_conduit_layer_count;
    public int buried_conduit_layer_count;
    public int has_buried_conduits_per_layer;
    public int buried_conduits_per_layer;
    public int has_cables_per_buried_conduit;
    public int cables_per_buried_conduit;
    public int has_buried_concrete_thermal_resistivity_kmw;
    public double buried_concrete_thermal_resistivity_kmw;
    public int has_are_cables_enclosed;
    public int are_cables_enclosed;
    public int has_enclosed_cable_or_group_count;
    public int enclosed_cable_or_group_count;
    public int has_is_cable_on_mobile_equipment_drum;
    public int is_cable_on_mobile_equipment_drum;
    public int has_drum_winding_type;
    public int drum_winding_type;
    public int has_drum_turns_count;
    public int drum_turns_count;
    public int has_drum_completed_layer_count;
    public int drum_completed_layer_count;
    public int has_is_exposed_to_direct_sunlight;
    public int is_exposed_to_direct_sunlight;
    public int has_solar_radiation_w_m2;
    public double solar_radiation_w_m2;
    public int has_cable_surface_absorptivity;
    public double cable_surface_absorptivity;
    public int has_cable_surface_emissivity;
    public double cable_surface_emissivity;
    public int has_is_cable_in_closed_unfilled_trench_flush_with_floor;
    public int is_cable_in_closed_unfilled_trench_flush_with_floor;
    public int has_cable_construction;
    public int cable_construction;
    public int has_cable_trench_dimension_a_m;
    public double cable_trench_dimension_a_m;
    public int has_cable_trench_dimension_b_m;
    public double cable_trench_dimension_b_m;
    public int has_sum_cable_circuit_encombrements_m;
    public double sum_cable_circuit_encombrements_m;
    public int has_soil_thermal_resistivity_kmw;
    public double soil_thermal_resistivity_kmw;
    public int has_are_cable_phase_arrangements_symmetrical;
    public int are_cable_phase_arrangements_symmetrical;
}
