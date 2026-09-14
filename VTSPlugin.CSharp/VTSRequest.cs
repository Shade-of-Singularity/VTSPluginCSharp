using System.Text.Json.Serialization;

namespace VTSPlugin;

/// <summary>
/// Base class for requests with/without a data entry.
/// </summary>
/// Note: we don't just use <see cref="VTSRequest"/> as a base class,
/// to set it as a rule that a template should be used in generic methods.
/// This also aligns with how <see cref="VTSResponseTemplate"/> and its children are implemented.
public class VTSRequestTemplate : VTSPacket
{
    [JsonPropertyName("apiName")] public override string? APIName { get; set; } = VTSAPI.DefaultAPIName;
    [JsonPropertyName("apiVersion")] public override string? APIVersion { get; set; } = VTSAPI.DefaultAPIVersion;
}

/// <summary>
/// Request with a data entry.
/// </summary>
/// <typeparam name="TData"></typeparam>
public abstract class VTSRequest<TData> : VTSRequestTemplate where TData : VTSRequestData
{
    [JsonPropertyName("data")] public virtual required TData Data { get; set; }

    public override void Reset()
    {
        base.Reset();
        Data.Reset(); // Allows data to be reused by JsonSerializer and user code.
    }
}

/// <summary>
/// Request without a data entry.
/// </summary>
public abstract class VTSRequest : VTSRequestTemplate;
