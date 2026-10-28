using System.Collections.Generic;

namespace HTA.CableSizing.Models;

/// <summary>
/// Represents the outcome, intermediate values, factors, and warnings of a calculation.
/// </summary>
public sealed class CalculationResult
{
    /// <summary>
    /// Initializes a calculation result.
    /// </summary>
    /// <param name="status">The calculation status.</param>
    /// <param name="value">The primary numeric result, if calculable.</param>
    /// <param name="referenceTable">The reference table, if known.</param>
    /// <param name="baseCurrentAmps">The base current in amperes, if available.</param>
    /// <param name="globalCorrectionFactor">The global correction factor, if available.</param>
    /// <param name="correctedCurrentAmps">The corrected current in amperes, if available.</param>
    /// <param name="selectedCrossSectionMm2">The selected cross-section in square millimeters, if available.</param>
    /// <param name="factors">The factor results, or null to use an empty list.</param>
    /// <param name="warnings">The warnings, or null to use an empty list.</param>
    public CalculationResult(
        CalculationStatus status,
        double? value,
        string? referenceTable,
        double? baseCurrentAmps,
        double? globalCorrectionFactor,
        double? correctedCurrentAmps,
        double? selectedCrossSectionMm2,
        IReadOnlyList<FactorResult>? factors = null,
        IReadOnlyList<string>? warnings = null)
    {
        Status = status;
        Value = value;
        ReferenceTable = referenceTable;
        BaseCurrentAmps = baseCurrentAmps;
        GlobalCorrectionFactor = globalCorrectionFactor;
        CorrectedCurrentAmps = correctedCurrentAmps;
        SelectedCrossSectionMm2 = selectedCrossSectionMm2;
        Factors = new List<FactorResult>(factors ?? System.Array.Empty<FactorResult>()).AsReadOnly();
        Warnings = new List<string>(warnings ?? System.Array.Empty<string>()).AsReadOnly();
    }

    /// <summary>
    /// Gets the calculation status.
    /// </summary>
    public CalculationStatus Status { get; }

    /// <summary>
    /// Gets the primary numeric result, or null when impossible or not calculable.
    /// </summary>
    public double? Value { get; }

    /// <summary>
    /// Gets the reference table, if known.
    /// </summary>
    public string? ReferenceTable { get; }

    /// <summary>
    /// Gets the base current in amperes, if available.
    /// </summary>
    public double? BaseCurrentAmps { get; }

    /// <summary>
    /// Gets the global correction factor, if available.
    /// </summary>
    public double? GlobalCorrectionFactor { get; }

    /// <summary>
    /// Gets the corrected current in amperes, if available.
    /// </summary>
    public double? CorrectedCurrentAmps { get; }

    /// <summary>
    /// Gets the selected cross-section in square millimeters, if available.
    /// </summary>
    public double? SelectedCrossSectionMm2 { get; }

    /// <summary>
    /// Gets the factor results.
    /// </summary>
    public IReadOnlyList<FactorResult> Factors { get; }

    /// <summary>
    /// Gets the calculation warnings.
    /// </summary>
    public IReadOnlyList<string> Warnings { get; }
}
