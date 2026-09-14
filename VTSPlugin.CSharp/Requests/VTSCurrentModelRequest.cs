using System.Text.Json.Serialization;

namespace VTSPlugin.Requests;

public sealed class VTSCurrentModelRequest : VTSRequest
{
    public static readonly VTSCurrentModelRequest Instance = new();
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "CurrentModelRequest";
}