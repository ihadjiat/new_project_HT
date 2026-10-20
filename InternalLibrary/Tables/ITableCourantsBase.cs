using HTA.CableSizing.Enums;

namespace HTA.CableSizing.Tables;

public interface IBaseCurrentTable
{
    /// <summary>Returns the base current for a cross-section and configuration.</summary>
    /// <exception cref="ArgumentException">The cross-section and configuration are not present in the table.</exception>
    double GetBaseCurrent(double crossSectionMm2, InstallationMethod installationMethod, CableType cableType);

    /// <summary>Returns the discrete cross-sections available for a configuration.</summary>
    /// <exception cref="ArgumentException">No cross-section is defined for the configuration.</exception>
    IEnumerable<double> GetAvailableCrossSections(InstallationMethod installationMethod, CableType cableType);
}
