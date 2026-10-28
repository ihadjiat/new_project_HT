using System.Runtime.InteropServices;

namespace CInterfaceNative;

/// <summary>
/// Blittable native mirror of <c>ReferenceCurrentSelectionInput</c> (sequential, Pack = 8).
/// Enums are passed as numeric values. The assigned voltage is a single group: when <c>has_assigned_voltage</c> is 0, U0/U/Um are ignored and the model receives null.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct SelectionInputNative
{
    public int has_cable_construction;
    public int cable_construction;
    public int has_electric_field_type;
    public int electric_field_type;
    public int has_insulation_type;
    public int insulation_type;
    public int has_conductor_material;
    public int conductor_material;
    public int has_arrangement_type;
    public int arrangement_type;
    public int has_assigned_voltage;
    public double u0_kv;
    public double u_kv;
    public double um_kv;
}
