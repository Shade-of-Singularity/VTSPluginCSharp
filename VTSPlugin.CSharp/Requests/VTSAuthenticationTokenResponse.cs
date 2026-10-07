using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

/// <remarks>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#authentication"/>
/// </remarks>
[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSAuthenticationTokenResponse : VTSResponse<VTSAuthenticationTokenResponseData>
{
    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSAuthenticationTokenResponse Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSAuthenticationTokenResponse Rent(VTSPool pool) => pool.Rent<VTSAuthenticationTokenResponse>();
}

/// <remarks>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#authentication"/>
/// </remarks>
public sealed class VTSAuthenticationTokenResponseData : VTSResponseData
{
    [JsonPropertyName("authenticationToken")] public string? AuthenticationToken { get; set; }

    public override void Reset()
    {
        base.Reset();
        AuthenticationToken = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    AppendLine(b, prefix, AuthenticationToken);
    //    return base.ToString(b, prefix);
    //}
}