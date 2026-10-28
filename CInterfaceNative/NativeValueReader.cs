using System.Runtime.CompilerServices;

namespace CInterfaceNative;

/// <summary>Converts native flag/value pairs to nullable .NET values. An absent value is never replaced by a default.</summary>
public static class NativeValueReader
{
    /// <summary>Reads a 0/1 flag.</summary>
    /// <param name="flag">The native flag.</param>
    /// <param name="name">The field name used in the error message.</param>
    /// <returns>True for 1, false for 0.</returns>
    /// <exception cref="NativeDataException">The flag is neither 0 nor 1.</exception>
    public static bool ReadFlag(int flag, string name) => flag switch
    {
        0 => false,
        1 => true,
        _ => throw new NativeDataException($"Field '{name}' must be 0 or 1 but was {flag}.")
    };

    /// <summary>Reads an optional int.</summary>
    /// <param name="has">The has_xxx flag.</param>
    /// <param name="value">The native value.</param>
    /// <param name="name">The field name.</param>
    /// <returns>Null when has is 0, otherwise the value.</returns>
    public static int? ReadInt(int has, int value, string name) => ReadFlag(has, "has_" + name) ? value : null;

    /// <summary>Reads an optional double.</summary>
    /// <param name="has">The has_xxx flag.</param>
    /// <param name="value">The native value.</param>
    /// <param name="name">The field name.</param>
    /// <returns>Null when has is 0, otherwise the value.</returns>
    public static double? ReadDouble(int has, double value, string name) => ReadFlag(has, "has_" + name) ? value : null;

    /// <summary>Reads an optional boolean passed as int (0 or 1).</summary>
    /// <param name="has">The has_xxx flag.</param>
    /// <param name="value">The native value, which must be 0 or 1 when present.</param>
    /// <param name="name">The field name.</param>
    /// <returns>Null when has is 0, otherwise the boolean.</returns>
    public static bool? ReadBool(int has, int value, string name) => ReadFlag(has, "has_" + name) ? ReadFlag(value, name) : null;

    /// <summary>Reads an optional enum passed as int, without validating the numeric value.</summary>
    /// <typeparam name="TEnum">An int-based enum.</typeparam>
    /// <param name="has">The has_xxx flag.</param>
    /// <param name="value">The native value.</param>
    /// <param name="name">The field name.</param>
    /// <returns>Null when has is 0, otherwise the enum value.</returns>
    public static TEnum? ReadEnum<TEnum>(int has, int value, string name) where TEnum : unmanaged, Enum
    {
        return ReadFlag(has, "has_" + name) ? ToEnum<TEnum>(value) : null;
    }

    /// <summary>Converts an int to an int-based enum without validation.</summary>
    /// <typeparam name="TEnum">An int-based enum.</typeparam>
    /// <param name="value">The numeric value.</param>
    /// <returns>The enum value.</returns>
    public static TEnum ToEnum<TEnum>(int value) where TEnum : unmanaged, Enum => Unsafe.As<int, TEnum>(ref value);
}
