namespace VTSPlugin;

/// <summary>
/// Interface for handling periodic updates (ticks) from <see cref="IVTSTickService"/>
/// </summary>
public interface IVTSTickHandler
{
    /// <summary>
    /// Periodic update handler method, produced by <see cref="IVTSTickService"/>.
    /// </summary>
    /// <param name="timeDelta">Imprecise time since last tick.</param>
    void Tick(double timeDelta);
}