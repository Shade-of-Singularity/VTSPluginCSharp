using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSEventSubscriptionRequest : VTSRequest<VTSEventSubscriptionRequestData>
{
    /// <inheritdoc/>
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "EventSubscriptionRequest";

    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSEventSubscriptionRequest Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSEventSubscriptionRequest Rent(VTSPool pool) => pool.Rent<VTSEventSubscriptionRequest>(static () => new()
    {
        Data = new()
        {
            Subscribe = default,
            EventName = string.Empty,
            Config = null,
        }
    });
}
public sealed class VTSEventSubscriptionRequestData : VTSRequestData
{
    [JsonPropertyName("eventName")] public required string? EventName { get; set; }
    [JsonPropertyName("subscribe")] public required bool Subscribe { get; set; }
    [JsonPropertyName("config")] public required VTSEventConfig? Config { get; set; }

    public override void Reset()
    {
        base.Reset();
        EventName = default;
        Subscribe = default;
        Config = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    base.ToString(b, prefix);
    //    AppendLine(b, prefix, EventName);
    //    AppendLine(b, prefix, Subscribe);
    //    Append(b, prefix, Config);
    //    return b;
    //}
}