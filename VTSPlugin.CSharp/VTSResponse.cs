using System.Text.Json.Serialization;

namespace VTSPlugin;

// TODO: Annotate.
public abstract class VTSResponseTemplate : VTSPacket;
public abstract class VTSResponse<TData> : VTSResponseTemplate where TData : VTSResponseData
{
    [JsonPropertyName("data")] public virtual TData? Data { get; set; }

    public override void Reset()
    {
        base.Reset();
        // We keep the reference to allow JsonSerializer to deserialize values directly into the existing reference.
        Data?.Reset();
    }
}

public class VTSResponse : VTSResponseTemplate
{
    [JsonPropertyName("data")] public virtual VTSResponseData? Data { get; set; }

    public override void Reset()
    {
        base.Reset();
        // We keep the reference to allow JsonSerializer to deserialize values directly into the existing reference.
        Data?.Reset();
    }
}
