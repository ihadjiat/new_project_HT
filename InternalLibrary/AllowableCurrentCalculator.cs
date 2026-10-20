using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;
using HTA.CableSizing.Tables;

namespace HTA.CableSizing;

public sealed class AllowableCurrentCalculator
{
    private readonly IBaseCurrentTable _baseCurrentTable;
    private readonly IReadOnlyList<ICorrectionFactor> _correctionFactors;

    public AllowableCurrentCalculator(
        IBaseCurrentTable? baseCurrentTable = null,
        IEnumerable<ICorrectionFactor>? correctionFactors = null)
    {
        _baseCurrentTable = baseCurrentTable ?? new BaseCurrentTable();
        _correctionFactors = Array.AsReadOnly((correctionFactors ?? CreateDefaultFactors()).ToArray());
    }

    /// <summary>Calculates the product of all registered correction factors.</summary>
    /// <exception cref="ArgumentException">A registered factor returns a negative or non-finite value.</exception>
    public double CalculateGlobalCorrectionFactor(InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var product = 1.0;
        foreach (var factor in _correctionFactors)
        {
            var value = factor.Calculate(context);
            if (!double.IsFinite(value) || value < 0)
            {
                throw new ArgumentException($"Factor {factor.Name} must return a finite, non-negative value.", nameof(context));
            }

            product *= value;
        }

        return product;
    }

    /// <summary>Multiplies the tabulated base current by the product of applicable factors.</summary>
    /// <exception cref="ArgumentException">The cross-section or configuration is not present in the table.</exception>
    public double CalculateAllowableCurrent(
        double crossSectionMm2,
        InstallationMethod installationMethod,
        CableType cableType,
        InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return _baseCurrentTable.GetBaseCurrent(crossSectionMm2, installationMethod, cableType)
            * CalculateGlobalCorrectionFactor(context);
    }

    /// <summary>Returns the smallest tabulated cross-section whose corrected current meets the requested current.</summary>
    /// <exception cref="ArgumentOutOfRangeException">The requested current is invalid or exceeds all available cross-sections.</exception>
    /// <exception cref="ArgumentException">No cross-section is available for the configuration.</exception>
    public double CalculateMinimumCrossSection(
        double desiredCurrentAmps,
        InstallationMethod installationMethod,
        CableType cableType,
        InstallationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (!double.IsFinite(desiredCurrentAmps) || desiredCurrentAmps <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(desiredCurrentAmps), "The requested current must be a finite, strictly positive number.");
        }

        foreach (var crossSection in _baseCurrentTable.GetAvailableCrossSections(installationMethod, cableType).Order())
        {
            if (CalculateAllowableCurrent(crossSection, installationMethod, cableType, context) >= desiredCurrentAmps)
            {
                return crossSection;
            }
        }

        throw new ArgumentOutOfRangeException(nameof(desiredCurrentAmps), desiredCurrentAmps, "No available cross-section can meet the requested current.");
    }

    private static IEnumerable<ICorrectionFactor> CreateDefaultFactors() =>
    [
        new K1CorrectionFactor(),
        new K2CorrectionFactor(),
        new K3CorrectionFactor(),
        new K4CorrectionFactor(),
        new K5CorrectionFactor(),
        new K6CorrectionFactor(),
        new K7CorrectionFactor(),
        new K8CorrectionFactor(),
        new K9CorrectionFactor(),
        new K10CorrectionFactor(),
        new K11CorrectionFactor(),
        new K12CorrectionFactor(),
        new K13CorrectionFactor(),
        new K14CorrectionFactor(),
        new K15CorrectionFactor(),
        new K16CorrectionFactor(),
        new K17CorrectionFactor(),
        new SymmetryFactor()
    ];
}
