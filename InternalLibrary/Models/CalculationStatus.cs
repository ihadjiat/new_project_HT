namespace HTA.CableSizing.Models;

/// <summary>
/// Describes whether a calculation is complete, partial, or impossible.
/// </summary>
public enum CalculationStatus
{
    /// <summary>
    /// The calculation is complete.
    /// </summary>
    Complete = 0,

    /// <summary>
    /// The calculation is partial.
    /// </summary>
    Partial = 1,

    /// <summary>
    /// The calculation is impossible.
    /// </summary>
    Impossible = 2
}
