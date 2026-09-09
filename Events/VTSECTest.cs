using System.Text.Json.Serialization;
using VTS.Core;

namespace VTSPlugin.Events;

public sealed class VTSECTest : VTSEventConfig
{
    [JsonPropertyName("testMessageForEvent")] public required string? TestMessageForEvent { get; set; }

    public override void Reset()
    {
        base.Reset();
        TestMessageForEvent = default;
    }

    //public override StringBuilder ToString(StringBuilder b, string prefix = "")
    //{
    //    Append(b, prefix, TestMessageForEvent);
    //    return b;
    //}
}
