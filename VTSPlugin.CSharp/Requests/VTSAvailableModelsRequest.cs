using System.Text.Json.Serialization;
using VTSPlugin.Legacy;

namespace VTSPlugin.Requests;

public sealed class VTSAvailableModelsRequest : VTSRequest
{
    public static readonly VTSAvailableModelsRequest Instance = new();
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "AvailableModelsRequest";
}
