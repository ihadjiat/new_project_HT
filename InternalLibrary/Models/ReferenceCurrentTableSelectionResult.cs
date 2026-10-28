using HTA.CableSizing.Tables;

namespace HTA.CableSizing.Models;

/// <summary>Represents the outcome and explanation of reference current table selection.</summary>
public sealed class ReferenceCurrentTableSelectionResult
{
    internal ReferenceCurrentTableSelectionResult(
        ReferenceCurrentTableSelectionStatus status,
        IReferenceCurrentTable? table,
        string explanation)
    {
        Status = status;
        Table = table;
        Explanation = explanation;
    }

    /// <summary>Gets whether selection succeeded or is impossible.</summary>
    public ReferenceCurrentTableSelectionStatus Status { get; }

    /// <summary>Gets the selected table, or null when selection is impossible.</summary>
    public IReferenceCurrentTable? Table { get; }

    /// <summary>Gets the explanation for the selection outcome.</summary>
    public string Explanation { get; }
}
