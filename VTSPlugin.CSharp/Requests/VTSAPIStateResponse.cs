using System.Text.Json.Serialization;
using VTSPlugin.Legacy;

namespace VTSPlugin.Requests;

public sealed class VTSAPIStateResponse : VTSResponse<VTSAPIStateResponseData>;
public sealed class VTSAPIStateResponseData : VTSResponseData
{
    [JsonPropertyName("active")] public bool Active { get; set; }
    [JsonPropertyName("vTubeStudioVersion")] public string? VTubeStudioVersion { get; set; }
    [JsonPropertyName("currentSessionAuthenticated")] public bool CurrentSessionAuthenticated { get; set; }

    public override void Reset()
    {
        base.Reset();
        Active = default;
        VTubeStudioVersion = default;
        CurrentSessionAuthenticated = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    AppendLine(b, prefix, Active);
    //    AppendLine(b, prefix, VTubeStudioVersion);
    //    AppendLine(b, prefix, CurrentSessionAuthenticated);
    //    return base.ToString(b, prefix);
    //}
}