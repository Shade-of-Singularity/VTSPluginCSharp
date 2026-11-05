namespace VTSPlugin;

/// <summary>
/// Event handler for periodic updates produced by <see cref="IVTSTickService"/>.
/// </summary>
/// <param name="service">Service instance which produced this tick. Can be used to identify the source of a tick.</param>
/// <param name="timeDelta">Imprecise time in seconds since the last tick.</param>
public delegate void TickServiceEventHandler(IVTSTickService service, double timeDelta);

/// <summary>
/// Service, responsible for ticking/periodically updating <see cref="IVTSTickHandler"/>s.
/// </summary>
public interface IVTSTickService : IVTSSubSystem<IVTSTickServiceInitializationOptions>, IHasVTSLogger
{
    /// <summary>
    /// Periodic tick event for this tick service.
    /// </summary>
    event TickServiceEventHandler? Tick;
    /// <summary>
    /// Interval between ticks.
    /// </summary>
    TimeSpan TickInterval { get; set; }
    /// <summary>
    /// Registers <paramref name="handler"/> to handle periodic updates (ticks).
    /// </summary>
    bool Register(IVTSTickHandler handler);
    /// <summary>
    /// Removes <paramref name="handler"/> from receiving periodic updates (ticks).
    /// </summary>
    bool Unregister(IVTSTickHandler handler);
}