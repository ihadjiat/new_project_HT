namespace HTA.CableSizing.Models;

/// <summary>Represents an assigned voltage using U₀, U, and Uₘ values expressed in kilovolts.</summary>
public sealed class AssignedVoltage
{
    /// <summary>Initializes an assigned voltage.</summary>
    /// <param name="u0Kilovolts">The phase-to-earth voltage U₀ in kilovolts.</param>
    /// <param name="uKilovolts">The phase-to-phase voltage U in kilovolts.</param>
    /// <param name="umKilovolts">The maximum system voltage Uₘ in kilovolts.</param>
    public AssignedVoltage(double u0Kilovolts, double uKilovolts, double umKilovolts)
    {
        U0Kilovolts = u0Kilovolts;
        UKilovolts = uKilovolts;
        UmKilovolts = umKilovolts;
    }

    /// <summary>Gets the phase-to-earth voltage U₀ in kilovolts.</summary>
    public double U0Kilovolts { get; }

    /// <summary>Gets the phase-to-phase voltage U in kilovolts.</summary>
    public double UKilovolts { get; }

    /// <summary>Gets the maximum system voltage Uₘ in kilovolts.</summary>
    public double UmKilovolts { get; }
}
