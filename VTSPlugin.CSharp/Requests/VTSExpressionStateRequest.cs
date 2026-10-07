using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSExpressionStateRequest : VTSRequest<VTSExpressionStateRequestData>
{
    /// <inheritdoc/>
    [JsonPropertyName("messageType")] public override string? MessageType { get; set; } = "ExpressionStateRequest";

    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSExpressionStateRequest Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSExpressionStateRequest Rent(VTSPool pool) => pool.Rent<VTSExpressionStateRequest>(static () => new()
    {
        Data = new()
        {
            ExpressionFile = string.Empty,
            Details = default,
        }
    });
}

public sealed class VTSExpressionStateRequestData : VTSRequestData
{
    [JsonPropertyName("details")] public required bool Details { get; set; }
    [JsonPropertyName("expressionFile")] public required string? ExpressionFile { get; set; }

    public override void Reset()
    {
        base.Reset();
        Details = default;
        ExpressionFile = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = "")
    //{
    //    base.ToString(b, prefix);
    //    AppendLine(b, prefix, Details);
    //    Append(b, prefix, ExpressionFile);
    //    return b;
    //}
}
