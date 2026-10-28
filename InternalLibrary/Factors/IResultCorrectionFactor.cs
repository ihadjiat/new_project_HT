using HTA.CableSizing.Models;

namespace HTA.CableSizing.Factors;

/// <summary>
/// Defines a correction factor that returns a structured result.
/// </summary>
public interface IResultCorrectionFactor
{
    /// <summary>
    /// Evaluates the correction factor for the specified installation context.
    /// </summary>
    FactorResult Evaluate(InstallationContext context);
}
