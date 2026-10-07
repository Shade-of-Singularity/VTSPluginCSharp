using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

/// <remarks>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#getting-a-list-of-available-vts-models"/>
/// </remarks>
[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSAvailableModelsResponse : VTSResponse<VTSAvailableModelsResponseData>
{
    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSAvailableModelsResponse Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSAvailableModelsResponse Rent(VTSPool pool) => pool.Rent<VTSAvailableModelsResponse>();
}

/// <remarks>
/// API guide: <see href="https://github.com/DenchiSoft/VTubeStudio#getting-a-list-of-available-vts-models"/>
/// </remarks>
public sealed class VTSAvailableModelsResponseData : VTSResponseData
{
    [JsonPropertyName("numberOfModels")] public int NumberOfModels { get; set; }
    [JsonPropertyName("availableModels")] public List<Model>? AvailableModels { get; set; }

    public override void Reset()
    {
        base.Reset();
        NumberOfModels = default;
        AvailableModels?.Clear();
    }

    public readonly struct Model
    {
        [JsonPropertyName("modelLoaded")] public bool ModelLoaded { get; init; }
        [JsonPropertyName("modelName")] public string? ModelName { get; init; }
        [JsonPropertyName("modelID")] public string? ModelID { get; init; }
        [JsonPropertyName("vtsModelName")] public string? VTSModelName { get; init; }
        [JsonPropertyName("vtsModelIconName")] public string? VTSModelIconName { get; init; }

        public override string ToString() => this.ToLoggingString();
        //public override string? ToString() => ToString(VTSHelpers.DefaultPrefix);
        //public string? ToString(string prefix) => ToString(b: new(), prefix).ToString();
        //public static StringBuilder ToString(Model model, StringBuilder b, string prefix = VTSHelpers.DefaultPrefix) => model.ToString(b, prefix);
        //public StringBuilder ToString(StringBuilder b, string prefix = VTSHelpers.DefaultPrefix)
        //{
        //    VTSHelpers.AppendLine(b, prefix, ModelLoaded);
        //    VTSHelpers.AppendLine(b, prefix, ModelName);
        //    VTSHelpers.AppendLine(b, prefix, ModelID);
        //    VTSHelpers.AppendLine(b, prefix, VTSModelName);
        //    VTSHelpers.Append(b, prefix, VTSModelIconName);
        //    return b;
        //}
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = "")
    //{
    //    AppendLine(b, prefix, NumberOfModels);
    //    AppendList(b, prefix, AvailableModels, Model.ToString).AppendLine();
    //    return base.ToString(b, prefix);
    //}
}
