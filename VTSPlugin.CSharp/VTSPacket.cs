using System.Text.Json.Serialization;

namespace VTSPlugin;

/// <summary>
/// Base class for both request and response packet types.
/// </summary>
/// Note: fields cannot be init-only, as JSON source generators doesn't allow init-only fields.
/// See issue: https://github.com/dotnet/runtime/issues/58770
/// This, however, means that all static packet instances can be modified. This should be heavily discouraged!
public abstract class VTSPacket : VTSPacketTemplate
{
    /// <summary>
    /// Unique Name for the Application Public Interface.
    /// With VTube Studio, always <see cref="VTSAPI.DefaultAPIName"/>.
    /// </summary>
    [JsonPropertyName(VTSAPI.JsonPropertyNames.APIName)] public string? APIName { get; set; } = VTSAPI.DefaultAPIName;

    /// <summary>
    /// Version of VTube Studio's API.
    /// Not related to VTube Studio application version.
    /// Version will stay the same, until VTube Studio updates something, making the old system incompatible with a new one.
    /// Default value is <see cref="VTSAPI.DefaultAPIVersion"/>.
    /// </summary>
    [JsonPropertyName(VTSAPI.JsonPropertyNames.APIVersion)] public string? APIVersion { get; set; } = VTSAPI.DefaultAPIVersion;

    /// <summary>
    /// Type of the message this packet describes.
    /// </summary>
    [JsonPropertyName(VTSAPI.JsonPropertyNames.MessageType)] public string? MessageType { get; set; }

    /// <summary>
    /// Unique Identifier for a request, produced with this packet.
    /// </summary>
    /// <remarks>
    /// When you don't provide RequestID in your request - VTube Studio will generate a new RequestID in a response.
    /// </remarks>
    [JsonPropertyName(VTSAPI.JsonPropertyNames.RequestID)] public string? RequestID { get; set; }

    /// <inheritdoc/>
    public override void Reset()
    {
        base.Reset();
        APIName = default;
        APIVersion = default;
        MessageType = default;
        RequestID = default;
    }
}
