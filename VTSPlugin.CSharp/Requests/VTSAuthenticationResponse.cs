using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

/// <summary>
/// Response packet for <see cref="VTSAuthenticationRequest"/>.
/// Describes whether session was authenticated or not.
/// If not - provides reason in <see cref="VTSAuthenticationResponseData.Reason"/>, which can be logged or displayed to the user.
/// </summary>
/// <remarks>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#authentication"/>
/// </remarks>
[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSAuthenticationResponse : VTSResponse<VTSAuthenticationResponseData>
{
    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSAuthenticationResponse Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSAuthenticationResponse Rent(VTSPool pool) => pool.Rent<VTSAuthenticationResponse>();
}

/// <remarks>
/// <para>
/// This class specifically is a data holder for <see cref="VTSAuthenticationResponse"/>.
/// </para>
/// <para>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#authentication"/>
/// </para>
/// </remarks>
/// <inheritdoc cref="VTSAuthenticationResponse"/>
public sealed class VTSAuthenticationResponseData : VTSResponseData
{
    [JsonPropertyName("authenticated")] public bool Authenticated { get; set; }
    [JsonPropertyName("reason")] public string? Reason { get; set; }

    public override void Reset()
    {
        base.Reset();
        Authenticated = default;
        Reason = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    AppendLine(b, prefix, Authenticated);
    //    AppendLine(b, prefix, Reason);
    //    return base.ToString(b, prefix);
    //}
}