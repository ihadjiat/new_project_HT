using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;
using System;
using System.Collections.Generic;

namespace HTA.CableSizing.Factors;

public sealed class K1CorrectionFactor : ICorrectionFactor
{
    public string Name => "k1";

    private const double ReferenceTemperatureCelsius = 30.0;

    private static readonly Dictionary<InsulationType, double> MaxCoreTemperature = new()
    {
        { InsulationType.Pvc, 70.0 },
        { InsulationType.PrEpr, 90.0 }
    };

    public double Calculate(InstallationContext context)
    {
        if (context.AmbientAirTemperatureCelsius is not double theta1
            || context.InsulationType is not InsulationType insulation)
            return 1.0; // k1 not applicable in this context

        double theta2 = MaxCoreTemperature[insulation];

        if (theta1 >= theta2)
            throw new ArgumentOutOfRangeException(
                nameof(context.AmbientAirTemperatureCelsius),
                "Ambient temperature must be lower than the maximum core temperature.");

        return Math.Sqrt((theta2 - theta1) / (theta2 - ReferenceTemperatureCelsius));
    }
}
