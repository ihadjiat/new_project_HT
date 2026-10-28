using System.Runtime.InteropServices;

namespace CInterfaceNative;

/// <summary>
/// Blittable native description of one correction factor (sequential, Pack = 8). Names and sources are UTF-8, zero-terminated,
/// truncated to fit; the explanation is read with <c>hta_get_factor_explanation</c>.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 8)]
public unsafe struct FactorResultNative
{
    /// <summary>Size in bytes of <see cref="name_buffer"/> and <see cref="source_buffer"/>.</summary>
    public const int TextBufferSize = 128;

    /// <summary>FactorStatus value: 0 = Applied, 1 = NotApplicable, 2 = Missing, 3 = OutOfRange.</summary>
    public int status;
    public int has_value;
    public double value;
    public fixed byte name_buffer[TextBufferSize];
    public fixed byte source_buffer[TextBufferSize];
}
