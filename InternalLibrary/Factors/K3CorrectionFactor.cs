namespace HTA.CableSizing.Factors;

using System;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

public sealed class K3CorrectionFactor : IResultCorrectionFactor
{
    public string Name => "k3";

    private const string Source = "instruction-metier.md §6.3";

    private static readonly double[] TouchingValues =
        { 1.0, 0.85, 0.79, 0.75, 0.73, 0.72, 0.72, 0.71, 0.70 };

    /// <summary>
    /// Evaluates K3 for the specified installation context using the documented touching-cable table.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The installation environment is required to evaluate K3.", Source);
        }

        if (context.Environment == InstallationEnvironment.Buried)
        {
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0, "K3 is not applicable to buried installations.", Source);
        }

        if (context.Environment != InstallationEnvironment.FreeAir)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The installation environment is not supported for K3 evaluation.", Source);
        }

        if (context.TouchingCablesOnUnperforatedSurfaceCount is not int count)
        {
            var explanation = context.CableClearanceMeters is not null && context.LargestCableDiameterMeters is not null
                ? "Cable clearance and largest cable diameter do not identify whether the support is an unperforated surface or a ceiling; provide the K3 support type and touching-cable count."
                : "The K3 touching-cable count on an unperforated surface is required for a free-air installation.";
            return new FactorResult(Name, FactorStatus.Missing, null, explanation, Source);
        }

        if (count < 1 || count > TouchingValues.Length)
        {
            return new FactorResult(Name, FactorStatus.OutOfRange, null, "The K3 touching-cable count must be between 1 and 9; no extrapolation is applied.", Source);
        }

        return new FactorResult(Name, FactorStatus.Applied, TouchingValues[count - 1], "K3 was applied using the documented table for cables touching an unperforated surface.", Source);
    }
}
