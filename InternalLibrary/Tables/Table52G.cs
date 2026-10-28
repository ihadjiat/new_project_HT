using HTA.CableSizing.Enums;

namespace HTA.CableSizing.Tables;

/// <summary>Provides the reference current values from table 52G in instruction-metier §5.1.</summary>
public sealed class Table52G : IReferenceCurrentTable
{
    private static readonly IReadOnlyDictionary<(double CrossSectionMm2, InstallationEnvironment Environment, ConductorMaterial ConductorMaterial), double> BaseCurrents =
        new Dictionary<(double, InstallationEnvironment, ConductorMaterial), double>
        {
            [(16, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 120,
            [(16, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 126,
            [(16, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 94,
            [(16, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 98,
            [(25, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 156,
            [(25, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 160,
            [(25, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 121,
            [(25, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 125,
            [(35, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 189,
            [(35, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 192,
            [(35, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 146,
            [(35, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 148,
            [(50, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 227,
            [(50, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 226,
            [(50, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 175,
            [(50, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 175,
            [(70, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 282,
            [(70, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 277,
            [(70, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 220,
            [(70, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 215,
            [(95, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 345,
            [(95, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 331,
            [(95, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 268,
            [(95, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 257,
            [(120, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 400,
            [(120, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 377,
            [(120, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 309,
            [(120, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 292,
            [(150, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 456,
            [(150, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 423,
            [(150, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 351,
            [(150, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 327,
            [(185, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 523,
            [(185, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 478,
            [(185, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 407,
            [(185, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 372,
            [(240, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 619,
            [(240, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 554,
            [(240, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 479,
            [(240, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 431,
            [(300, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 718,
            [(300, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 627,
            [(300, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 552,
            [(300, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 487
        };

    private static readonly IReadOnlyList<double> AvailableCrossSections =
        Array.AsReadOnly(new[] { 16.0, 25.0, 35.0, 50.0, 70.0, 95.0, 120.0, 150.0, 185.0, 240.0, 300.0 });

    /// <summary>Gets the reference number of this table.</summary>
    public string TableNumber => "52G";

    /// <summary>Tries to get the tabulated base current; arrangement type is not used by table 52G.</summary>
    public bool TryGetBaseCurrent(double crossSectionMm2, InstallationEnvironment environment, ConductorMaterial conductorMaterial, ArrangementType? arrangementType, out double currentAmps)
    {
        _ = arrangementType;
        if (BaseCurrents.TryGetValue((crossSectionMm2, environment, conductorMaterial), out currentAmps))
        {
            return true;
        }

        currentAmps = 0;
        return false;
    }

    /// <summary>Gets the immutable tabulated cross-sections; arrangement type is not used by table 52G.</summary>
    public IReadOnlyList<double> GetAvailableCrossSections(InstallationEnvironment environment, ConductorMaterial conductorMaterial, ArrangementType? arrangementType)
    {
        _ = arrangementType;
        if ((environment == InstallationEnvironment.FreeAir || environment == InstallationEnvironment.Buried)
            && (conductorMaterial == ConductorMaterial.Copper || conductorMaterial == ConductorMaterial.Aluminum))
        {
            return AvailableCrossSections;
        }

        return Array.Empty<double>();
    }
}
