namespace VTSPlugin;

/// <summary>
/// Event handler for periodic updates produced by <see cref="ITickService"/>.
/// </summary>
/// <param name="service">Service instance which produced this tick. Can be used to identify the source of a tick.</param>
/// <param name="timeDelta">Imprecise time in seconds since the last tick.</param>
public delegate void TickServiceEventHandler(ITickService service, double timeDelta);

/// <summary>
/// Service, responsible for ticking/periodically updating <see cref="ITickHandler"/>s.
/// </summary>
public interface ITickService : IService<ITickServiceInitializationOptions>, IHasVTSLogger
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
    bool Register(ITickHandler handler);
    /// <summary>
    /// Removes <paramref name="handler"/> from receiving periodic updates (ticks).
    /// </summary>
    bool Unregister(ITickHandler handler);
}