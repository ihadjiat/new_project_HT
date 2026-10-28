namespace HTA.CableSizing.Models;

/// <summary>Describes whether a reference current table was selected.</summary>
public enum ReferenceCurrentTableSelectionStatus
{
    /// <summary>A reference current table was selected.</summary>
    Selected = 0,

    /// <summary>No table can be selected reliably from the supplied information.</summary>
    Impossible = 1
}
