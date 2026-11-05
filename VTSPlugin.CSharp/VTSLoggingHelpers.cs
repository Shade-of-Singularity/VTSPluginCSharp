using System.Text;
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
            IVTSLogger.Default.LogException(ex, $"[{nameof(VTSLoggingHelpers)}]");
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
            IVTSLogger.Default.LogException(ex, $"[{nameof(VTSLoggingHelpers)}]");
        }
        return type.FullName ?? string.Empty;
    }

    /// <summary>
    /// Formats <paramref name="exception"/> in a better way.
    /// Outputs exception type name, message, and stack trace on a new line.
    /// </summary>
    /// <param name="builder">Builder to write the exception to.</param>
    /// <param name="exception">Exception to format.</param>
    public static void FormatException(StringBuilder builder, Exception exception)
    {
        builder.Append('[');
        builder.Append(exception.GetType().Name);
        builder.Append("] ");
        builder.AppendLine(exception.Message);
        builder.Append(exception.StackTrace);
    }

    /// <summary>
    /// Formats <paramref name="exception"/> in a better way.
    /// Outputs exception type name, message, and stack trace on a new line.
    /// </summary>
    /// <param name="exception">Exception to format.</param>
    public static string FormatException(Exception exception)
    {
        return $"[{exception.GetType().Name}] {exception.Message}\n{exception.StackTrace}";
    }
}