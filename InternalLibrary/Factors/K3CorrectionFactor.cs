namespace HTA.CableSizing.Factors;

using System;
using HTA.CableSizing.Models;

public sealed class K3CorrectionFactor : ICorrectionFactor
{
    public string Name => "k3";

    private static readonly double[] TouchingValues =
        { 1.0, 0.85, 0.79, 0.75, 0.73, 0.72, 0.72, 0.71, 0.70 };

    public double Calculate(InstallationContext context)
    {
        if (context.CableClearanceMeters is double clearance
            && context.LargestCableDiameterMeters is double diameter
            && clearance >= 2 * diameter)
        {
            return 1.0;
        }

        if (context.TouchingCablesOnUnperforatedSurfaceCount is not int count)
        {
            return 1.0; // k3 not applicable in this context
        }

        if (count < 1 || count > 9)
        {
            throw new ArgumentOutOfRangeException(
                nameof(context.TouchingCablesOnUnperforatedSurfaceCount),
                "The number of cables or groupings must be between 1 and 9.");
        }

        return TouchingValues[count - 1];
    }
}
