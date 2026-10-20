namespace HTA.CableSizing.Factors;

using System;
using HTA.CableSizing.Models;

public sealed class K5CorrectionFactor : ICorrectionFactor
{
    public string Name => "k5";

    private const double MinVerticalClearanceMeters = 0.30;

    private static readonly double[] TouchingValues =
        { 1.00, 0.88, 0.82, 0.77, 0.75, 0.73, 0.73, 0.72, 0.72 };

    public double Calculate(InstallationContext context)
    {
        if (context.CableClearanceMeters is double clearance
            && context.LargestCableDiameterMeters is double diameter
            && context.VerticalCableClearanceMeters is double verticalClearance
            && clearance >= 2 * diameter
            && verticalClearance >= MinVerticalClearanceMeters)
        {
            return 1.0;
        }

        if (context.TouchingCablesOnPerforatedTrayCount is not int count)
        {
            return 1.0; // k5 not applicable in this context
        }

        if (count < 1 || count > 9)
        {
            throw new ArgumentOutOfRangeException(
                nameof(context.TouchingCablesOnPerforatedTrayCount),
                "The number of cables or groupings must be between 1 and 9.");
        }

        return TouchingValues[count - 1];
    }
}