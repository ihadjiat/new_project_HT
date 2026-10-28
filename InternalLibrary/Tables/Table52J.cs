using HTA.CableSizing.Enums;

namespace HTA.CableSizing.Tables;

/// <summary>Provides the reference current values from table 52J in instruction-metier §5.4.</summary>
public sealed class Table52J : IReferenceCurrentTable
{
    private static readonly IReadOnlyDictionary<(double CrossSectionMm2, InstallationEnvironment Environment, ConductorMaterial ConductorMaterial, ArrangementType ArrangementType), double> BaseCurrents =
        new Dictionary<(double, InstallationEnvironment, ConductorMaterial, ArrangementType), double>
        {
            [(16, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 120,
            [(16, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 123,
            [(16, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 126,
            [(16, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 129,
            [(16, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 94,
            [(16, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 96,
            [(16, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 98,
            [(16, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 101,
            [(25, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 157,
            [(25, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 161,
            [(25, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 161,
            [(25, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 165,
            [(25, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 122,
            [(25, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 125,
            [(25, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 125,
            [(25, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 128,
            [(35, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 190,
            [(35, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 195,
            [(35, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 192,
            [(35, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 197,
            [(35, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 147,
            [(35, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 151,
            [(35, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 149,
            [(35, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 153,
            [(50, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 233,
            [(50, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 244,
            [(50, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 225,
            [(50, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 231,
            [(50, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 185,
            [(50, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 189,
            [(50, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 175,
            [(50, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 179,
            [(70, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 292,
            [(70, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 304,
            [(70, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 276,
            [(70, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 283,
            [(70, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 226,
            [(70, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 236,
            [(70, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 214,
            [(70, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 220,
            [(95, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 356,
            [(95, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 369,
            [(95, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 330,
            [(95, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 338,
            [(95, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 266,
            [(95, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 285,
            [(95, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 252,
            [(95, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 262,
            [(120, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 409,
            [(120, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 423,
            [(120, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 375,
            [(120, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 383,
            [(120, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 318,
            [(120, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 330,
            [(120, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 291,
            [(120, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 299,
            [(150, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 465,
            [(150, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 478,
            [(150, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 420,
            [(150, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 430,
            [(150, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 360,
            [(150, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 370,
            [(150, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 325,
            [(150, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 334,
            [(185, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 533,
            [(185, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 549,
            [(185, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 474,
            [(185, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 484,
            [(185, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 417,
            [(185, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 430,
            [(185, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 370,
            [(185, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 379,
            [(240, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 630,
            [(240, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 646,
            [(240, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 549,
            [(240, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 559,
            [(240, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 490,
            [(240, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 504,
            [(240, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 428,
            [(240, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 439,
            [(300, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 724,
            [(300, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 735,
            [(300, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 619,
            [(300, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 623,
            [(300, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 567,
            [(300, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 579,
            [(300, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 485,
            [(300, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 492,
            [(400, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 836,
            [(400, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 838,
            [(400, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 698,
            [(400, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 703,
            [(400, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 662,
            [(400, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 669,
            [(400, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 554,
            [(400, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 562,
            [(500, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 959,
            [(500, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 958,
            [(500, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 786,
            [(500, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 785,
            [(500, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 771,
            [(500, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 776,
            [(500, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 631,
            [(500, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 637,
            [(630, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 1108,
            [(630, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 1108,
            [(630, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 887,
            [(630, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 886,
            [(630, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 897,
            [(630, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 905,
            [(630, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 720,
            [(630, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 727,
            [(800, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 1255,
            [(800, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 1244,
            [(800, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 980,
            [(800, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 970,
            [(800, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 1037,
            [(800, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 1040,
            [(800, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 810,
            [(800, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 812,
            [(1000, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 1390,
            [(1000, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 1366,
            [(1000, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 1063,
            [(1000, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 1042,
            [(1000, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 1165,
            [(1000, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 1160,
            [(1000, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 895,
            [(1000, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 890,
            [(1200, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 1480,
            [(1200, InstallationEnvironment.FreeAir, ConductorMaterial.Copper, ArrangementType.Flat)] = 1445,
            [(1200, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Trefoil)] = 1117,
            [(1200, InstallationEnvironment.Buried, ConductorMaterial.Copper, ArrangementType.Flat)] = 1087,
            [(1200, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 1264,
            [(1200, InstallationEnvironment.FreeAir, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 1252,
            [(1200, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Trefoil)] = 957,
            [(1200, InstallationEnvironment.Buried, ConductorMaterial.Aluminum, ArrangementType.Flat)] = 945
        };

    private static readonly IReadOnlyList<double> AvailableCrossSections =
        Array.AsReadOnly(new[] { 16.0, 25.0, 35.0, 50.0, 70.0, 95.0, 120.0, 150.0, 185.0, 240.0, 300.0, 400.0, 500.0, 630.0, 800.0, 1000.0, 1200.0 });

    /// <summary>Gets the reference number of this table.</summary>
    public string TableNumber => "52J";

    /// <summary>Tries to get the tabulated base current for a section and configuration; arrangement type is required.</summary>
    public bool TryGetBaseCurrent(double crossSectionMm2, InstallationEnvironment environment, ConductorMaterial conductorMaterial, ArrangementType? arrangementType, out double currentAmps)
    {
        if (arrangementType is ArrangementType.Trefoil or ArrangementType.Flat
            && BaseCurrents.TryGetValue((crossSectionMm2, environment, conductorMaterial, arrangementType.Value), out currentAmps))
        {
            return true;
        }

        currentAmps = 0;
        return false;
    }

    /// <summary>Gets the immutable tabulated cross-sections for a supported configuration.</summary>
    public IReadOnlyList<double> GetAvailableCrossSections(InstallationEnvironment environment, ConductorMaterial conductorMaterial, ArrangementType? arrangementType)
    {
        if ((environment == InstallationEnvironment.FreeAir || environment == InstallationEnvironment.Buried)
            && (conductorMaterial == ConductorMaterial.Copper || conductorMaterial == ConductorMaterial.Aluminum)
            && (arrangementType is ArrangementType.Trefoil or ArrangementType.Flat))
        {
            return AvailableCrossSections;
        }

        return Array.Empty<double>();
    }
}
