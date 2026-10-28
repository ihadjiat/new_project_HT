using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class K17CorrectionFactor : IResultCorrectionFactor
{
    private const string Source = "instruction-metier.md §6.17";
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

    /// <summary>
    /// Evaluates the buried-conduit correction factor using only documented table values.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The installation environment is required to evaluate k17.", Source);
        }

        if (context.Environment == InstallationEnvironment.FreeAir)
        {
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0, "K17 does not apply to free-air installations.", Source);
        }

        if (context.CablesInConduitCount is null || context.CableSpacingInConduitMeters is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "Both cable count and conduit spacing are required for buried installations.", Source);
        }

        var key = (context.CablesInConduitCount.Value, context.CableSpacingInConduitMeters.Value);
        if (Values.TryGetValue(key, out var correctionFactor))
        {
            return new FactorResult(Name, FactorStatus.Applied, correctionFactor, "The cable count and spacing match a documented table entry.", Source);
        }

        return new FactorResult(Name, FactorStatus.OutOfRange, null, "The cable count and spacing do not match a tabulated entry.", Source);
    }
}
