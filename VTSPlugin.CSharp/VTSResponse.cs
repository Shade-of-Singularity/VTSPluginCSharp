using System.Text.Json.Serialization;

namespace VTSPlugin;

/// <summary>
/// Base class for all response packet types.
/// </summary>
public abstract class VTSResponseTemplate : VTSPacket
{
    /// <summary>
    /// Time (UNIX millisecond) at which VTube Studio processed your request.
    /// </summary>
    [JsonPropertyName("timestamp")] public long Timestamp { get; set; }
}

/// <summary>
/// Base class for all response packets, which can hold a specific <typeparamref name="TData"/> with them.
/// </summary>
public abstract class VTSResponse<TData> : VTSResponseTemplate where TData : VTSResponseData
{
    /// <summary>
    /// Data this response holds.
    /// </summary>
    [JsonPropertyName("data")] public TData? Data { get; set; }

    /// <inheritdoc/>
    public override void Reset()
    {
        base.Reset();
        // We keep the reference to allow JsonSerializer to populate the existing value.
        Data?.Reset();
    }
}

/// <summary>
/// Base class for all response packets without any data.
/// </summary>
public abstract class VTSResponse : VTSResponseTemplate;
//{
//    /// <summary>
//    /// Data this response holds.
//    /// </summary>
//    [JsonPropertyName("data")] public VTSResponseData? Data { get; set; }

//    public override void Reset()
//    {
//        base.Reset();
//        // We keep the reference to allow JsonSerializer to populate the existing value.
//        Data?.Reset();
//    }
//}
