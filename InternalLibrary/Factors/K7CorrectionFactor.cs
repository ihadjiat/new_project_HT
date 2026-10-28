namespace HTA.CableSizing.Factors;

using System;
using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

public sealed class K7CorrectionFactor : IResultCorrectionFactor
{
    public string Name => "k7";

    private const string Source = "instruction-metier.md §6.7";

    private static readonly double[] LayerValues = { 1.00, 0.80, 0.73, 0.70 };

    /// <summary>
    /// Evaluates K7 for the installation environment and the number of touching cable layers.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        if (context.Environment is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The installation environment is required to evaluate K7.", Source);
        }

        if (context.Environment == InstallationEnvironment.Buried)
        {
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0, "K7 is not applicable to buried installations.", Source);
        }

        if (context.TouchingCableLayerCountInFreeAir is not int layerCount)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The number of touching cable layers in free air is required to evaluate K7.", Source);
        }

        if (layerCount < 1 || layerCount > 4)
        {
            return new FactorResult(Name, FactorStatus.OutOfRange, null, "K7 is tabulated only for 1 to 4 touching cable layers.", Source);
        }

        var value = LayerValues[layerCount - 1];
        return new FactorResult(Name, FactorStatus.Applied, value, $"K7 is {value:0.00} for {layerCount} touching cable layer(s) in free air. Combine K7 with exactly one of K3, K4, K5, or K6 according to the support.", Source);
    }
}