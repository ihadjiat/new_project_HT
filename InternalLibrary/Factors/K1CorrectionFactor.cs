using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;
using System;
using System.Collections.Generic;

namespace HTA.CableSizing.Factors;

public sealed class K1CorrectionFactor : IResultCorrectionFactor
{
    public string Name => "k1";

    private const string Source = "instruction-metier.md §6.1";

    private static readonly Dictionary<InsulationType, Dictionary<double, double>> Table = new()
    {
        [InsulationType.Pvc] = new()
        {
            [10] = 1.22, [15] = 1.17, [20] = 1.12, [25] = 1.06, [30] = 1.00,
            [35] = 0.94, [40] = 0.87, [45] = 0.79, [50] = 0.71, [55] = 0.61
        },
        [InsulationType.PrEpr] = new()
        {
            [10] = 1.15, [15] = 1.12, [20] = 1.08, [25] = 1.04, [30] = 1.00,
            [35] = 0.96, [40] = 0.91, [45] = 0.87, [50] = 0.82, [55] = 0.76
        }
    };

    /// <summary>
    /// Evaluates K1 using the documented ambient-temperature table for free-air installations.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
            return new FactorResult(Name, FactorStatus.Missing, null,
                "The installation environment is required to determine whether K1 applies.", Source);

        if (context.Environment == InstallationEnvironment.Buried)
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0,
                "K1 does not apply to a buried installation.", Source);

        if (context.Environment != InstallationEnvironment.FreeAir)
            return new FactorResult(Name, FactorStatus.Missing, null,
                "The installation environment is not recognized for K1 applicability.", Source);

        if (context.AmbientAirTemperatureCelsius is not double temperature
            || context.InsulationType is not InsulationType insulation
            || !Table.TryGetValue(insulation, out var insulationTable))
            return new FactorResult(Name, FactorStatus.Missing, null,
                "K1 applies to free-air installations, but the ambient temperature or a supported insulation type is missing.", Source);

        if (!insulationTable.TryGetValue(temperature, out var value))
            return new FactorResult(Name, FactorStatus.OutOfRange, null,
                "The ambient temperature is not present in the documented K1 table; no interpolation or formula was applied.", Source);

        return new FactorResult(Name, FactorStatus.Applied, value,
            $"K1 is {value:0.00} for {insulation} insulation at {temperature:0} °C ambient temperature.", Source);
    }
}
