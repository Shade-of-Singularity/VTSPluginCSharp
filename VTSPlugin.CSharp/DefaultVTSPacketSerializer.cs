namespace VTSPlugin;

/// <summary>
/// Seals implementation of <see cref="AbstractVTSPacketSerializer"/>.
/// Sealed for performance reasons.
/// Provides no functionality besides sealing <see cref="AbstractVTSPacketSerializer"/>.
/// </summary>
/// <remarks>
/// To modify its functionality, inherit <see cref="AbstractVTSPacketSerializer"/> instead.
/// </remarks>
public sealed class DefaultVTSPacketSerializer : AbstractVTSPacketSerializer
{
    /// <summary>
    /// Default instance of serializer to use.
    /// </summary>
    /// Note: Referencing this field directly allows JIT to devirtualize calls to <see cref="IVTSPacketSerializer.Default"/>.

    public static readonly DefaultVTSPacketSerializer Instance = new();
    /// <summary>
    /// Hidden private .ctor.
    /// </summary>
    /// <remarks>
    /// <see cref="Instance"/> should be used instead of creating a new one.
    /// </remarks>
    private DefaultVTSPacketSerializer() { }
    /// <inheritdoc/>
    public override string ToString() => $"[{nameof(DefaultVTSPacketSerializer)}]";
}
