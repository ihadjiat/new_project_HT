using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CInterfaceNative;

/// <summary>Native exports (cdecl). Thin wrappers over <see cref="NativeApi"/>; the caller allocates all structs and buffers.</summary>
public static unsafe class Exports
{
    /// <summary>Calculates the global correction factor.</summary>
    /// <param name="ctx">Input context, allocated by the caller.</param>
    /// <param name="result">Output result, allocated by the caller.</param>
    /// <returns>0 = OK, 1 = null argument, 2 = inconsistent native data, 3 = internal error.</returns>
    [UnmanagedCallersOnly(EntryPoint = "hta_calculate_global_factor", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static int CalculateGlobalFactor(InstallationContextNative* ctx, CalculationResultNative* result)
        => NativeApi.CalculateGlobalFactor(ctx, result);

    /// <summary>Calculates the corrected allowable current for a section.</summary>
    /// <param name="sectionMm2">Section in mm².</param>
    /// <param name="input">Selection input, allocated by the caller.</param>
    /// <param name="ctx">Input context, allocated by the caller.</param>
    /// <param name="result">Output result, allocated by the caller.</param>
    /// <returns>0 = OK, 1 = null argument, 2 = inconsistent native data, 3 = internal error.</returns>
    [UnmanagedCallersOnly(EntryPoint = "hta_calculate_allowable_current", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static int CalculateAllowableCurrent(double sectionMm2, SelectionInputNative* input, InstallationContextNative* ctx, CalculationResultNative* result)
        => NativeApi.CalculateAllowableCurrent(sectionMm2, input, ctx, result);

    /// <summary>Calculates the minimum section for a desired current.</summary>
    /// <param name="desiredCurrent">Desired current in amperes.</param>
    /// <param name="input">Selection input, allocated by the caller.</param>
    /// <param name="ctx">Input context, allocated by the caller.</param>
    /// <param name="result">Output result, allocated by the caller.</param>
    /// <returns>0 = OK, 1 = null argument, 2 = inconsistent native data, 3 = internal error.</returns>
    [UnmanagedCallersOnly(EntryPoint = "hta_calculate_minimum_section", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static int CalculateMinimumSection(double desiredCurrent, SelectionInputNative* input, InstallationContextNative* ctx, CalculationResultNative* result)
        => NativeApi.CalculateMinimumSection(desiredCurrent, input, ctx, result);

    /// <summary>Copies a warning of the last result as UTF-8 into a caller buffer.</summary>
    /// <param name="index">Zero-based warning index.</param>
    /// <param name="buffer">Buffer allocated by the caller.</param>
    /// <param name="bufferSize">Buffer size in bytes.</param>
    /// <returns>Bytes written excluding the terminator, or -1 for invalid arguments.</returns>
    [UnmanagedCallersOnly(EntryPoint = "hta_get_warning", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static int GetWarning(int index, byte* buffer, int bufferSize)
        => NativeApi.GetWarning(index, buffer, bufferSize);

    /// <summary>Fills a factor of the last result.</summary>
    /// <param name="index">Zero-based factor index.</param>
    /// <param name="output">Output struct, allocated by the caller.</param>
    /// <returns>0 = OK, 1 = null argument, 2 = invalid index, 3 = internal error.</returns>
    [UnmanagedCallersOnly(EntryPoint = "hta_get_factor", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static int GetFactor(int index, FactorResultNative* output)
        => NativeApi.GetFactor(index, output);

    /// <summary>Copies a factor explanation of the last result as UTF-8 into a caller buffer.</summary>
    /// <param name="index">Zero-based factor index.</param>
    /// <param name="buffer">Buffer allocated by the caller.</param>
    /// <param name="bufferSize">Buffer size in bytes.</param>
    /// <returns>Bytes written excluding the terminator, or -1 for invalid arguments.</returns>
    [UnmanagedCallersOnly(EntryPoint = "hta_get_factor_explanation", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static int GetFactorExplanation(int index, byte* buffer, int bufferSize)
        => NativeApi.GetFactorExplanation(index, buffer, bufferSize);
}
