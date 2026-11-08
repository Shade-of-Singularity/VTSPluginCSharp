namespace VTSPlugin;

/// <summary>
/// Seals implementation of <see cref="VTSAbstractPacketSerializer"/>.
/// Sealed for performance reasons.
/// Provides no functionality besides sealing <see cref="VTSAbstractPacketSerializer"/>.
/// </summary>
/// <remarks>
/// To modify its functionality, inherit <see cref="VTSAbstractPacketSerializer"/> instead.
/// </remarks>
public sealed class VTSDefaultPacketSerializer : VTSAbstractPacketSerializer
{
    /// <summary>
    /// Default instance of serializer to use.
    /// </summary>
    /// Note: Referencing this field directly allows JIT to devirtualize calls to <see cref="IVTSPacketSerializer.Default"/>.

    public static readonly VTSDefaultPacketSerializer Instance = new();
    /// <summary>
    /// Hidden private .ctor.
    /// </summary>
    /// <remarks>
    /// <see cref="Instance"/> should be used instead of creating a new one.
    /// </remarks>
    private VTSDefaultPacketSerializer() { }
    /// <inheritdoc/>
    public override string ToString() => $"[{nameof(VTSDefaultPacketSerializer)}]";
}
