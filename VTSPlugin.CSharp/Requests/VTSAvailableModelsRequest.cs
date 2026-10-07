using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

/// <remarks>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#getting-a-list-of-available-vts-models"/>
/// </remarks>
[VTSPrecomputable]
[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSAvailableModelsRequest : VTSRequest
{
    /// <inheritdoc/>
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "AvailableModelsRequest";

    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSAvailableModelsRequest Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSAvailableModelsRequest Rent(VTSPool pool) => pool.Rent<VTSAvailableModelsRequest>();
}
