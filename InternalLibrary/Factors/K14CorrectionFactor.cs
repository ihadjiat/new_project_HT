using HTA.CableSizing.Enums;
using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class K14CorrectionFactor : IResultCorrectionFactor
{
    private const string Source = "instruction-metier.md §6.14";

    public string Name => "k14";

    private static readonly (double Depth, double Coefficient)[] Table =
    {
        (0.50, 1.04),
        (0.60, 1.02),
        (0.80, 1.00),
        (1.00, 0.97),
        (1.25, 0.95),
        (1.50, 0.93),
        (1.75, 0.91),
        (2.00, 0.90)
    };

    /// <summary>
    /// Evaluates the laying-depth
    /// </summary>
    public FactorResult Evaluate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Environment is null)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The installation environment is required to evaluate k14.", Source);
        }

        if (context.Environment == InstallationEnvironment.FreeAir)
        {
            return new FactorResult(Name, FactorStatus.NotApplicable, 1.0, "K14 does not apply to free-air installations.", Source);
        }

        if (context.LayingDepthMeters is not double depth)
        {
            return new FactorResult(Name, FactorStatus.Missing, null, "The laying depth is required for buried installations.", Source);
        }

        foreach (var (tableDepth, coefficient) in Table)
        {
            if (depth == tableDepth)
            {
                return new FactorResult(Name, FactorStatus.Applied, coefficient, "The laying depth matches a documented table entry.", Source);
            }
        }

        return new FactorResult(Name, FactorStatus.OutOfRange, null, "The laying depth does not match a tabulated entry.", Source);
    }
}
