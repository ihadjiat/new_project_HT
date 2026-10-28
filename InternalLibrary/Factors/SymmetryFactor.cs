using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class SymmetryFactor : IResultCorrectionFactor
{
    private const string Source = "instruction-metier.md §7.2–7.3";

    public string Name => "fs";

    /// <summary>
    /// Evaluates the symmetry factor for the specified installation context.
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.CableConstruction is null)
        {
            return Result(FactorStatus.Missing, null, "Cable construction is required to evaluate the symmetry factor.");
        }

        if (!Enum.IsDefined(context.CableConstruction.Value))
        {
            return Result(FactorStatus.OutOfRange, null, "The cable construction value is not recognized.");
        }

        if (context.CableConstruction == CableConstruction.ThreeCoreCable)
        {
            return Result(FactorStatus.NotApplicable, 1.0, "The symmetry factor does not apply to a three-core cable.");
        }

        if (context.CablesPerPhase is null)
        {
            return Result(FactorStatus.Missing, null, "The number of single-core cables per phase is required to evaluate the symmetry factor.");
        }

        var cablesPerPhase = context.CablesPerPhase.Value;
        if (cablesPerPhase <= 0 || cablesPerPhase > 8)
        {
            return Result(FactorStatus.OutOfRange, null, "The number of single-core cables per phase must be between 1 and 8.");
        }

        if (cablesPerPhase == 1)
        {
            return Result(FactorStatus.NotApplicable, 1.0, "The symmetry factor is not applicable when there is one single-core cable per phase.");
        }

        if (!Enum.IsDefined(context.ArrangementType))
        {
            return Result(FactorStatus.OutOfRange, null, "The cable arrangement value is not recognized.");
        }

        if (cablesPerPhase > 4)
        {
            if (context.ArrangementType == ArrangementType.Trefoil)
            {
                return Result(FactorStatus.OutOfRange, null, "Trefoil arrangements are limited to four cables per phase.");
            }

            return Result(FactorStatus.Missing, null, "No universal symmetry-factor method is documented for this arrangement with five to eight cables per phase.");
        }

        if (context.AreCablePhaseArrangementsSymmetrical is null)
        {
            return Result(FactorStatus.Missing, null, "Whether the cable phase arrangements meet the symmetry conditions is required.");
        }

        if (!context.AreCablePhaseArrangementsSymmetrical.Value)
        {
            return Result(FactorStatus.Applied, 0.8, "The symmetry conditions are not met; the documented symmetry factor is 0.8.");
        }

        if (context.ArrangementType is not (ArrangementType.Trefoil or ArrangementType.Flat))
        {
            return Result(FactorStatus.Missing, null, "A documented trefoil or flat arrangement is required when the symmetry conditions are met.");
        }

        var value = cablesPerPhase switch
        {
            2 => 1.0,
            3 => 0.8,
            4 => 1.0,
            _ => throw new InvalidOperationException("The cable count was validated before evaluating the documented symmetry values.")
        };

        return Result(FactorStatus.Applied, value, $"The documented symmetry factor for {cablesPerPhase} cables per phase in a symmetric {context.ArrangementType} arrangement is {value:0.0}.");
    }

    private FactorResult Result(FactorStatus status, double? value, string explanation) =>
        new(Name, status, value, explanation, Source);
}
