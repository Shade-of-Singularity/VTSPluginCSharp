using System.Text.Json.Serialization;

namespace VTSPlugin;

/// <summary>
/// Base class for requests with/without a data entry.
/// </summary>
/// Note: we don't just use <see cref="VTSRequest"/> as a base class,
/// to set it as a rule that a template should be used in generic methods.
/// This also aligns with how <see cref="VTSResponseTemplate"/> and its children are implemented.
public abstract class VTSRequestTemplate : VTSPacket;

/// <summary>
/// Request with a data entry.
/// </summary>
/// <typeparam name="TData"></typeparam>
public abstract class VTSRequest<TData> : VTSRequestTemplate where TData : VTSRequestData
{
    /// <inheritdoc/>
    [JsonPropertyName("data")] public required TData Data { get; set; }

    /// <inheritdoc/>
    public override void Reset()
    {
        base.Reset();
        Data.Reset(); // We keep the reference, to allow it to be populated by JsonSerializer and user code.
    }
}

/// <summary>
/// Request without a data entry.
/// </summary>
/// <remarks>
/// Children can annotate the class with <see cref="VTSPrecomputableAttribute"/> to allow precomputing.
/// Example: <see cref="Requests.VTSAPIStateRequest"/>
/// </remarks>
public abstract class VTSRequest : VTSRequestTemplate;
