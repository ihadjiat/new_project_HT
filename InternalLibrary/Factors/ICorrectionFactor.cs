using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

public interface ICorrectionFactor
{
    string Name { get; }
    double Calculate(InstallationContext context);
}
