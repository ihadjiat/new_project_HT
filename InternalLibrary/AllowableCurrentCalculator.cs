using HTA.CableSizing.Enums;
using HTA.CableSizing.Factors;
using HTA.CableSizing.Models;
using HTA.CableSizing.Tables;

namespace HTA.CableSizing;

/// <summary>Calculates correction factors, allowable currents, and minimum cross-sections without throwing business exceptions.</summary>
public sealed class AllowableCurrentCalculator
{
    private const string NoInterpolationAssumption = "Assumption: no interpolation, extrapolation, or premature rounding is applied to tabulated values or factors.";
    private const string FsAssumption = "Assumption: fs is multiplied with the other factors; its relation to the K factors is not confirmed in instruction-metier §9.3.";

    private readonly IReadOnlyList<IResultCorrectionFactor> _correctionFactors;

    /// <summary>Initializes the calculator.</summary>
    /// <param name="correctionFactors">The factors to evaluate, or null to use K1 to K17 and the symmetry factor.</param>
    public AllowableCurrentCalculator(IEnumerable<IResultCorrectionFactor>? correctionFactors = null)
    {
        _correctionFactors = Array.AsReadOnly((correctionFactors ?? CreateDefaultFactors()).ToArray());
    }

    /// <summary>Calculates the product of the evaluated correction factors, following instruction-metier §9.</summary>
    /// <param name="context">The installation context.</param>
    /// <returns>A result whose value is the global factor; Partial when a factor is missing or out of range, Impossible when no factor can contribute.</returns>
    public CalculationResult CalculateGlobalCorrectionFactor(InstallationContext? context)
    {
        if (context is null)
        {
            return Impossible("The installation context is missing.");
        }

        var evaluation = EvaluateFactors(context);
        var status = !evaluation.HasBlockingFactor
            ? CalculationStatus.Complete
            : evaluation.HasContribution ? CalculationStatus.Partial : CalculationStatus.Impossible;

        if (status == CalculationStatus.Impossible)
        {
            return new CalculationResult(status, null, null, null, null, null, null, evaluation.Factors, evaluation.Warnings);
        }

        return new CalculationResult(status, evaluation.Product, null, null, evaluation.Product, null, null, evaluation.Factors, evaluation.Warnings);
    }

    /// <summary>Calculates the corrected allowable current for a tabulated cross-section.</summary>
    /// <param name="crossSectionMm2">The nominal cross-section in square millimeters.</param>
    /// <param name="input">The cable information used to select the reference table.</param>
    /// <param name="context">The installation context; its environment is required.</param>
    /// <returns>A result whose value is the corrected current in amperes (base current × global factor).</returns>
    public CalculationResult CalculateAllowableCurrent(
        double crossSectionMm2,
        ReferenceCurrentSelectionInput? input,
        InstallationContext? context)
    {
        if (!TryPrepare(input, context, out var setup, out var failure))
        {
            return failure!;
        }

        if (!double.IsFinite(crossSectionMm2) || crossSectionMm2 <= 0)
        {
            return Impossible("The cross-section must be a finite, strictly positive number.", setup.Table.TableNumber, setup.SelectionExplanation);
        }

        if (!setup.Table.TryGetBaseCurrent(crossSectionMm2, setup.Environment, setup.Material, setup.Arrangement, out var baseCurrent))
        {
            return Impossible(
                FormattableString.Invariant($"The cross-section {crossSectionMm2:0.###} mm² is not present in table {setup.Table.TableNumber} for this configuration; no interpolation or extrapolation is applied."),
                setup.Table.TableNumber,
                setup.SelectionExplanation);
        }

        var evaluation = EvaluateFactors(context!);
        var corrected = baseCurrent * evaluation.Product;
        var warnings = BuildWarnings(setup, evaluation);
        warnings.Add(FormattableString.Invariant($"Calculation: {crossSectionMm2:0.###} mm², {setup.Environment}, {setup.Material}; base current {baseCurrent:0.###} A × global factor {evaluation.Product:0.####} = {corrected:0.####} A."));

        return new CalculationResult(
            StatusOf(evaluation), corrected, setup.Table.TableNumber, baseCurrent, evaluation.Product, corrected, crossSectionMm2, evaluation.Factors, warnings);
    }

    /// <summary>Finds the smallest tabulated cross-section whose corrected current is at least the desired current.</summary>
    /// <param name="desiredCurrentAmps">The desired current in amperes; must be finite and strictly positive.</param>
    /// <param name="input">The cable information used to select the reference table.</param>
    /// <param name="context">The installation context; its environment is required.</param>
    /// <returns>A result whose value is the selected cross-section in square millimeters; the warnings list the neighboring sections.</returns>
    public CalculationResult CalculateMinimumCrossSection(
        double desiredCurrentAmps,
        ReferenceCurrentSelectionInput? input,
        InstallationContext? context)
    {
        if (!TryPrepare(input, context, out var setup, out var failure))
        {
            return failure!;
        }

        if (!double.IsFinite(desiredCurrentAmps) || desiredCurrentAmps <= 0)
        {
            return Impossible("The desired current must be a finite, strictly positive number.", setup.Table.TableNumber, setup.SelectionExplanation);
        }

        var evaluation = EvaluateFactors(context!);
        var candidates = GetCandidates(setup, evaluation.Product);
        if (candidates.Count == 0)
        {
            return Impossible("No cross-section is available in the selected table for this configuration.", setup.Table.TableNumber, setup.SelectionExplanation);
        }

        var index = candidates.FindIndex(candidate => candidate.CorrectedCurrent >= desiredCurrentAmps);
        if (index < 0)
        {
            var largest = candidates[^1];
            return Impossible(
                FormattableString.Invariant($"No available cross-section meets {desiredCurrentAmps:0.####} A; the largest, {largest.CrossSection:0.###} mm², gives {largest.CorrectedCurrent:0.####} A."),
                setup.Table.TableNumber,
                setup.SelectionExplanation);
        }

        var selected = candidates[index];
        var warnings = BuildWarnings(setup, evaluation);
        warnings.Add(DescribeSelection(candidates, index, desiredCurrentAmps));

        return new CalculationResult(
            StatusOf(evaluation), selected.CrossSection, setup.Table.TableNumber, selected.BaseCurrent, evaluation.Product, selected.CorrectedCurrent, selected.CrossSection, evaluation.Factors, warnings);
    }

    private static CalculationStatus StatusOf(FactorEvaluation evaluation) =>
        evaluation.HasBlockingFactor ? CalculationStatus.Partial : CalculationStatus.Complete;

    private static List<Candidate> GetCandidates(Setup setup, double globalFactor)
    {
        var candidates = new List<Candidate>();
        foreach (var crossSection in setup.Table.GetAvailableCrossSections(setup.Environment, setup.Material, setup.Arrangement).Order())
        {
            if (setup.Table.TryGetBaseCurrent(crossSection, setup.Environment, setup.Material, setup.Arrangement, out var baseCurrent))
            {
                candidates.Add(new Candidate(crossSection, baseCurrent, baseCurrent * globalFactor));
            }
        }

        return candidates;
    }

    private static string DescribeSelection(List<Candidate> candidates, int index, double desiredCurrentAmps)
    {
        var selected = candidates[index];
        var lower = index > 0 ? Describe(candidates[index - 1]) : "none";
        var upper = index < candidates.Count - 1 ? Describe(candidates[index + 1]) : "none";
        return FormattableString.Invariant($"Selected cross-section {selected.CrossSection:0.###} mm² ({selected.CorrectedCurrent:0.####} A) for {desiredCurrentAmps:0.####} A. Lower neighbor: {lower}. Upper neighbor: {upper}.");
    }

    private static string Describe(Candidate candidate) =>
        FormattableString.Invariant($"{candidate.CrossSection:0.###} mm² ({candidate.CorrectedCurrent:0.####} A)");

    private static List<string> BuildWarnings(Setup setup, FactorEvaluation evaluation)
    {
        var warnings = new List<string>
        {
            $"Reference table {setup.Table.TableNumber}: {setup.SelectionExplanation}",
            NoInterpolationAssumption
        };
        warnings.AddRange(evaluation.Warnings);
        return warnings;
    }

    private static bool TryPrepare(
        ReferenceCurrentSelectionInput? input,
        InstallationContext? context,
        out Setup setup,
        out CalculationResult? failure)
    {
        setup = default!;
        failure = null;

        if (input is null || context is null)
        {
            failure = Impossible(input is null ? "The reference current selection input is missing." : "The installation context is missing.");
            return false;
        }

        var effectiveInput = WithArrangement(input, context);
        var selection = ReferenceCurrentTableSelector.Select(effectiveInput);
        if (selection.Status != ReferenceCurrentTableSelectionStatus.Selected || selection.Table is null)
        {
            failure = Impossible(selection.Explanation);
            return false;
        }

        if (context.Environment is not InstallationEnvironment environment)
        {
            failure = Impossible("The installation environment (free air or buried) is required to read the reference current.", selection.Table.TableNumber, selection.Explanation);
            return false;
        }

        setup = new Setup(selection.Table, environment, effectiveInput.ConductorMaterial!.Value, effectiveInput.ArrangementType, selection.Explanation);
        return true;
    }

    private static ReferenceCurrentSelectionInput WithArrangement(ReferenceCurrentSelectionInput input, InstallationContext context)
    {
        if (input.ArrangementType is not null || context.ArrangementType == ArrangementType.Unspecified)
        {
            return input;
        }

        return new ReferenceCurrentSelectionInput(
            input.CableConstruction,
            input.ElectricFieldType,
            input.InsulationType,
            input.ConductorMaterial,
            input.AssignedVoltage,
            context.ArrangementType);
    }

    private FactorEvaluation EvaluateFactors(InstallationContext context)
    {
        var results = _correctionFactors.Select(factor => EvaluateSafely(factor, context)).ToList();
        var warnings = new List<string>();
        var support = results.Where(result => IsSupportFactor(result) && IsUsable(result)).ToList();
        var supportResolved = support.Count == 1;
        var supportAmbiguous = support.Count > 1;

        var product = 1.0;
        var blocking = false;
        var contributed = false;

        foreach (var result in results)
        {
            if (supportAmbiguous && support.Contains(result))
            {
                blocking = true;
                warnings.Add($"{result.Name} is applicable together with another K3 to K6 factor; K7 combines with only one of them (§9), so none is applied and the choice is ambiguous.");
                continue;
            }

            switch (result.Status)
            {
                case FactorStatus.Applied when IsUsable(result):
                    product *= result.Value!.Value;
                    contributed = true;
                    break;
                case FactorStatus.NotApplicable:
                    contributed = true;
                    break;
                case FactorStatus.Missing when supportResolved && IsSupportFactor(result):
                    warnings.Add($"{result.Name} is missing but ignored as an alternative support: {support[0].Name} was selected.");
                    break;
                default:
                    blocking = true;
                    warnings.Add($"{result.Name} is {DescribeProblem(result)}: {result.Explanation} It is not replaced by 1.0.");
                    break;
            }
        }

        blocking |= AddCombinationWarnings(results, support, supportAmbiguous, warnings);
        blocking |= AddSunlightWarning(context, results, warnings);

        return new FactorEvaluation(results, product, blocking, contributed, warnings);
    }

    private static bool AddCombinationWarnings(List<FactorResult> results, List<FactorResult> support, bool supportAmbiguous, List<string> warnings)
    {
        var blocking = false;
        if (results.Any(result => IsName(result, "k7") && IsUsable(result)) && support.Count == 0 && !supportAmbiguous)
        {
            blocking = true;
            warnings.Add("K7 must be combined with exactly one of K3, K4, K5, or K6 (§9.3), but none of them is applied.");
        }

        if (results.Any(result => IsName(result, "fs") && IsUsable(result)))
        {
            warnings.Add(FsAssumption);
        }

        return blocking;
    }

    private static bool AddSunlightWarning(InstallationContext context, List<FactorResult> results, List<string> warnings)
    {
        if (context.IsExposedToDirectSunlight != true)
        {
            return false;
        }

        var k2 = results.FirstOrDefault(result => IsName(result, "k2"));
        if (k2 is not null && k2.Status is FactorStatus.Missing or FactorStatus.OutOfRange)
        {
            return false;
        }

        warnings.Add("The installation is exposed to direct sunlight but K2 is not documented, so it cannot be applied.");
        return true;
    }

    private static FactorResult EvaluateSafely(IResultCorrectionFactor factor, InstallationContext context)
    {
        var name = factor.GetType().Name;
        try
        {
            return factor.Evaluate(context)
                ?? new FactorResult(name, FactorStatus.Missing, null, "The factor returned no result.", null);
        }
        catch (Exception exception)
        {
            return new FactorResult(name, FactorStatus.Missing, null, $"The factor could not be evaluated ({exception.GetType().Name}).", null);
        }
    }

    private static bool IsUsable(FactorResult result) =>
        result.Status == FactorStatus.Applied
        && result.Value is double value
        && double.IsFinite(value)
        && value >= 0;

    private static bool IsName(FactorResult result, string name) =>
        string.Equals(result.Name, name, StringComparison.OrdinalIgnoreCase);

    private static bool IsSupportFactor(FactorResult result) =>
        IsName(result, "k3") || IsName(result, "k4") || IsName(result, "k5") || IsName(result, "k6");

    private static string DescribeProblem(FactorResult result) => result.Status switch
    {
        FactorStatus.Missing => "missing",
        FactorStatus.OutOfRange => "out of range",
        _ => "invalid"
    };

    private static CalculationResult Impossible(string explanation, string? referenceTable = null, string? selectionExplanation = null)
    {
        var warnings = new List<string> { explanation };
        if (selectionExplanation is not null)
        {
            warnings.Add($"Reference table {referenceTable}: {selectionExplanation}");
        }

        return new CalculationResult(CalculationStatus.Impossible, null, referenceTable, null, null, null, null, null, warnings);
    }

    private static IEnumerable<IResultCorrectionFactor> CreateDefaultFactors() =>
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

    private sealed record Setup(
        IReferenceCurrentTable Table,
        InstallationEnvironment Environment,
        ConductorMaterial Material,
        ArrangementType? Arrangement,
        string SelectionExplanation);

    private sealed record Candidate(double CrossSection, double BaseCurrent, double CorrectedCurrent);

    private sealed record FactorEvaluation(
        IReadOnlyList<FactorResult> Factors,
        double Product,
        bool HasBlockingFactor,
        bool HasContribution,
        IReadOnlyList<string> Warnings);
}
