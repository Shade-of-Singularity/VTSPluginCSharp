using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

/// <summary>
/// Request for authenticating VTS plugin from a previous session.
/// To start a new session, first request new token using <see cref="VTSAuthenticationTokenRequest"/>.
/// </summary>
/// <remarks>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#authentication"/>
/// </remarks>
[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSAuthenticationRequest : VTSRequest<VTSAuthenticationRequestData>
{
    /// <inheritdoc/>
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "AuthenticationRequest";

    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSAuthenticationRequest Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSAuthenticationRequest Rent(VTSPool pool) => pool.Rent<VTSAuthenticationRequest>(static () => new()
    {
        Data = new()
        {
            PluginName = string.Empty,
            PluginDeveloper = string.Empty,
            AuthenticationToken = string.Empty,
        }
    });
}

/// <remarks>
/// <para>
/// This class specifically is a data holder for <see cref="VTSAuthenticationRequest"/>.
/// </para>
/// <para>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#authentication"/>
/// </para>
/// </remarks>
/// <inheritdoc cref="VTSAuthenticationRequest"/>
public sealed class VTSAuthenticationRequestData : VTSRequestData
{
    /// <summary>
    /// Display name of the plugin.
    /// </summary>
    [JsonPropertyName("pluginName")] public required string? PluginName { get; set; }
    /// <summary>
    /// Developers working on this plugin. Shown together with <see cref="PluginName"/>.
    /// </summary>
    [JsonPropertyName("pluginDeveloper")] public required string? PluginDeveloper { get; set; }
    /// <summary>
    /// Token, used for authenticating current session.
    /// </summary>
    [JsonPropertyName("authenticationToken")] public required string? AuthenticationToken { get; set; }

    public override void Reset()
    {
        base.Reset();
        PluginName = default;
        PluginDeveloper = default;
        AuthenticationToken = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    base.ToString(b, prefix);
    //    AppendLine(b, prefix, PluginName);
    //    AppendLine(b, prefix, PluginDeveloper);
    //    Append(b, prefix, AuthenticationToken);
    //    return b;
    //}
}