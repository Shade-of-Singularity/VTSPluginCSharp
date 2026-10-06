namespace VTSPlugin;

/// <summary>
/// Interface for handling periodic updates (ticks) from <see cref="ITickService"/>
/// </summary>
public interface ITickHandler
{
    /// <summary>
    /// Periodic update handler method, produced by <see cref="ITickService"/>.
    /// </summary>
    /// <param name="timeDelta">Imprecise time since last tick.</param>
    void Tick(double timeDelta);
}