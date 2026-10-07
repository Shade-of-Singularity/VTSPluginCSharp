using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

/// <summary>
/// Response packet for <see cref="VTSAPIStateRequest"/>.
/// Describes a state of a currently connected VTube Studio app.
/// </summary>
/// <remarks>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#api-details"/>
/// </remarks>
[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSAPIStateResponse : VTSResponse<VTSAPIStateResponseData>
{
    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSAPIStateResponse Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSAPIStateResponse Rent(VTSPool pool) => pool.Rent<VTSAPIStateResponse>();
}

/// <remarks>
/// <para>
/// This class specifically is a data holder for <see cref="VTSAPIStateResponse"/>.
/// </para>
/// <para>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#api-details"/>
/// </para>
/// </remarks>
/// <inheritdoc cref="VTSAPIStateResponse"/>
[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSAPIStateResponseData : VTSResponseData
{
    [JsonPropertyName("active")] public bool Active { get; set; }
    [JsonPropertyName("vTubeStudioVersion")] public string? VTubeStudioVersion { get; set; }
    [JsonPropertyName("currentSessionAuthenticated")] public bool CurrentSessionAuthenticated { get; set; }

    public override void Reset()
    {
        base.Reset();
        Active = default;
        VTubeStudioVersion = default;
        CurrentSessionAuthenticated = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = DefaultPrefix)
    //{
    //    AppendLine(b, prefix, Active);
    //    AppendLine(b, prefix, VTubeStudioVersion);
    //    AppendLine(b, prefix, CurrentSessionAuthenticated);
    //    return base.ToString(b, prefix);
    //}

    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSAPIStateResponseData Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSAPIStateResponseData Rent(VTSPool pool) => pool.Rent<VTSAPIStateResponseData>();
}