using VTSPlugin.Legacy;

namespace VTSPlugin;

/// <summary>
/// Helpful extensions when working with <see cref="IVTSPlugin"/>s.
/// </summary>
public static class VTSPluginExtensions
{
    const long TicksTo100Nanoseconds = 1000;
    public static long TicksSinceLastKeepAlive(this IVTSEndPoint ep) => TicksSinceLastKeepAlive(ep, Environment.TickCount64);
    public static long TicksSinceLastKeepAlive(this IVTSEndPoint ep, long tickNow)
    {
        tickNow -= ep.LastKeepAliveTick;
        if (tickNow < 0) return 0;
        return tickNow;
    }

    public static TimeSpan TimeSinceLastKeepAlive(this IVTSEndPoint ep) => TimeSinceLastKeepAlive(ep, Environment.TickCount64);
    public static TimeSpan TimeSinceLastKeepAlive(this IVTSEndPoint ep, long tickNow)
    {
        tickNow -= ep.LastKeepAliveTick;
        if (tickNow < 0) return new TimeSpan(0);
        return new TimeSpan(tickNow * TicksTo100Nanoseconds);
    }
}
