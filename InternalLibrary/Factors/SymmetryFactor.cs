using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class SymmetryFactor : ICorrectionFactor
{
    public string Name => "fs";

    public double Calculate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.CablesPerPhase is null)
        {
            return 1.0;
        }

        if (context.CablesPerPhase <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(context), "The number of cables per phase must be strictly positive.");
        }

        if (context.ArrangementType == ArrangementType.Trefoil && context.CablesPerPhase > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(context), "Trefoil arrangement is not supported for more than 4 cables per phase.");
        }

        // TODO: Add missing standard symmetry coefficients; use a neutral temporary value.
        return 1.0;
    }
}
