using HTA.CableSizing.Enums;

namespace HTA.CableSizing.Tables;

/// <summary>Defines an immutable reference current table for supported cable configurations.</summary>
public interface IReferenceCurrentTable
{
    /// <summary>Gets the reference number of this table.</summary>
    string TableNumber { get; }

    /// <summary>Tries to get the base current for a cross-section and configuration; returns false and zero when no valid tabulated value exists.</summary>
    bool TryGetBaseCurrent(double crossSectionMm2, InstallationEnvironment environment, ConductorMaterial conductorMaterial, ArrangementType? arrangementType, out double currentAmps);

    /// <summary>Gets the immutable cross-sections available for a configuration, or an empty list when none are available.</summary>
    IReadOnlyList<double> GetAvailableCrossSections(InstallationEnvironment environment, ConductorMaterial conductorMaterial, ArrangementType? arrangementType);
}
