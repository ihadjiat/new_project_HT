namespace HTA.CableSizing.Models;

/// <summary>
/// Represents the result and explanation of a correction factor.
/// </summary>
public sealed class FactorResult
{
    /// <summary>
    /// Initializes a correction factor result.
    /// </summary>
    /// <param name="name">The factor name.</param>
    /// <param name="status">The factor status.</param>
    /// <param name="value">The calculated factor value, if available.</param>
    /// <param name="explanation">The explanation of applicability, value, or missing data.</param>
    /// <param name="source">The business table or article, if known.</param>
    public FactorResult(string name, FactorStatus status, double? value, string explanation, string? source)
    {
        Name = name;
        Status = status;
        Value = value;
        Explanation = explanation;
        Source = source;
    }

    /// <summary>
    /// Gets the factor name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the factor status.
    /// </summary>
    public FactorStatus Status { get; }

    /// <summary>
    /// Gets the calculated factor value, or null when no value is available.
    /// </summary>
    public double? Value { get; }

    /// <summary>
    /// Gets the explanation of applicability, value, or missing data.
    /// </summary>
    public string Explanation { get; }

    /// <summary>
    /// Gets the business table or article source, if known.
    /// </summary>
    public string? Source { get; }
}
