using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSDiscoveryResponse : VTSResponse<VTSDiscoveryResponseData>
{
    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSDiscoveryResponse Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSDiscoveryResponse Rent(VTSPool pool) => pool.Rent<VTSDiscoveryResponse>();
}

public sealed class VTSDiscoveryResponseData : VTSResponseData
{
    [JsonPropertyName("active")] public bool Active { get; set; }
    [JsonPropertyName("port")] public ushort Port { get; set; }
    [JsonPropertyName("instanceID")] public string? InstanceID { get; set; }
    [JsonPropertyName("windowTitle")] public string? WindowTitle { get; set; }

    public override void Reset()
    {
        base.Reset();
        Active = default;
        Port = default;
        InstanceID = default;
        WindowTitle = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    AppendLine(b, prefix, Active);
    //    AppendLine(b, prefix, Port);
    //    AppendLine(b, prefix, InstanceID);
    //    AppendLine(b, prefix, WindowTitle);
    //    return base.ToString(b, prefix);
    //}
}
