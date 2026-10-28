using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class K16CorrectionFactor : IResultCorrectionFactor
{
    private const string Source = "instruction-metier.md §6.16";

    private static readonly double[,] FactorTable =
    {
        { 0.80, 0.70, 0.62, 0.58, 0.54, 0.52 },
        { 0.70, 0.57, 0.50, 0.46, 0.42, 0.40 },
        { 0.62, 0.50, 0.42, 0.38, 0.36, 0.34 },
        { 0.58, 0.46, 0.38, 0.35, 0.32, 0.30 },
        { 0.54, 0.42, 0.36, 0.32, 0.30, 0.28 },
        { 0.52, 0.40, 0.34, 0.30, 0.28, 0.26 }
    };

    public string Name => "k16";

    /// <summary>
    /// Evaluates the K16 correction factor for buried conduits using the tabulated combinations.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
        {
            return CreateResult(FactorStatus.Missing, null, "The installation environment is missing.");
        }

        if (context.Environment == InstallationEnvironment.FreeAir)
        {
            return CreateResult(FactorStatus.NotApplicable, 1.0, "K16 does not apply to free-air installations.");
        }

        if (context.Environment != InstallationEnvironment.Buried)
        {
            return CreateResult(FactorStatus.OutOfRange, null, "The installation environment is not supported by K16.");
        }

        if (context.BuriedConduitLayerCount is null ||
            context.BuriedConduitsPerLayer is null ||
            context.CablesPerBuriedConduit is null ||
            context.BuriedConcreteThermalResistivityKMW is null)
        {
            return CreateResult(FactorStatus.Missing, null, "Required buried-conduit data is missing.");
        }

        int layers = context.BuriedConduitLayerCount.Value;
        int conduitsPerLayer = context.BuriedConduitsPerLayer.Value;

        if (layers is < 1 or > 6 || conduitsPerLayer is < 1 or > 6)
        {
            return CreateResult(FactorStatus.OutOfRange, null, "The number of layers and conduits per layer must be between 1 and 6.");
        }

        if (context.CablesPerBuriedConduit != 1)
        {
            return CreateResult(FactorStatus.OutOfRange, null, "K16 requires exactly one cable per conduit.");
        }

        if (context.BuriedConcreteThermalResistivityKMW != 1.0)
        {
            return CreateResult(FactorStatus.OutOfRange, null, "K16 requires a concrete thermal resistivity of 1 K·m/W.");
        }

        double value = FactorTable[layers - 1, conduitsPerLayer - 1];
        return CreateResult(FactorStatus.Applied, value, "The buried-conduit combination is listed in the K16 table.");
    }

    private static FactorResult CreateResult(FactorStatus status, double? value, string explanation)
    {
        return new FactorResult("k16", status, value, explanation, Source);
    }
}
