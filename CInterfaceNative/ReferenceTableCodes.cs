namespace CInterfaceNative;

/// <summary>Numeric codes of the reference tables exposed in <c>CalculationResultNative.reference_table</c>.</summary>
public static class ReferenceTableCodes
{
    /// <summary>Unknown or absent table.</summary>
    public const int Unknown = 0;

    /// <summary>Table 52G.</summary>
    public const int Table52G = 1;

    /// <summary>Table 52H.</summary>
    public const int Table52H = 2;

    /// <summary>Table 52I.</summary>
    public const int Table52I = 3;

    /// <summary>Table 52J.</summary>
    public const int Table52J = 4;

    /// <summary>Converts a table number such as "52G" to its code; 0 when null or unrecognized.</summary>
    /// <param name="tableNumber">The table number.</param>
    /// <returns>The code.</returns>
    public static int FromTableNumber(string? tableNumber) => tableNumber switch
    {
        "52G" => Table52G,
        "52H" => Table52H,
        "52I" => Table52I,
        "52J" => Table52J,
        _ => Unknown
    };
}
