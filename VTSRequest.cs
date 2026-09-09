using System.Text.Json.Serialization;

namespace VTSPlugin;

// Note: Maybe use pool TData and Rent/Return it on Json deserialization/disposal?
// September 8th: On it.
public class VTSRequestTemplate : VTSPacket
{
    /// <summary>
    /// API Name that is used in <see cref="APIName"/> unless user overwrites it.
    /// </summary>
    protected const string DefaultAPIName = "VTubeStudioPublicAPI";
    /// <summary>
    /// API Version to use in <see cref="APIVersion"/> unless user overwrites it.
    /// </summary>
    protected const string DefaultAPIVersion = "1.0";

    [JsonPropertyName("apiName")] public override string? APIName { get; set; } = DefaultAPIName;
    [JsonPropertyName("apiVersion")] public override string? APIVersion { get; set; } = DefaultAPIVersion;
}

public abstract class VTSRequest<TData> : VTSRequestTemplate where TData : VTSRequestData
{
    [JsonPropertyName("data")] public virtual required TData Data { get; set; }

    public override void Reset()
    {
        base.Reset();
        Data.Reset(); // Allows data to be reused by JsonSerializer and user code.
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    base.ToString(b, prefix).AppendLine();
    //    AppendData(b, prefix, Data);
    //    return b;
    //}
}

/// <summary>
/// Request without a data entry.
/// </summary>
/// <remarks>
/// Non-abstract only to be able to create <see cref="VTSAPI.DummyRequest"/>.
/// </remarks>
public class VTSRequest : VTSRequestTemplate;
public abstract class VTSRequestData : VTSPacketData
{
    //public override StringBuilder ToString(StringBuilder b, string prefix = "") => b;
}
