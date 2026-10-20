using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class K14CorrectionFactor : ICorrectionFactor
{
    public string Name => "k14";

    // TODO: verify whether the standard explicitly allows linear interpolation
    // between tabulated values, or imposes another rule (rounding, nearest value...).
    // This is a default assumption pending confirmation.
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

    public double Calculate(InstallationContext context)
    {
        if (context.LayingDepthMeters is not double depth)
        {
            return 1.0; // k14 not applicable in this context
        }

        if (depth <= Table[0].Depth)
        {
            return Table[0].Coefficient;
        }
        if (depth >= Table[^1].Depth)
        {
            return Table[^1].Coefficient;
        }

        for (int i = 0; i < Table.Length - 1; i++)
        {
            var (d1, c1) = Table[i];
            var (d2, c2) = Table[i + 1];

            if (depth >= d1 && depth <= d2)
            {
                double ratio = (depth - d1) / (d2 - d1);
                return c1 + ratio * (c2 - c1);
            }
        }

        throw new InvalidOperationException("Depth out of range for k14 table.");
    }
}
