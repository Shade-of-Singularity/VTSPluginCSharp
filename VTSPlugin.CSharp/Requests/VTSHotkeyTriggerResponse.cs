using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSHotkeyTriggerResponse : VTSResponse<VTSHotkeyTriggerResponseData>
{
    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSHotkeyTriggerResponse Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSHotkeyTriggerResponse Rent(VTSPool pool) => pool.Rent<VTSHotkeyTriggerResponse>();
}

public sealed class VTSHotkeyTriggerResponseData : VTSResponseData
{
    [JsonPropertyName("hotkeyID")] public string? HotkeyID { get; set; }

    public override void Reset()
    {
        base.Reset();
        HotkeyID = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    AppendLine(b, prefix, HotkeyID);
    //    return base.ToString(b, prefix);
    //}
}