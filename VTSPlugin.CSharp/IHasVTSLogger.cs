namespace VTSPlugin;

/// <summary>
/// Describes any object which has <see cref="IVTSLogger"/> attached to them.
/// </summary>
public interface IHasVTSLogger
{
    /// <summary>
    /// Logger to use for any logs and errors.
    /// </summary>
    /// <remarks>
    /// If not provided - returns <see cref="IVTSLogger.Default"/>.
    /// </remarks>
    public IVTSLogger Logger { get; }
}
