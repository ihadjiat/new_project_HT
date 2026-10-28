using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class K15CorrectionFactor : IResultCorrectionFactor
{
    private const string Source = "instruction-metier.md §6.15";

    private static readonly IReadOnlyDictionary<(int Count, double Spacing), double> TabulatedFactors =
        new Dictionary<(int Count, double Spacing), double>
        {
            [(2, 0.0)] = 0.76,
            [(2, 0.25)] = 0.84,
            [(2, 0.5)] = 0.88,
            [(2, 1.0)] = 0.92,
            [(3, 0.0)] = 0.64,
            [(3, 0.25)] = 0.74,
            [(3, 0.5)] = 0.79,
            [(3, 1.0)] = 0.85,
            [(4, 0.0)] = 0.57,
            [(4, 0.25)] = 0.69,
            [(4, 0.5)] = 0.75,
            [(4, 1.0)] = 0.82,
            [(5, 0.0)] = 0.52,
            [(5, 0.25)] = 0.65,
            [(5, 0.5)] = 0.71,
            [(5, 1.0)] = 0.80,
            [(6, 0.0)] = 0.48,
            [(6, 0.25)] = 0.60,
            [(6, 0.5)] = 0.69,
            [(6, 1.0)] = 0.78,
            [(7, 0.0)] = 0.44,
            [(7, 0.25)] = 0.58,
            [(7, 0.5)] = 0.68,
            [(7, 1.0)] = 0.76,
            [(8, 0.0)] = 0.41,
            [(8, 0.25)] = 0.57,
            [(8, 0.5)] = 0.67,
            [(9, 0.0)] = 0.39,
            [(9, 0.25)] = 0.55,
            [(9, 0.5)] = 0.66,
            [(10, 0.0)] = 0.37,
            [(10, 0.25)] = 0.54,
            [(11, 0.0)] = 0.36,
            [(11, 0.25)] = 0.53,
            [(12, 0.0)] = 0.35,
            [(12, 0.25)] = 0.52
        };

    public string Name => "k15";

    /// <summary>
    /// Evaluates the K15 correction factor using the tabulated buried circuit counts and spacings in section 6.15.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The installation environment is missing.", Source);
        }

        if (context.Environment == InstallationEnvironment.FreeAir)
        {
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0, "K15 does not apply to a free-air installation.", Source);
        }

        if (context.Environment != InstallationEnvironment.Buried)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The installation environment is not supported by K15.", Source);
        }

        if (context.BuriedCircuitOrCableCount is null || context.BuriedCircuitSpacingMeters is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The buried circuit/cable count or spacing is missing.", Source);
        }

        if (TabulatedFactors.TryGetValue(
                (context.BuriedCircuitOrCableCount.Value, context.BuriedCircuitSpacingMeters.Value),
                out var factor))
        {
            return new FactorResult(Name, FactorStatus.Applied, factor, "The buried circuit/cable count and spacing are tabulated in K15 section 6.15.", Source);
        }

        return new FactorResult(Name, FactorStatus.OutOfRange, null, "The supplied buried circuit/cable count and spacing combination is not tabulated in K15 section 6.15.", Source);
    }
}
