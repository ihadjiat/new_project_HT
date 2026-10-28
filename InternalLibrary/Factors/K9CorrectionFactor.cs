using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class K9CorrectionFactor : IResultCorrectionFactor
{
    private const string Source = "instruction-metier.md §6.9";

    public string Name => "k9";

    /// <summary>
    /// Evaluates K9 for enclosed cables using the documented table values.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The installation environment is required to evaluate k9.", Source);
        }

        if (context.Environment == InstallationEnvironment.Buried)
        {
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0, "K9 does not apply to buried installations.", Source);
        }

        if (context.Environment != InstallationEnvironment.FreeAir)
        {
            return new FactorResult(Name, FactorStatus.OutOfRange, null, "The installation environment is not supported by k9.", Source);
        }

        if (context.AreCablesEnclosed is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The enclosed-cable indicator is required for a free-air installation.", Source);
        }

        if (context.AreCablesEnclosed == false)
        {
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0, "K9 does not apply when cables are not enclosed.", Source);
        }

        if (context.EnclosedCableOrGroupCount is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The number of enclosed cables or groups is required.", Source);
        }

        var value = context.EnclosedCableOrGroupCount switch
        {
            1 => 0.80,
            2 => 0.70,
            3 => 0.65,
            4 => 0.60,
            5 => 0.55,
            6 => 0.55,
            7 => 0.50,
            8 => 0.50,
            9 => 0.45,
            12 => 0.40,
            16 => 0.40,
            20 => 0.40,
            _ => (double?)null
        };

        if (value is null)
        {
            return new FactorResult(Name, FactorStatus.OutOfRange, null, "The number of enclosed cables or groups is not tabulated for k9.", Source);
        }

        return new FactorResult(Name, FactorStatus.Applied, value, "The number of enclosed cables or groups matches a documented k9 table entry.", Source);
    }
}
