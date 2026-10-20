namespace HTA.CableSizing.Factors;

using System;
using HTA.CableSizing.Models;

public sealed class K7CorrectionFactor : ICorrectionFactor
{
    public string Name => "k7";

    private static readonly double[] LayerValues = { 1.00, 0.80, 0.73, 0.70 };

    public double Calculate(InstallationContext context)
    {
        if (context.TouchingCableLayerCountInFreeAir is not int layerCount)
        {
            return 1.0; // k7 not applicable in this context
        }

        if (layerCount < 1 || layerCount > 4)
        {
            throw new ArgumentOutOfRangeException(
                nameof(context.TouchingCableLayerCountInFreeAir),
                "The number of layers must be between 1 and 4.");
        }

        return LayerValues[layerCount - 1];
    }
}