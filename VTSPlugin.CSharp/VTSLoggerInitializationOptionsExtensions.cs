namespace VTSPlugin;

/// <summary>
/// Contains useful extensions for working with <see cref="IVTSLoggerInitializationOptions"/>, used throughout the library.
/// </summary>
public static class VTSLoggerInitializationOptionsExtensions
{
    /// <summary>
    /// Validates the state of the provided <paramref name="options"/>
    /// </summary>
    /// <typeparam name="T">Tye of the options to validate. Must be a struct.</typeparam>
    /// <param name="options"></param>
    /// <returns>
    /// <see langword="true"/> - when <paramref name="options"/> passed API common validations.
    /// <see langword="false"/> - when <paramref name="options"/> contain issues.
    /// </returns>
    /// Note: implemented as generic, to avoid struct boxing on each call.
    public static bool Validate<T>(this T options) where T : IVTSLoggerInitializationOptions
    {
        return options.LogFileExtension.StartsWith('.');
    }

    /// <summary>
    /// Validates the state of the provided <paramref name="options"/> using assertions.
    /// </summary>
    /// <typeparam name="T">Type of the options to validate. Must be a struct.</typeparam>
    /// <param name="options">Options to validate.</param>
    /// <exception cref="InvalidExtensionException">
    /// When file extension in <paramref name="options"/> does not start from a '.' (dot).
    /// </exception>
    /// Note: implemented as generic, to avoid struct boxing on each call.
    public static void AssertValidate<T>(this T options) where T : IVTSLoggerInitializationOptions
    {
        if (!options.LogFileExtension.StartsWith('.'))
            InvalidExtensionException.ThrowMissingDot();
    }
}
