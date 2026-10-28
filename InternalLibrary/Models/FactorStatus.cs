namespace HTA.CableSizing.Models;

/// <summary>
/// Describes whether a correction factor was applied, is not applicable, is missing, or is out of range.
/// </summary>
public enum FactorStatus
{
    /// <summary>
    /// The factor was calculated and applied.
    /// </summary>
    Applied = 0,

    /// <summary>
    /// The factor does not apply to the configuration.
    /// </summary>
    NotApplicable = 1,

    /// <summary>
    /// The factor may be required, but the necessary data is missing.
    /// </summary>
    Missing = 2,

    /// <summary>
    /// The supplied data is outside the documented values.
    /// </summary>
    OutOfRange = 3
}
