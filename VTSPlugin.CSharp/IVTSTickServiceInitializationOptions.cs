namespace VTSPlugin;

/// <summary>
/// Initialization options for <see cref="IVTSTickService"/>.
/// </summary>
public interface IVTSTickServiceInitializationOptions : IVTSInitializationOptions
{
    /// <summary>
    /// Logger to log the info to.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="IVTSLogger.Default"/> on <see langword="null"/>.
    /// </remarks>
    IVTSLogger? Logger { get; }
    /// <summary>
    /// Interval between ticks.
    /// </summary>
    TimeSpan TickInterval { get; }
}
