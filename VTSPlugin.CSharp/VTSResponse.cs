using System.Text.Json.Serialization;

namespace VTSPlugin;

// TODO: Annotate.
public abstract class VTSResponseTemplate : VTSPacket
{
    /// <summary>
    /// Time (UNIX millisecond) at which VTube Studio processed your request.
    /// </summary>
    [JsonPropertyName("timestamp")] public long Timestamp { get; set; }
}

public abstract class VTSResponse<TData> : VTSResponseTemplate where TData : VTSResponseData
{
    /// <summary>
    /// Data this response holds.
    /// </summary>
    [JsonPropertyName("data")] public TData? Data { get; set; }

    public override void Reset()
    {
        base.Reset();
        // We keep the reference to allow JsonSerializer to deserialize values directly into the existing reference.
        Data?.Reset();
    }
}

public class VTSResponse : VTSResponseTemplate
{
    /// <summary>
    /// Data this response holds.
    /// </summary>
    [JsonPropertyName("data")] public VTSResponseData? Data { get; set; }

    public override void Reset()
    {
        base.Reset();
        // We keep the reference to allow JsonSerializer to deserialize values directly into the existing reference.
        Data?.Reset();
    }
}
