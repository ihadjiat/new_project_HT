using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class K11CorrectionFactor : IResultCorrectionFactor
{
    private const string Source = "instruction-metier.md §6.11";

    public string Name => "k11";

    /// <summary>
    /// Evaluates K11 for a cable wound on mobile-equipment drums using the documented tables.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The installation environment is required to evaluate k11.", Source);
        }

        if (context.Environment == InstallationEnvironment.Buried)
        {
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0, "K11 does not apply to buried installations.", Source);
        }

        if (context.Environment != InstallationEnvironment.FreeAir)
        {
            return new FactorResult(Name, FactorStatus.OutOfRange, null, "The installation environment is not supported by k11.", Source);
        }

        if (context.IsCableOnMobileEquipmentDrum is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The mobile-equipment drum indicator is required for a free-air installation.", Source);
        }

        if (context.IsCableOnMobileEquipmentDrum == false)
        {
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0, "K11 does not apply when the cable is not on a mobile-equipment drum.", Source);
        }

        if (context.DrumWindingType is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The drum winding type is required.", Source);
        }

        return context.DrumWindingType switch
        {
            DrumWindingType.SingleLayer => EvaluateSingleLayer(context),
            DrumWindingType.MultipleLayers => EvaluateMultipleLayers(context),
            _ => new FactorResult(Name, FactorStatus.OutOfRange, null, "The drum winding type is not supported by k11.", Source)
        };
    }

    private FactorResult EvaluateSingleLayer(InstallationContext context)
    {
        if (context.DrumTurnsCount is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The number of drum turns is required for a single-layer winding.", Source);
        }

        var value = context.DrumTurnsCount switch
        {
            2 => 0.88,
            3 => 0.80,
            4 => 0.75,
            5 => 0.74,
            6 => 0.73,
            8 => 0.72,
            10 => 0.70,
            12 => 0.69,
            14 => 0.65,
            _ => (double?)null
        };

        return value is null
            ? new FactorResult(Name, FactorStatus.OutOfRange, null, "The number of drum turns is not tabulated for a single-layer winding.", Source)
            : new FactorResult(Name, FactorStatus.Applied, value, "The number of drum turns matches a documented single-layer k11 table entry.", Source);
    }

    private FactorResult EvaluateMultipleLayers(InstallationContext context)
    {
        if (context.DrumCompletedLayerCount is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The number of completed drum layers is required for a multiple-layer winding.", Source);
        }

        var value = context.DrumCompletedLayerCount switch
        {
            1 => 0.80,
            2 => 0.60,
            3 => 0.50,
            > 3 => 0.35,
            _ => (double?)null
        };

        return value is null
            ? new FactorResult(Name, FactorStatus.OutOfRange, null, "The number of completed drum layers must be positive.", Source)
            : new FactorResult(Name, FactorStatus.Applied, value, "The completed drum layers match a documented multiple-layer k11 table category.", Source);
    }
}
