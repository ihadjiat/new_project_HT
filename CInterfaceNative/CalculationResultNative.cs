using System.Runtime.InteropServices;

namespace CInterfaceNative;

/// <summary>
/// Blittable native output of a calculation (sequential, Pack = 8). Texts are not part of the struct:
/// use <c>hta_get_warning</c>, <c>hta_get_factor</c> and <c>hta_get_factor_explanation</c> with indexes below the counts.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public struct CalculationResultNative
{
    /// <summary>CalculationStatus value: 0 = Complete, 1 = Partial, 2 = Impossible.</summary>
    public int status;
    public int has_value;
    public double value;
    public int has_base_current;
    public double base_current;
    public int has_global_factor;
    public double global_factor;
    public int has_corrected_current;
    public double corrected_current;
    public int has_selected_section;
    public double selected_section;

    /// <summary>Reference table code: 0 = unknown, 1 = 52G, 2 = 52H, 3 = 52I, 4 = 52J (see <see cref="ReferenceTableCodes"/>).</summary>
    public int reference_table;

    public int warning_count;
    public int factor_count;
}
