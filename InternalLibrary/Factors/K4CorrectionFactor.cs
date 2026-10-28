namespace HTA.CableSizing.Factors;

using System;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

public sealed class K4CorrectionFactor : IResultCorrectionFactor
{
    public string Name => "k4";

    private const double NonTouchingValue = 0.95;
    private const string Source = "instruction-metier.md §6.4";

    private static readonly double[] TouchingValues =
        { 0.95, 0.85, 0.76, 0.72, 0.69, 0.67, 0.66, 0.65, 0.64 };

    /// <summary>
    /// Evaluates K4 for the specified installation context using the documented touching-cable table.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        if (context.Environment is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The installation environment is required to evaluate K4.", Source);
        }

        if (context.Environment == InstallationEnvironment.Buried)
        {
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0, "K4 is not applicable to buried installations.", Source);
        }

        if (context.Environment != InstallationEnvironment.FreeAir)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The installation environment is not supported for K4 evaluation.", Source);
        }

        if (context.TouchingCablesOnCeilingCount is not int count)
        {
            var explanation = context.CableClearanceMeters is not null && context.LargestCableDiameterMeters is not null
                ? "Cable clearance and largest cable diameter do not identify whether the support is a ceiling or an unperforated surface; provide the K4 support type and touching-cable count."
                : "The K4 touching-cable count on a ceiling is required for a free-air installation.";
            return new FactorResult(Name, FactorStatus.Missing, null, explanation, Source);
        }

        if (count < 1 || count > TouchingValues.Length)
        {
            return new FactorResult(Name, FactorStatus.OutOfRange, null, "The K4 touching-cable count must be between 1 and 9; no extrapolation is applied.", Source);
        }

        return new FactorResult(Name, FactorStatus.Applied, TouchingValues[count - 1], "K4 was applied using the documented table for cables touching a ceiling.", Source);
    }
}