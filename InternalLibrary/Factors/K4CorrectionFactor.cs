namespace HTA.CableSizing.Factors;

using System;
using HTA.CableSizing.Models;

public sealed class K4CorrectionFactor : ICorrectionFactor
{
    public string Name => "k4";

    private const double NonTouchingValue = 0.95;

    private static readonly double[] TouchingValues =
        { 0.95, 0.85, 0.76, 0.72, 0.69, 0.67, 0.66, 0.65, 0.64 };

    public double Calculate(InstallationContext context)
    {
        if (context.CableClearanceMeters is double clearance
            && context.LargestCableDiameterMeters is double diameter
            && clearance >= 2 * diameter)
        {
            return NonTouchingValue;
        }

        if (context.TouchingCablesOnCeilingCount is not int count)
        {
            return 1.0; // k4 not applicable in this context
        }

        if (count < 1 || count > 9)
        {
            throw new ArgumentOutOfRangeException(
                nameof(context.TouchingCablesOnCeilingCount),
                "The number of cables or feeders must be between 1 and 9.");
        }

        return TouchingValues[count - 1];
    }
}