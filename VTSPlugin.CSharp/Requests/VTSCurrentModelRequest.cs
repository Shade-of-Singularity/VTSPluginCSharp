using System.Text.Json.Serialization;

namespace VTSPlugin.Requests;

[VTSPrecomputable]
public sealed class VTSCurrentModelRequest : VTSRequest
{
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "CurrentModelRequest";
}