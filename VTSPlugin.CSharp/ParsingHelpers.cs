using System.Runtime.CompilerServices;
using System.Text;

namespace VTSPlugin;

/// <summary>
/// Helper methods for parsing fields from <see cref="RequestResult{T}"/> for debugging.
/// </summary>
public static class ParsingHelpers
{
    /// <summary>
    /// Converts input <paramref name="bytes"/> to <see cref="Encoding.UTF8"/> string using <see cref="UTF8Encoding"/>.
    /// </summary>
    /// <param name="bytes">Bytes to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsUTF8String(this ReadOnlyMemory<byte> bytes) => AsUTF8String(bytes.Span);

    /// <summary>
    /// Converts input <paramref name="bytes"/> to <see cref="Encoding.UTF8"/> string using <see cref="UTF8Encoding"/>.
    /// </summary>
    /// <param name="bytes">Bytes to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsUTF8String(this ReadOnlySpan<byte> bytes) => Encoding.UTF8.GetString(bytes);
}
