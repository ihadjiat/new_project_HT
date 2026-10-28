using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class K10CorrectionFactor : IResultCorrectionFactor
{
    private const string Source = "instruction-metier.md §6.10";

    private static readonly IReadOnlyDictionary<double, double> Values = new Dictionary<double, double>
    {
        [5] = 0.60,
        [7] = 0.66,
        [10] = 0.72,
        [15] = 0.80,
        [20] = 0.85,
        [30] = 0.90,
        [50] = 0.92
    };

    public string Name => "k10";

    /// <summary>
    /// Evaluates the K10 correction factor for the specified installation context.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The installation environment is required to evaluate K10.", Source);
        }

        if (context.Environment == InstallationEnvironment.Buried)
        {
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0, "K10 does not apply to buried installations.", Source);
        }

        if (context.Environment != InstallationEnvironment.FreeAir)
        {
            return new FactorResult(Name, FactorStatus.OutOfRange, null, "The installation environment is not recognized for K10.", Source);
        }

        if (context.IsCableInClosedUnfilledTrenchFlushWithFloor is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The closed, unfilled trench installation indicator is required to evaluate K10.", Source);
        }

        if (!context.IsCableInClosedUnfilledTrenchFlushWithFloor.Value)
        {
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0, "K10 does not apply because the cable is not in a closed, unfilled trench flush with the floor.", Source);
        }

        if (context.CableConstruction is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The cable construction is required to evaluate K10.", Source);
        }

        if (context.CableConstruction == CableConstruction.ThreeSingleCoreCables)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "K10 cannot be calculated for three single-core cables because the documented cable circuit space requirement is ambiguous.", Source);
        }

        if (context.CableConstruction != CableConstruction.ThreeCoreCable)
        {
            return new FactorResult(Name, FactorStatus.OutOfRange, null, "The cable construction is not recognized for K10.", Source);
        }

        if (context.CableTrenchDimensionAMeters is null
            || context.CableTrenchDimensionBMeters is null
            || context.SumCableCircuitEncombrementsMeters is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "Both trench dimensions and the sum of cable circuit space requirements are required to evaluate K10.", Source);
        }

        var dimensionA = context.CableTrenchDimensionAMeters.Value;
        var dimensionB = context.CableTrenchDimensionBMeters.Value;
        var sumCableCircuitEncombrements = context.SumCableCircuitEncombrementsMeters.Value;
        if (!double.IsFinite(dimensionA) || dimensionA <= 0
            || !double.IsFinite(dimensionB) || dimensionB <= 0
            || !double.IsFinite(sumCableCircuitEncombrements) || sumCableCircuitEncombrements <= 0)
        {
            return new FactorResult(Name, FactorStatus.OutOfRange, null, "K10 trench dimensions and cable circuit space requirements must be finite values greater than zero.", Source);
        }

        var perimeter = 2 * dimensionA + dimensionB;
        var occupationCoefficient = perimeter / sumCableCircuitEncombrements;
        if (!Values.TryGetValue(occupationCoefficient, out var value))
        {
            return new FactorResult(Name, FactorStatus.OutOfRange, null, "The K10 occupation coefficient does not exactly match a documented table value.", Source);
        }

        return new FactorResult(Name, FactorStatus.Applied, value, "The K10 value was selected using the exact documented occupation coefficient.", Source);
    }
}
