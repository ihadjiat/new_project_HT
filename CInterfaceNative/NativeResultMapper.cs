using System.Text;
using HTA.CableSizing.Models;

namespace CInterfaceNative;

/// <summary>Maps calculation results to native structs and copies UTF-8 text into caller buffers. Testable without any native call.</summary>
public static class NativeResultMapper
{
    /// <summary>Maps a calculation result to its native summary; texts are excluded.</summary>
    /// <param name="result">The calculation result.</param>
    /// <returns>The native result.</returns>
    public static CalculationResultNative ToNative(CalculationResult result)
    {
        var native = new CalculationResultNative
        {
            status = (int)result.Status,
            reference_table = ReferenceTableCodes.FromTableNumber(result.ReferenceTable),
            warning_count = result.Warnings.Count,
            factor_count = result.Factors.Count
        };

        (native.has_value, native.value) = Optional(result.Value);
        (native.has_base_current, native.base_current) = Optional(result.BaseCurrentAmps);
        (native.has_global_factor, native.global_factor) = Optional(result.GlobalCorrectionFactor);
        (native.has_corrected_current, native.corrected_current) = Optional(result.CorrectedCurrentAmps);
        (native.has_selected_section, native.selected_section) = Optional(result.SelectedCrossSectionMm2);
        return native;
    }

    /// <summary>Maps the numeric part of a factor result; name and source are written separately.</summary>
    /// <param name="factor">The factor result.</param>
    /// <param name="native">The native factor, whose status and value are set.</param>
    public static void FillFactorNumbers(FactorResult factor, ref FactorResultNative native)
    {
        native.status = (int)factor.Status;
        (native.has_value, native.value) = Optional(factor.Value);
    }

    /// <summary>
    /// Copies a string as zero-terminated UTF-8 into a buffer, never splitting a multi-byte character.
    /// </summary>
    /// <param name="text">The text; null is treated as empty.</param>
    /// <param name="buffer">The destination buffer.</param>
    /// <returns>The number of bytes written excluding the terminating zero (smaller than the full UTF-8 length when truncated), or -1 when the buffer is empty and cannot hold the terminator.</returns>
    public static int CopyUtf8(string? text, Span<byte> buffer)
    {
        if (buffer.Length == 0)
        {
            return -1;
        }

        var capacity = buffer.Length - 1;
        var written = 0;
        foreach (var rune in (text ?? string.Empty).EnumerateRunes())
        {
            var size = rune.Utf8SequenceLength;
            if (written + size > capacity)
            {
                break;
            }

            rune.EncodeToUtf8(buffer.Slice(written, size));
            written += size;
        }

        buffer[written] = 0;
        return written;
    }

    /// <summary>Gets the UTF-8 length of a text without terminator.</summary>
    /// <param name="text">The text.</param>
    /// <returns>The byte count.</returns>
    public static int GetUtf8Length(string? text) => Encoding.UTF8.GetByteCount(text ?? string.Empty);

    private static (int Has, double Value) Optional(double? value) => value.HasValue ? (1, value.Value) : (0, 0.0);
}
