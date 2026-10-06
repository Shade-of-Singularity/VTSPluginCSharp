using System.Text.Json.Serialization;

namespace VTSPlugin.Requests;

[VTSPrecomputable]
public sealed class VTSAvailableModelsRequest : VTSRequest
{
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "AvailableModelsRequest";
}
