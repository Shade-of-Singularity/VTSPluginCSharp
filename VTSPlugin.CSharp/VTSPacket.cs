using System.Text.Json.Serialization;
using VTSPlugin;
using VTSPlugin.Legacy;

namespace VTSPlugin;

public abstract class VTSPacket : VTSPacketData // Inherits associated functions.
{
    [JsonPropertyName(VTSPackets.APINameJsonPropertyName)] public virtual string? APIName { get; set; }
    [JsonPropertyName(VTSPackets.APIVersionJsonPropertyName)] public virtual string? APIVersion { get; set; }
    [JsonPropertyName(VTSPackets.MessageTypeJsonPropertyName)] public virtual string? MessageType { get; set; }
    [JsonPropertyName(VTSPackets.RequestIDJsonPropertyName)] public virtual string? RequestID { get; set; }

    public override void Reset()
    {
        base.Reset();
        APIName = default;
        APIVersion = default;
        MessageType = default;
        RequestID = default;
    }
}
