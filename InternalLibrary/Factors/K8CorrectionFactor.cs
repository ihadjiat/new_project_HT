using HTA.CableSizing.Models;
using HTA.CableSizing.Enums;

namespace HTA.CableSizing.Factors;

public sealed class K8CorrectionFactor : IResultCorrectionFactor
{
    private const string Source = "instruction-metier.md §6.8";

    public string Name => "k8";

    // Key: (number of layers, conduits per layer) -> k8
    private static readonly Dictionary<(int Layers, int ConduitsPerLayer), double> Table = new()
    {
        { (1, 1), 0.96 }, { (1, 2), 0.94 }, { (1, 3), 0.91 }, { (1, 4), 0.88 }, { (1, 5), 0.87 }, { (1, 6), 0.86 },
        { (2, 1), 0.92 }, { (2, 2), 0.87 }, { (2, 3), 0.84 }, { (2, 4), 0.81 }, { (2, 5), 0.80 }, { (2, 6), 0.79 },
        { (3, 1), 0.85 }, { (3, 2), 0.81 }, { (3, 3), 0.78 }, { (3, 4), 0.76 }, { (3, 5), 0.75 }, { (3, 6), 0.74 },
        { (4, 1), 0.82 }, { (4, 2), 0.78 }, { (4, 3), 0.74 }, { (4, 4), 0.73 }, { (4, 5), 0.72 }, { (4, 6), 0.72 },
        { (5, 1), 0.80 }, { (5, 2), 0.76 }, { (5, 3), 0.72 }, { (5, 4), 0.71 }, { (5, 5), 0.70 }, { (5, 6), 0.70 },
        { (6, 1), 0.79 }, { (6, 2), 0.75 }, { (6, 3), 0.71 }, { (6, 4), 0.70 }, { (6, 5), 0.69 }, { (6, 6), 0.68 },
    };

    /// <summary>
    /// Evaluates K8 for touching conduits in free air using the documented layer and conduit counts.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The installation environment is required to evaluate K8.", Source);
        }

        if (context.Environment == InstallationEnvironment.Buried)
        {
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0, "K8 is not applicable to buried installations.", Source);
        }

        var layers = context.TouchingConduitLayerCountInFreeAir;
        var conduitsPerLayer = context.TouchingConduitsPerLayerInFreeAir;

        if ((layers is int layerCount && (layerCount < 1 || layerCount > 6))
            || (conduitsPerLayer is int conduitCount && (conduitCount < 1 || conduitCount > 6)))
        {
            return new FactorResult(Name, FactorStatus.OutOfRange, null, "K8 is tabulated only for 1 to 6 layers and 1 to 6 touching conduits per layer.", Source);
        }

        if (layers is not int validLayers || conduitsPerLayer is not int validConduitsPerLayer)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The number of touching conduit layers and conduits per layer in free air are required to evaluate K8.", Source);
        }

        var key = (validLayers, validConduitsPerLayer);
        if (!Table.TryGetValue(key, out var value))
        {
            return new FactorResult(Name, FactorStatus.OutOfRange, null, "The supplied K8 layer and conduit counts do not have a documented table value.", Source);
        }

        return new FactorResult(Name, FactorStatus.Applied, value, $"K8 is {value:0.00} for {validLayers} layer(s) and {validConduitsPerLayer} touching conduit(s) per layer in free air.", Source);
    }
}
