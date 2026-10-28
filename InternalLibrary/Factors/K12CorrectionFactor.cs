using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;
using System.Collections.Generic;

namespace HTA.CableSizing.Factors;

public sealed class K12CorrectionFactor : IResultCorrectionFactor
{
    public string Name => "k12";

    private const string Source = "instruction-metier.md §6.12";

    private static readonly Dictionary<InsulationType, Dictionary<double, double>> Table = new()
    {
        [InsulationType.Pvc] = new()
        {
            [0] = 1.18, [5] = 1.14, [10] = 1.10, [15] = 1.05, [20] = 1.00,
            [25] = 0.95, [30] = 0.89, [35] = 0.84, [40] = 0.77, [45] = 0.71
        },
        [InsulationType.PrEpr] = new()
        {
            [0] = 1.13, [5] = 1.10, [10] = 1.07, [15] = 1.04, [20] = 1.00,
            [25] = 0.96, [30] = 0.93, [35] = 0.89, [40] = 0.85, [45] = 0.80
        }
    };

    /// <summary>
    /// Evaluates K12 using
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
            return new FactorResult(Name, FactorStatus.Missing, null,
                "The installation environment is required to determine whether K12 applies.", Source);

        if (context.Environment == InstallationEnvironment.FreeAir)
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0,
                "K12 does not apply to a free-air installation.", Source);

        if (context.Environment != InstallationEnvironment.Buried)
            return new FactorResult(Name, FactorStatus.Missing, null,
                "The installation environment is not recognized for K12 applicability.", Source);

        if (context.SoilTemperatureCelsius is not double temperature
            || context.InsulationType is not InsulationType insulation
            || !Table.TryGetValue(insulation, out var insulationTable))
            return new FactorResult(Name, FactorStatus.Missing, null,
                "K12 applies to buried installations, but the soil temperature or a supported insulation type is missing.", Source);

        if (!insulationTable.TryGetValue(temperature, out var value))
            return new FactorResult(Name, FactorStatus.OutOfRange, null,
                "The soil temperature is not present in the documented K12 table; no interpolation or formula was applied.", Source);

        return new FactorResult(Name, FactorStatus.Applied, value,
            $"K12 is {value:0.00} for {insulation} insulation at {temperature:0} °C soil temperature.", Source);
    }
}
