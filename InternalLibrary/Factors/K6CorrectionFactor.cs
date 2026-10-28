namespace HTA.CableSizing.Factors;

using System;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

public sealed class K6CorrectionFactor : IResultCorrectionFactor
{
    public string Name => "k6";

    private const double MinVerticalClearanceMeters = 0.30;
    private const string Source = "instruction-metier.md §6.6";

    private static readonly double[] TouchingValues =
        { 1.00, 0.88, 0.82, 0.80, 0.80, 0.79, 0.79, 0.78, 0.78 };

    /// <summary>
    /// Evaluates the K6 correction factor for the specified installation context.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        if (context.Environment is null)
        {
            return new FactorResult(
                Name,
                FactorStatus.Missing,
                null,
                "The installation environment is required to evaluate K6.",
                Source);
        }

        if (context.Environment == InstallationEnvironment.Buried)
        {
            return new FactorResult(
                Name,
                FactorStatus.NotApplicable,
                1.0,
                "K6 does not apply to buried installations.",
                Source);
        }

        if (context.TouchingCablesOnLadderOrCleatOrMeshCount is not int count)
        {
            return new FactorResult(
                Name,
                FactorStatus.Missing,
                null,
                "The number of touching cables or groups on a ladder, cleat, or mesh is required to evaluate K6.",
                Source);
        }

        if (count < 1 || count > TouchingValues.Length)
        {
            return new FactorResult(
                Name,
                FactorStatus.OutOfRange,
                null,
                "The number of touching cables or groups must be between 1 and 9 for K6.",
                Source);
        }

        if (context.CableClearanceMeters is double clearance
            && context.LargestCableDiameterMeters is double diameter
            && context.VerticalCableClearanceMeters is double verticalClearance
            && clearance >= 2 * diameter
            && verticalClearance >= MinVerticalClearanceMeters)
        {
            return new FactorResult(
                Name,
                FactorStatus.Applied,
                1.0,
                "The K6 non-touching cable clearance conditions are satisfied.",
                Source);
        }

        return new FactorResult(
            Name,
            FactorStatus.Applied,
            TouchingValues[count - 1],
            $"The K6 tabulated value for {count} touching cables or groups on a ladder, cleat, or mesh is applied.",
            Source);
    }
}