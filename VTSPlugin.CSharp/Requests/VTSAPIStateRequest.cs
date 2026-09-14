using System.Text.Json.Serialization;
using VTSPlugin.Legacy;

namespace VTSPlugin.Requests;

public sealed class VTSAPIStateRequest : VTSRequest
{
    public static readonly VTSAPIStateRequest Instance = new();
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "APIStateRequest";
}
