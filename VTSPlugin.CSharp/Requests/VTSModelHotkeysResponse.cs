using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using VTSPlugin.Pooling;

namespace VTSPlugin.Requests;

[VTSJsonConstructibleWith(nameof(JsonConstruct))]
public sealed class VTSModelHotkeysResponse : VTSResponse<VTSModelHotkeysResponseData>
{
    /// <seealso cref="VTSJsonConstructibleWithAttribute"/>
    static object JsonConstruct() => Rent();

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <inheritdoc cref="Rent(VTSPool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VTSModelHotkeysResponse Rent() => Rent(VTSPool.Shared);

    /// <summary>
    /// Rents new packet instance from <see cref="VTSPool.Shared"/>.
    /// </summary>
    /// <remarks>
    /// Rented members require extra care.
    /// See <see cref="VTSRequestManagerExtensions"/> for implementation examples.
    /// </remarks>
    public static VTSModelHotkeysResponse Rent(VTSPool pool) => pool.Rent<VTSModelHotkeysResponse>();
}

public sealed class VTSModelHotkeysResponseData : VTSResponseData
{
    [JsonPropertyName("modelLoaded")] public bool ModelLoaded { get; set; }
    [JsonPropertyName("modelName")] public string? ModelName { get; set; }
    [JsonPropertyName("modelID")] public string? ModelID { get; set; }
    [JsonPropertyName("availableHotkeys")] public List<Hotkey>? AvailableHotkeys { get; set; }

    public override void Reset()
    {
        base.Reset();
        ModelLoaded = default;
        ModelName = default;
        ModelID = default;
        AvailableHotkeys?.Clear();
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = "")
    //{
    //    AppendLine(b, prefix, ModelLoaded);
    //    AppendLine(b, prefix, ModelName);
    //    AppendLine(b, prefix, ModelID);
    //    AppendList(b, prefix, AvailableHotkeys, Hotkey.ToString).AppendLine();
    //    return base.ToString(b, prefix);
    //}

    public readonly struct Hotkey
    {
        [JsonPropertyName("name")] public string? Name { get; init; }
        [JsonPropertyName("type")] public string? Type { get; init; }
        [JsonPropertyName("description")] public string? Description { get; init; }
        [JsonPropertyName("file")] public string? File { get; init; }
        [JsonPropertyName("hotkeyID")] public string? HotkeyID { get; init; }
        [JsonPropertyName("onScreenButtonID")] public int OnScreenButtonID { get; init; }

        public override string ToString() => this.ToLoggingString();
        //public override string? ToString() => ToString(VTSHelpers.DefaultPrefix);
        //public string? ToString(string prefix) => ToString(b: new(), prefix).ToString();
        //public static StringBuilder ToString(Hotkey hotkey, StringBuilder b, string prefix = VTSHelpers.DefaultPrefix) => hotkey.ToString(b, prefix);
        //public StringBuilder ToString(StringBuilder b, string prefix = VTSHelpers.DefaultPrefix)
        //{
        //    VTSHelpers.AppendLine(b, prefix, Name);
        //    VTSHelpers.AppendLine(b, prefix, Type);
        //    VTSHelpers.AppendLine(b, prefix, Description);
        //    VTSHelpers.AppendLine(b, prefix, File);
        //    VTSHelpers.AppendLine(b, prefix, HotkeyID);
        //    VTSHelpers.Append(b, prefix, OnScreenButtonID);
        //    return b;
        //}
    }
}