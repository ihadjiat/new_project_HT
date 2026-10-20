using HTA.CableSizing.Enums;

namespace HTA.CableSizing.Tables;

/// <summary>Temporary demonstration table; the currents below are not standard values.</summary>
public sealed class BaseCurrentTable : IBaseCurrentTable
{
    private static readonly IReadOnlyDictionary<(double CrossSectionMm2, InstallationMethod InstallationMethod, CableType CableType), double> BaseCurrents =
        new Dictionary<(double, InstallationMethod, CableType), double>
        {
            [(50, InstallationMethod.DirectBuried, CableType.Copper)] = 100,
            [(70, InstallationMethod.DirectBuried, CableType.Copper)] = 150,
            [(95, InstallationMethod.DirectBuried, CableType.Copper)] = 210
        };

    private static readonly IReadOnlyList<double> DirectBuriedCopperCrossSections = Array.AsReadOnly(new[] { 50.0, 70.0, 95.0 });

    public double GetBaseCurrent(double crossSectionMm2, InstallationMethod installationMethod, CableType cableType)
    {
        if (!double.IsFinite(crossSectionMm2) || crossSectionMm2 <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(crossSectionMm2), "The cross-section must be a finite, strictly positive number.");
        }

        if (BaseCurrents.TryGetValue((crossSectionMm2, installationMethod, cableType), out var current))
        {
            return current;
        }

        throw new ArgumentException(
            $"No base current is defined for the {crossSectionMm2} mm² cross-section, the {installationMethod} installation method, and the {cableType} cable type.");
    }

    public IEnumerable<double> GetAvailableCrossSections(InstallationMethod installationMethod, CableType cableType)
    {
        if (installationMethod == InstallationMethod.DirectBuried && cableType == CableType.Copper)
        {
            return DirectBuriedCopperCrossSections;
        }

        throw new ArgumentException($"No cross-section is defined for the {installationMethod} installation method and the {cableType} cable type.");
    }
}
