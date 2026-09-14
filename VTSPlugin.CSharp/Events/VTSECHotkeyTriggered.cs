using System.Text.Json.Serialization;
using VTSPlugin.Legacy;

namespace VTSPlugin.Events;

public sealed class VTSECHotkeyTriggered : VTSEventConfig
{
    [JsonPropertyName("onlyForAction")] public required string? OnlyForAction { get; set; }
    [JsonPropertyName("ignoreHotkeysTriggeredByAPI")] public required bool IgnoreHotkeysTriggeredByAPI { get; set; }

    public override void Reset()
    {
        base.Reset();
        OnlyForAction = default;
        IgnoreHotkeysTriggeredByAPI = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = "")
    //{
    //    AppendLine(b, prefix, OnlyForAction);
    //    Append(b, prefix, IgnoreHotkeysTriggeredByAPI);
    //    return b;
    //}
}