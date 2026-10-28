using HTA.CableSizing.Models;

namespace CInterfaceNative;

/// <summary>
/// Holds the last calculation result so that the detail texts can be read through separate calls.
/// Access is protected by a lock. Technical debt: process-wide state, acceptable for a single-threaded Delphi caller only.
/// </summary>
public static class LastResultStore
{
    private static readonly object Gate = new();
    private static CalculationResult? _last;

    /// <summary>Replaces the stored result.</summary>
    /// <param name="result">The result, or null to clear.</param>
    public static void Set(CalculationResult? result)
    {
        lock (Gate)
        {
            _last = result;
        }
    }

    /// <summary>Clears the stored result.</summary>
    public static void Clear() => Set(null);

    /// <summary>Copies a warning as UTF-8 into a buffer.</summary>
    /// <param name="index">The zero-based warning index.</param>
    /// <param name="buffer">The destination buffer.</param>
    /// <returns>The bytes written excluding the terminator, or -1 when the index or buffer is invalid.</returns>
    public static int GetWarning(int index, Span<byte> buffer)
    {
        lock (Gate)
        {
            if (_last is null || index < 0 || index >= _last.Warnings.Count)
            {
                return -1;
            }

            return NativeResultMapper.CopyUtf8(_last.Warnings[index], buffer);
        }
    }

    /// <summary>Gets a factor of the stored result.</summary>
    /// <param name="index">The zero-based factor index.</param>
    /// <returns>The factor, or null when the index is invalid or no result is stored.</returns>
    public static FactorResult? GetFactor(int index)
    {
        lock (Gate)
        {
            if (_last is null || index < 0 || index >= _last.Factors.Count)
            {
                return null;
            }

            return _last.Factors[index];
        }
    }

    /// <summary>Copies a factor explanation as UTF-8 into a buffer.</summary>
    /// <param name="index">The zero-based factor index.</param>
    /// <param name="buffer">The destination buffer.</param>
    /// <returns>The bytes written excluding the terminator, or -1 when the index or buffer is invalid.</returns>
    public static int GetFactorExplanation(int index, Span<byte> buffer)
    {
        var factor = GetFactor(index);
        return factor is null ? -1 : NativeResultMapper.CopyUtf8(factor.Explanation, buffer);
    }
}
