using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSHotkeyTriggerRequest : VTSRequest<VTSHotkeyTriggerRequestData>
{
    /// <inheritdoc/>
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "HotkeyTriggerRequest";

    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSHotkeyTriggerRequest Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSHotkeyTriggerRequest Rent(VTSPool pool) => pool.Rent<VTSHotkeyTriggerRequest>(static () => new()
    {
        Data = new()
        {
            HotkeyID = string.Empty,
            ItemInstanceID = string.Empty,
        }
    });
}

public sealed class VTSHotkeyTriggerRequestData : VTSRequestData
{
    [JsonPropertyName("hotkeyID")] public required string? HotkeyID { get; set; }
    [JsonPropertyName("itemInstanceID")] public required string? ItemInstanceID { get; set; }

    public override void Reset()
    {
        base.Reset();
        HotkeyID = default;
        ItemInstanceID = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    base.ToString(b, prefix);
    //    AppendLine(b, prefix, HotkeyID);
    //    Append(b, prefix, ItemInstanceID);
    //    return b;
    //}
}