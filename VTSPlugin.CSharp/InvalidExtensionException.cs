using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Thrown when you provide wrong extensions in <see cref="IInitializationOptions"/>.
/// Mainly - in <see cref="IVTSLoggerInitializationOptions"/>.
/// </summary>
[Serializable]
public sealed class InvalidExtensionException : VTSException
{
    /// <inheritdoc/>
    public InvalidExtensionException() : base() { }

    /// <inheritdoc/>
    public InvalidExtensionException(string message) : base(message) { }

    /// <inheritdoc/>
    public InvalidExtensionException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Throws <see cref="InvalidExtensionException"/>, specifying that the extension should start from a '.' (dot).
    /// </summary>
    /// <exception cref="InvalidExtensionException">Exception, specifying that the extension should start from a '.' (dot).</exception>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowMissingDot() => throw new InvalidExtensionException("Extension should start from a '.' (dot)!");
}
