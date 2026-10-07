using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

/// <remarks>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#authentication"/>
/// </remarks>
[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSAuthenticationTokenRequest : VTSRequest<VTSAuthenticationTokenRequestData>
{
    /// <inheritdoc/>
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "AuthenticationTokenRequest";

    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSAuthenticationTokenRequest Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSAuthenticationTokenRequest Rent(VTSPool pool) => pool.Rent<VTSAuthenticationTokenRequest>(static () => new()
    {
        Data = new()
        {
            PluginName = string.Empty,
            PluginIcon = string.Empty,
            PluginDeveloper = string.Empty,
        }
    });
}

/// <remarks>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#authentication"/>
/// </remarks>
public sealed class VTSAuthenticationTokenRequestData : VTSRequestData
{
    [JsonPropertyName("pluginName")] public required string? PluginName { get; set; }
    [JsonPropertyName("pluginDeveloper")] public required string? PluginDeveloper { get; set; }
    [JsonPropertyName("pluginIcon")] public required string? PluginIcon { get; set; }

    public override void Reset()
    {
        base.Reset();
        PluginName = default;
        PluginDeveloper = default;
        PluginIcon = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    base.ToString(b, prefix);
    //    AppendLine(b, prefix, PluginName);
    //    AppendLine(b, prefix, PluginDeveloper);
    //    Append(b, prefix, PluginIcon);
    //    return b;
    //}
}