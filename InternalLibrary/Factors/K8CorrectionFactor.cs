using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class K8CorrectionFactor : ICorrectionFactor
{
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

    public double Calculate(InstallationContext context)
    {
        if (context.TouchingConduitLayerCountInFreeAir is not int layers || context.TouchingConduitsPerLayerInFreeAir is not int conduits)
        {
            return 1.0; // k8 not applicable in this context
        }

        var key = (layers, conduits);
        if (!Table.TryGetValue(key, out var value))
        {
            throw new ArgumentOutOfRangeException($"No k8 value for {layers} layer(s) / {conduits} conduit(s) per layer.");
        }
        
        return value;
    }
}
