namespace VTSPlugin;

/// <summary>
/// Base class for both <see cref="VTSPacket"/>s and their data entries (<see cref="VTSPacketData"/>)
/// </summary>
public abstract class VTSPacketTemplate
{
    /// <inheritdoc/>
    public override string ToString() => this.ToLoggingString(GetType());
    /// <summary>
    /// Resets all data about the packet.
    /// </summary>
    /// <remarks>
    /// Used for pooling in <see cref="VTSPacketPool.Return{T}(T)"/>
    /// </remarks>
    public virtual void Reset() { }
}
