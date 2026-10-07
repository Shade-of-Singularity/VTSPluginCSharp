using System.Text.Json;
using VTSPlugin.Pooling;

namespace VTSPlugin;

/// <summary>
/// Base class for both <see cref="VTSPacket"/>s and their data entries (<see cref="VTSPacketData"/>)
/// </summary>
public abstract class VTSPacketTemplate : IVTSPoolable
{
    /// <remarks>
    /// By default, serializes the packet using <see cref="JsonSerializer"/> with <see cref="VTSAPI.JsonLoggingOptions"/>.
    /// (Behaviour might be re-implemented via <see cref="System.Text.StringBuilder"/> and manual serialization)
    /// </remarks>
    /// <inheritdoc/>
    public override string ToString() => this.ToLoggingString();

    /// <inheritdoc/>
    public virtual void Reset() { }
}