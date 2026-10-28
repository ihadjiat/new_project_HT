using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class K13CorrectionFactor : IResultCorrectionFactor
{
    private const string Source = "instruction-metier.md §6.13";

    public string Name => "k13";

    /// <summary>Evaluates K13 using the explicit soil thermal resistivity and cable construction.</summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
        {
            return CreateResult(FactorStatus.Missing, null, "The installation environment is required to evaluate K13.");
        }

        if (context.Environment == InstallationEnvironment.FreeAir)
        {
            return CreateResult(FactorStatus.NotApplicable, 1.0, "K13 applies to buried installations, not free-air installations.");
        }

        if (context.Environment != InstallationEnvironment.Buried)
        {
            return CreateResult(FactorStatus.OutOfRange, null, "The specified installation environment is not recognized for K13.");
        }

        if (context.SoilThermalResistivityKMW is null || context.CableConstruction is null)
        {
            return CreateResult(FactorStatus.Missing, null, "Soil thermal resistivity and cable construction are required for buried K13 evaluation.");
        }

        var resistivity = context.SoilThermalResistivityKMW.Value;
        if (!double.IsFinite(resistivity))
        {
            return CreateResult(FactorStatus.OutOfRange, null, "The soil thermal resistivity must be finite and match a tabulated K13 value.");
        }

        if (!IsTabulatedResistivity(resistivity))
        {
            return CreateResult(FactorStatus.OutOfRange, null, "The soil thermal resistivity is not present in the K13 table.");
        }

        var value = context.CableConstruction switch
        {
            CableConstruction.ThreeSingleCoreCables => resistivity switch
            {
                0.50 => 1.25,
                0.70 => 1.14,
                0.85 => 1.06,
                1.00 => 1.00,
                1.20 => 0.93,
                1.50 => 0.85,
                2.00 => 0.75,
                2.50 => 0.68,
                3.00 => 0.62,
                _ => (double?)null
            },
            CableConstruction.ThreeCoreCable => resistivity switch
            {
                0.50 => 1.20,
                0.70 => 1.10,
                0.85 => 1.05,
                1.00 => 1.00,
                1.20 => 0.95,
                1.50 => 0.88,
                2.00 => 0.79,
                2.50 => 0.72,
                3.00 => 0.68,
                _ => (double?)null
            },
            _ => null
        };

        if (value is null)
        {
            return CreateResult(FactorStatus.OutOfRange, null, "The cable construction is not recognized for K13.");
        }

        return CreateResult(FactorStatus.Applied, value, $"K13 is {value.Value} for a soil thermal resistivity of {resistivity} K·m/W and the specified cable construction.");
    }

    private static bool IsTabulatedResistivity(double resistivity) => resistivity is
        0.50 or 0.70 or 0.85 or 1.00 or 1.20 or 1.50 or 2.00 or 2.50 or 3.00;

    private FactorResult CreateResult(FactorStatus status, double? value, string explanation) =>
        new(Name, status, value, explanation, Source);
}
