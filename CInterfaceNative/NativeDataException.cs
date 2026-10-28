namespace CInterfaceNative;

/// <summary>Raised internally when native data is inconsistent (for example a flag or boolean that is neither 0 nor 1). Never crosses the native boundary.</summary>
public sealed class NativeDataException : Exception
{
    /// <summary>Initializes the exception.</summary>
    /// <param name="message">The description of the inconsistency.</param>
    public NativeDataException(string message) : base(message)
    {
    }
}
