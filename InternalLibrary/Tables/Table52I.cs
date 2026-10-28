using HTA.CableSizing.Enums;

namespace HTA.CableSizing.Tables;

/// <summary>Provides the reference current values from table 52I in instruction-metier §5.3.</summary>
public sealed class Table52I : IReferenceCurrentTable
{
    private static readonly IReadOnlyDictionary<(double CrossSectionMm2, InstallationEnvironment Environment, ConductorMaterial ConductorMaterial), double> BaseCurrents =
        new Dictionary<(double, InstallationEnvironment, ConductorMaterial), double>
        {
            [(10, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 62,
            [(10, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 70,
            [(10, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 48,
            [(10, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 56,
            [(16, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 81,
            [(16, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 94,
            [(16, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 62,
            [(16, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 72,
            [(25, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 105,
            [(25, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 120,
            [(25, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 82,
            [(25, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 94,
            [(35, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 130,
            [(35, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 145,
            [(35, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 100,
            [(35, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 115,
            [(50, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 165,
            [(50, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 185,
            [(50, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 130,
            [(50, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 145,
            [(70, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 205,
            [(70, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 225,
            [(70, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 160,
            [(70, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 175,
            [(95, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 250,
            [(95, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 270,
            [(95, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 195,
            [(95, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 210,
            [(120, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 290,
            [(120, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 310,
            [(120, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 225,
            [(120, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 240,
            [(150, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 330,
            [(150, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 345,
            [(150, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 255,
            [(150, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 270,
            [(185, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 370,
            [(185, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 385,
            [(185, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 285,
            [(185, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 300,
            [(240, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 440,
            [(240, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 445,
            [(240, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 345,
            [(240, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 350
        };

    private static readonly IReadOnlyList<double> AvailableCrossSections =
        Array.AsReadOnly(new[] { 10.0, 16.0, 25.0, 35.0, 50.0, 70.0, 95.0, 120.0, 150.0, 185.0, 240.0 });

    /// <summary>Gets the reference number of this table.</summary>
    public string TableNumber => "52I";

    /// <summary>Tries to get the tabulated base current; arrangement type is not used by table 52I.</summary>
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

    /// <summary>Gets the immutable tabulated cross-sections; arrangement type is not used by table 52I.</summary>
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
