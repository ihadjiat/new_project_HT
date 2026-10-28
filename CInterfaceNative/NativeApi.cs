using HTA.CableSizing;
using HTA.CableSizing.Models;

namespace CInterfaceNative;

/// <summary>Return codes of the native exports.</summary>
public static class NativeReturnCodes
{
    /// <summary>The result was written, whatever the business status.</summary>
    public const int Ok = 0;

    /// <summary>A required pointer argument is null.</summary>
    public const int NullArgument = 1;

    /// <summary>Inconsistent native data (a flag or boolean that is neither 0 nor 1).</summary>
    public const int InvalidData = 2;

    /// <summary>Unexpected internal error.</summary>
    public const int InternalError = 3;
}

/// <summary>Testable bodies of the exports. Every method catches all exceptions and returns a <see cref="NativeReturnCodes"/> value.</summary>
public static unsafe class NativeApi
{
    /// <summary>Calculates the global correction factor.</summary>
    /// <param name="ctx">The native context, owned by the caller.</param>
    /// <param name="result">The output struct, owned by the caller.</param>
    /// <returns>A return code.</returns>
    public static int CalculateGlobalFactor(InstallationContextNative* ctx, CalculationResultNative* result)
    {
        if (ctx == null || result == null)
        {
            return NativeReturnCodes.NullArgument;
        }

        var nativeContext = *ctx;
        return Execute(result, () =>
        {
            var context = NativeInputMapper.ToInstallationContext(nativeContext);
            return new AllowableCurrentCalculator().CalculateGlobalCorrectionFactor(context);
        });
    }

    /// <summary>Calculates the corrected allowable current for a section.</summary>
    /// <param name="sectionMm2">The section in mm².</param>
    /// <param name="input">The native selection input, owned by the caller.</param>
    /// <param name="ctx">The native context, owned by the caller.</param>
    /// <param name="result">The output struct, owned by the caller.</param>
    /// <returns>A return code.</returns>
    public static int CalculateAllowableCurrent(double sectionMm2, SelectionInputNative* input, InstallationContextNative* ctx, CalculationResultNative* result)
    {
        if (input == null || ctx == null || result == null)
        {
            return NativeReturnCodes.NullArgument;
        }

        var nativeInput = *input;
        var nativeContext = *ctx;
        return Execute(result, () =>
        {
            var selection = NativeInputMapper.ToSelectionInput(nativeInput);
            var context = NativeInputMapper.ToInstallationContext(nativeContext);
            return new AllowableCurrentCalculator().CalculateAllowableCurrent(sectionMm2, selection, context);
        });
    }

    /// <summary>Calculates the minimum section for a desired current.</summary>
    /// <param name="desiredCurrent">The desired current in amperes.</param>
    /// <param name="input">The native selection input, owned by the caller.</param>
    /// <param name="ctx">The native context, owned by the caller.</param>
    /// <param name="result">The output struct, owned by the caller.</param>
    /// <returns>A return code.</returns>
    public static int CalculateMinimumSection(double desiredCurrent, SelectionInputNative* input, InstallationContextNative* ctx, CalculationResultNative* result)
    {
        if (input == null || ctx == null || result == null)
        {
            return NativeReturnCodes.NullArgument;
        }

        var nativeInput = *input;
        var nativeContext = *ctx;
        return Execute(result, () =>
        {
            var selection = NativeInputMapper.ToSelectionInput(nativeInput);
            var context = NativeInputMapper.ToInstallationContext(nativeContext);
            return new AllowableCurrentCalculator().CalculateMinimumCrossSection(desiredCurrent, selection, context);
        });
    }

    /// <summary>Copies a warning of the last result into a caller buffer.</summary>
    /// <param name="index">The zero-based index.</param>
    /// <param name="buffer">The caller buffer.</param>
    /// <param name="bufferSize">The buffer size in bytes.</param>
    /// <returns>The bytes written excluding the terminator, or -1 for invalid arguments.</returns>
    public static int GetWarning(int index, byte* buffer, int bufferSize)
    {
        try
        {
            return buffer == null || bufferSize <= 0 ? -1 : LastResultStore.GetWarning(index, new Span<byte>(buffer, bufferSize));
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>Copies a factor explanation of the last result into a caller buffer.</summary>
    /// <param name="index">The zero-based index.</param>
    /// <param name="buffer">The caller buffer.</param>
    /// <param name="bufferSize">The buffer size in bytes.</param>
    /// <returns>The bytes written excluding the terminator, or -1 for invalid arguments.</returns>
    public static int GetFactorExplanation(int index, byte* buffer, int bufferSize)
    {
        try
        {
            return buffer == null || bufferSize <= 0 ? -1 : LastResultStore.GetFactorExplanation(index, new Span<byte>(buffer, bufferSize));
        }
        catch
        {
            return -1;
        }
    }

    /// <summary>Fills a native factor from the last result.</summary>
    /// <param name="index">The zero-based index.</param>
    /// <param name="output">The output struct, owned by the caller.</param>
    /// <returns>0 = OK, 1 = null argument, 2 = invalid index, 3 = internal error.</returns>
    public static int GetFactor(int index, FactorResultNative* output)
    {
        try
        {
            if (output == null)
            {
                return NativeReturnCodes.NullArgument;
            }

            var factor = LastResultStore.GetFactor(index);
            if (factor is null)
            {
                return NativeReturnCodes.InvalidData;
            }

            *output = default;
            NativeResultMapper.FillFactorNumbers(factor, ref *output);
            NativeResultMapper.CopyUtf8(factor.Name, new Span<byte>(output->name_buffer, FactorResultNative.TextBufferSize));
            NativeResultMapper.CopyUtf8(factor.Source, new Span<byte>(output->source_buffer, FactorResultNative.TextBufferSize));
            return NativeReturnCodes.Ok;
        }
        catch
        {
            return NativeReturnCodes.InternalError;
        }
    }

    private static int Execute(CalculationResultNative* result, Func<CalculationResult> calculate)
    {
        try
        {
            LastResultStore.Clear();
            var calculation = calculate();
            *result = NativeResultMapper.ToNative(calculation);
            LastResultStore.Set(calculation);
            return NativeReturnCodes.Ok;
        }
        catch (NativeDataException)
        {
            return NativeReturnCodes.InvalidData;
        }
        catch
        {
            return NativeReturnCodes.InternalError;
        }
    }
}
