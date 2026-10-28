namespace HTA.CableSizing.Factors;

using System;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

public sealed class K5CorrectionFactor : IResultCorrectionFactor
{
    public string Name => "k5";

    private const double MinVerticalClearanceMeters = 0.30;
    private const string Source = "instruction-metier.md §6.5";

    private static readonly double[] TouchingValues =
        { 1.00, 0.88, 0.82, 0.77, 0.75, 0.73, 0.73, 0.72, 0.72 };

    /// <summary>
    /// Evaluates the K5 correction factor for the specified installation context.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
        {
            return new FactorResult(
                Name,
                FactorStatus.Missing,
                null,
                "The installation environment is required to evaluate K5.",
                Source);
        }

        if (context.Environment == InstallationEnvironment.Buried)
        {
            return new FactorResult(
                Name,
                FactorStatus.NotApplicable,
                1.0,
                "K5 does not apply to buried installations.",
                Source);
        }

        if (context.TouchingCablesOnPerforatedTrayCount is not int count)
        {
            return new FactorResult(
                Name,
                FactorStatus.Missing,
                null,
                "The number of touching cables or groups on a perforated tray is required to evaluate K5.",
                Source);
        }

        if (count < 1 || count > TouchingValues.Length)
        {
            return new FactorResult(
                Name,
                FactorStatus.OutOfRange,
                null,
                "The number of touching cables or groups must be between 1 and 9 for K5.",
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
                "The K5 non-touching cable clearance conditions are satisfied.",
                Source);
        }

        return new FactorResult(
            Name,
            FactorStatus.Applied,
            TouchingValues[count - 1],
            $"The K5 tabulated value for {count} touching cables or groups on a perforated tray is applied.",
            Source);
    }
}