using System.Text.Json.Serialization;
using VTSPlugin.Legacy;

namespace VTSPlugin.Requests;

public sealed class VTSAuthenticationTokenResponse : VTSResponse<VTSAuthenticationTokenResponseData>;
public sealed class VTSAuthenticationTokenResponseData : VTSResponseData
{
    [JsonPropertyName("authenticationToken")] public string? AuthenticationToken { get; set; }

    public override void Reset()
    {
        base.Reset();
        AuthenticationToken = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    AppendLine(b, prefix, AuthenticationToken);
    //    return base.ToString(b, prefix);
    //}
}