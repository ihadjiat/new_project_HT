using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class K17CorrectionFactor : ICorrectionFactor
{
    // Note: the k17 table applies equally to both single-core and multi-core cables
    private static readonly IReadOnlyDictionary<(int CableCount, double SpacingMeters), double> Values =
        new Dictionary<(int, double), double>
        {
            [(2, 0.25)] = 0.74,
            [(2, 0.5)] = 0.76,
            [(2, 1.0)] = 0.78,
            [(3, 0.25)] = 0.70,
            [(3, 0.5)] = 0.73,
            [(3, 1.0)] = 0.76,
            [(4, 0.25)] = 0.67,
            [(4, 0.5)] = 0.71,
            [(4, 1.0)] = 0.75,
            [(5, 0.25)] = 0.65,
            [(5, 0.5)] = 0.70,
            [(5, 1.0)] = 0.74,
            [(6, 0.25)] = 0.63,
            [(6, 0.5)] = 0.69,
            [(6, 1.0)] = 0.73
        };

    public string Name => "k17";

    public double Calculate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.CablesInConduitCount is null && context.CableSpacingInConduitMeters is null)
        {
            return 1.0;
        }

        if (context.CablesInConduitCount is null || context.CableSpacingInConduitMeters is null)
        {
            throw new ArgumentException("The cable count and spacing in the conduit must be provided together.", nameof(context));
        }

        if (context.CablesInConduitCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(context), "The cable count must be strictly positive.");
        }

        var key = (
            CableCount: context.CablesInConduitCount.Value,
            SpacingMeters: context.CableSpacingInConduitMeters.Value);
        if (!Values.TryGetValue(key, out var correctionFactor))
        {
            throw new ArgumentException(
                $"The combination of {key.CableCount} cables and a spacing of {key.SpacingMeters} m is not defined in the k17 table.",
                nameof(context));
        }

        return correctionFactor;
    }
}
