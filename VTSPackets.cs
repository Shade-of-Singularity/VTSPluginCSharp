using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace VTSPlugin;

/// <summary>
/// TODO: Move to VTSHelpers, to remove ambiguity with <see cref="VTSPacket"/> and <see cref="VTSPacketData"/>.
/// Note: Pooling can be separated in its own class for clarity, like VTSPacketPool.
/// </summary>
public static class VTSPackets
{
    // TODO: Implement using custom ILogger implementation.
    [return: NotNullIfNotNull(nameof(obj))]
    public static string? ToLoggingString<T>(this T? obj, Type? type = null)
    {
        if (obj is null) return null;
        type ??= typeof(T);

        try { return JsonSerializer.Serialize(obj, type, VTSAPI.JsonLoggingOptions); }
        catch (Exception ex) { ex.Out($"Cannot serialize ({typeof(T)})\n"); return string.Empty; }
    }
}
