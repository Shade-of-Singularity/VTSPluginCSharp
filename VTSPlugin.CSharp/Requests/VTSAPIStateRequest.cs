using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

/// <summary>
/// Request for requesting a state of a currently connected VTube Studio app.
/// </summary>
/// <remarks>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#api-details"/>
/// </remarks>
[VTSPrecomputable]
[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSAPIStateRequest : VTSRequest
{
    /// <inheritdoc/>
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "APIStateRequest";

    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSAPIStateRequest Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSAPIStateRequest Rent(VTSPool pool) => pool.Rent<VTSAPIStateRequest>();
}
