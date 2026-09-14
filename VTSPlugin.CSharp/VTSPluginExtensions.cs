using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Helpful extensions when working with <see cref="IVTSPlugin"/>s.
/// </summary>
public static class VTSPluginExtensions
{
    const long TicksTo100Nanoseconds = 1000;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TicksSinceLastKeepAlive(this IVTSEndPoint ep) => TicksSinceLastKeepAlive(ep, Environment.TickCount);
    public static int TicksSinceLastKeepAlive(this IVTSEndPoint ep, int tickNow)
    {
        int last = ep.LastKeepAliveTick;
        if (last < tickNow)
        {
            return Math.Max(0, tickNow - last);
        }
        else
        {
            return Math.Max(0, int.MaxValue - last - tickNow);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan TimeSinceLastKeepAlive(this IVTSEndPoint ep) => TimeSinceLastKeepAlive(ep, Environment.TickCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan TimeSinceLastKeepAlive(this IVTSEndPoint ep, int tickNow)
    {
        return new TimeSpan(TicksSinceLastKeepAlive(ep, tickNow) * TicksTo100Nanoseconds);
    }
}
