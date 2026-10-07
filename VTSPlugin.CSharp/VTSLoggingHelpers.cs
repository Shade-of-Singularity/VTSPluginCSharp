using System.Text.Json;

namespace VTSPlugin;

/// <summary>
/// Helpers for logging things globally.
/// </summary>
public static class VTSLoggingHelpers
{
    /// <summary>
    /// Serializes struct as JSON using <see cref="VTSAPI.JsonLoggingOptions"/>.
    /// If failed - returns full name of the type.
    /// </summary>
    public static string ToLoggingString<T>(this T obj) where T : struct
    {
        try
        {
            return JsonSerializer.Serialize(obj, VTSAPI.JsonLoggingOptions);
        }
        catch (Exception ex)
        {
            if (IVTSLogger.Default.ShouldLogException())
                IVTSLogger.Default.Log(ex, $"[{nameof(VTSLoggingHelpers)}]");
        }
        return typeof(T).FullName ?? string.Empty;
    }

    /// <summary>
    /// Serializes object as JSON using <see cref="VTSAPI.JsonLoggingOptions"/>.
    /// If failed - returns full name of the type.
    /// </summary>
    public static string ToLoggingString(this object? obj)
    {
        if (obj is null) return "null";
        var type = obj.GetType();
        try
        {
            return JsonSerializer.Serialize(obj, type, VTSAPI.JsonLoggingOptions);
        }
        catch (Exception ex)
        {
            if (IVTSLogger.Default.ShouldLogException())
                IVTSLogger.Default.Log(ex, $"[{nameof(VTSLoggingHelpers)}]");
        }
        return type.FullName ?? string.Empty;
    }
}