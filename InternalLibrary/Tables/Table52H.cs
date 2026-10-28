using HTA.CableSizing.Enums;

namespace HTA.CableSizing.Tables;

/// <summary>Provides the reference current values from table 52H in instruction-metier §5.2.</summary>
public sealed class Table52H : IReferenceCurrentTable
{
    private static readonly IReadOnlyDictionary<(double CrossSectionMm2, InstallationEnvironment Environment, ConductorMaterial ConductorMaterial), double> BaseCurrents =
        new Dictionary<(double, InstallationEnvironment, ConductorMaterial), double>
        {
            [(10, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 78,
            [(10, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 86,
            [(10, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 60,
            [(10, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 67,
            [(16, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 100,
            [(16, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 110,
            [(16, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 79,
            [(16, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 86,
            [(25, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 130,
            [(25, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 145,
            [(25, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 105,
            [(25, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 110,
            [(35, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 165,
            [(35, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 170,
            [(35, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 125,
            [(35, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 135,
            [(50, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 205,
            [(50, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 215,
            [(50, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 160,
            [(50, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 165,
            [(70, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 255,
            [(70, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 260,
            [(70, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 195,
            [(70, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 205,
            [(95, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 310,
            [(95, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 315,
            [(95, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 240,
            [(95, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 245,
            [(120, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 360,
            [(120, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 360,
            [(120, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 280,
            [(120, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 280,
            [(150, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 410,
            [(150, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 405,
            [(150, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 320,
            [(150, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 315,
            [(185, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 460,
            [(185, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 450,
            [(185, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 360,
            [(185, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 350,
            [(240, InstallationEnvironment.FreeAir, ConductorMaterial.Copper)] = 550,
            [(240, InstallationEnvironment.Buried, ConductorMaterial.Copper)] = 525,
            [(240, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum)] = 430,
            [(240, InstallationEnvironment.Buried, ConductorMaterial.Aluminum)] = 410
        };

    private static readonly IReadOnlyList<double> AvailableCrossSections =
        Array.AsReadOnly(new[] { 10.0, 16.0, 25.0, 35.0, 50.0, 70.0, 95.0, 120.0, 150.0, 185.0, 240.0 });

    /// <summary>Gets the reference number of this table.</summary>
    public string TableNumber => "52H";

    /// <summary>Tries to get the tabulated base current; arrangement type is not used by table 52H.</summary>
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

    /// <summary>Gets the immutable tabulated cross-sections; arrangement type is not used by table 52H.</summary>
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
