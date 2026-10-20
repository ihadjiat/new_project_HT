namespace HTA.CableSizing.Factors;

using System;
using HTA.CableSizing.Models;

public sealed class K6CorrectionFactor : ICorrectionFactor
{
    public string Name => "k6";

    private const double MinVerticalClearanceMeters = 0.30;

    private static readonly double[] TouchingValues =
        { 1.00, 0.88, 0.82, 0.80, 0.80, 0.79, 0.79, 0.78, 0.78 };

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

        if (context.TouchingCablesOnLadderOrCleatOrMeshCount is not int count)
        {
            return 1.0; // k6 not applicable in this context
        }

        if (count < 1 || count > 9)
        {
            throw new ArgumentOutOfRangeException(
                nameof(context.TouchingCablesOnLadderOrCleatOrMeshCount),
                "The number of cables or groupings must be between 1 and 9.");
        }

        return TouchingValues[count - 1];
    }
}