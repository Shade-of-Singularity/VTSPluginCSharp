using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Interface for implementing JSON serializers for VTS packets.
/// </summary>
public interface IVTSPacketSerializer : IHasVTSLogger
{
    /// <summary>
    /// Default implementation for <see cref="IVTSPacketSerializer"/>.
    /// </summary>
    public static IVTSPacketSerializer Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => DefaultVTSPacketSerializer.Instance;
    }
    /// <summary>
    /// Whether precomputing should be enabled.
    /// </summary>
    /// <remarks>
    /// Disabling can be useful as a temporary workaround for solving precompute related bugs, if there are any.
    /// </remarks>
    bool EnablePrecompute { get; set; }
    /// <summary>
    /// Configuration for packet precomputing (for those that support it).
    /// </summary>
    VTSPrecomputeConfiguration PrecomputeConfiguration { get; set; }
    /// <summary>
    /// Serializes packet into UTF8 JSON without precomputing.
    /// Encoding to use: <seealso cref="VTSAPI.Encoding"/>
    /// </summary>
    /// <param name="instance">Instance of <typeparamref name="T"/> to serialize.</param>
    /// <param name="allowPrecompute">
    /// Whether to check if given <typeparamref name="T"/> can be precomputed.
    /// Both this value and <see cref="EnablePrecompute"/> should be <see langword="true"/> for the precompute to happen.
    /// </param>
    /// <returns>Packet, serialized as JSON.</returns>
    string Serialize<T>(T instance, bool allowPrecompute = true) where T : VTSPacketTemplate;
    /// <summary>
    /// Deserializes <paramref name="utf8json"/> packet data as <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// <typeparamref name="T"/> is likely retrieved from <see cref="Pooling.VTSPool.Shared"/>,
    /// so feel free to return it back after use.
    /// </remarks>
    /// <param name="utf8json">Raw JSON packet.</param>
    /// <returns>
    /// <typeparamref name="T"/>, deserialized from provided <paramref name="utf8json"/> JSON data.
    /// </returns>
    T Deserialize<T>(ReadOnlySpan<byte> utf8json) where T : VTSPacketTemplate;
}
