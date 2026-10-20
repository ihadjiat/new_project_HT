using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public sealed class K15CorrectionFactor : ICorrectionFactor
{
    public string Name => "k15";

    public double Calculate(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        // TODO: Add missing standard data.
        return 1.0;
    }
}
