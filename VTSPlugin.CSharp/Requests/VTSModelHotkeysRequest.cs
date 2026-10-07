using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSModelHotkeysRequest : VTSRequest<VTSModelHotkeysRequestData>
{
    /// <inheritdoc/>
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "HotkeysInCurrentModelRequest";

    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSModelHotkeysRequest Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSModelHotkeysRequest Rent(VTSPool pool) => pool.Rent<VTSModelHotkeysRequest>(static () => new()
    {
        Data = new()
        {
            ModelID = string.Empty,
            Live2DItemFileName = string.Empty,
        }
    });
}

public sealed class VTSModelHotkeysRequestData : VTSRequestData
{
    [JsonPropertyName("modelID")] public required string? ModelID { get; set; }
    [JsonPropertyName("live2DItemFileName")] public required string? Live2DItemFileName { get; set; }

    public override void Reset()
    {
        base.Reset();
        ModelID = default;
        Live2DItemFileName = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    base.ToString(b, prefix);
    //    AppendLine(b, prefix, ModelID);
    //    Append(b, prefix, Live2DItemFileName);
    //    return b;
    //}
}