using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSEventSubscriptionResponse : VTSResponse<VTSEventSubscriptionResponseData>
{
    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSEventSubscriptionResponse Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSEventSubscriptionResponse Rent(VTSPool pool) => pool.Rent<VTSEventSubscriptionResponse>();
}

public sealed class VTSEventSubscriptionResponseData : VTSResponseData
{
    [JsonPropertyName("subscribedEventCount")] public int SubscribedEventCount { get; set; }
    [JsonPropertyName("subscribedEvents")] public List<string>? SubscribedEvents { get; set; }

    public override void Reset()
    {
        base.Reset();
        SubscribedEventCount = default;
        SubscribedEvents?.Clear();
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    base.ToString(b, prefix);
    //    AppendLine(b, prefix, SubscribedEventCount);
    //    AppendList(b, prefix, SubscribedEvents, VTSHelpers.StringWriter);
    //    return b;
    //}
}
