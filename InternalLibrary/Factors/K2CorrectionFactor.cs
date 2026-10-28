using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class K2CorrectionFactor : IResultCorrectionFactor
{
    public string Name => "k2";

    /// <summary>Evaluates K2 for direct sunlight exposure using the documented applicability rules.</summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null,
                "The installation environment is missing, so K2 applicability cannot be determined.",
                "instruction-metier.md §6.2");
        }

        return context.Environment switch
        {
            InstallationEnvironment.Buried => new FactorResult(Name, FactorStatus.NotApplicable, 1.0,
                "K2 is not applicable to buried installations.", "instruction-metier.md §6.2"),
            InstallationEnvironment.FreeAir => EvaluateFreeAir(context),
            _ => new FactorResult(Name, FactorStatus.OutOfRange, null,
                "The installation environment is not recognized.", "instruction-metier.md §6.2")
        };
    }

    private FactorResult EvaluateFreeAir(InstallationContext context)
    {
        return context.IsExposedToDirectSunlight switch
        {
            false => new FactorResult(Name, FactorStatus.NotApplicable, 1.0,
                "K2 is not applicable when the installation is not exposed to direct sunlight.",
                "instruction-metier.md §6.2"),
            true => new FactorResult(Name, FactorStatus.Missing, null,
                "K2 is required for direct sunlight exposure, but its value and formula are not documented in instruction-metier.md §6.2, which refers to IEC 60287.",
                "instruction-metier.md §6.2"),
            null => new FactorResult(Name, FactorStatus.Missing, null,
                "Direct sunlight exposure is unknown, so K2 applicability cannot be determined.",
                "instruction-metier.md §6.2")
        };
    }
}
