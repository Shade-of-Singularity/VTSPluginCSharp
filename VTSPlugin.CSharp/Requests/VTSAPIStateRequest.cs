using System.Text.Json.Serialization;

namespace VTSPlugin.Requests;

[VTSPrecomputable]
public sealed class VTSAPIStateRequest : VTSRequest
{
    /// <inheritdoc/>
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "APIStateRequest";
}
