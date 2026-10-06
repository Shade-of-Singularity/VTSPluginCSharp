namespace VTSPlugin.Pooling;

/// <summary>
/// Describes an item, that can be retrieved from and returned to an <see cref="VTSPool"/>.
/// </summary>
public interface IVTSPoolable
{
    /// <summary>
    /// Resets all data about the instance.
    /// </summary>
    /// <remarks>
    /// Usually used for pooling.
    /// </remarks>
    void Reset();
}
