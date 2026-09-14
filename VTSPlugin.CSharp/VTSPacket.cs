using System.Text.Json.Serialization;

namespace VTSPlugin;

/// <summary>
/// Base class for both request and response packet types.
/// </summary>
public abstract class VTSPacket : VTSPacketTemplate
{
    [JsonPropertyName(VTSAPI.JsonPropertyNames.APIName)] public virtual string? APIName { get; set; }
    [JsonPropertyName(VTSAPI.JsonPropertyNames.APIVersion)] public virtual string? APIVersion { get; set; }
    [JsonPropertyName(VTSAPI.JsonPropertyNames.MessageType)] public virtual string? MessageType { get; set; }
    [JsonPropertyName(VTSAPI.JsonPropertyNames.RequestID)] public virtual string? RequestID { get; set; }

    public override void Reset()
    {
        base.Reset();
        APIName = default;
        APIVersion = default;
        MessageType = default;
        RequestID = default;
    }
}
