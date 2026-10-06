using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VTSPlugin;

/// <summary>
/// Stores common constants/settings used through-out the entirety of <see cref="VTSPlugin"/> library.
/// </summary>
/// <remarks>
/// You can learn more about VTube Studio API itself in the official documentation: https://github.com/DenchiSoft/VTubeStudio
/// </remarks>
public static class VTSAPI
{
    /// <summary>
    /// API Name that is used in <see cref="JsonPropertyNames.APIName"/> unless user overwrites it.
    /// </summary>
    /// <remarks>
    /// This is the only API Name defined in VTube Studio API: https://github.com/DenchiSoft/VTubeStudio
    /// You can manually overwrite it for each request individually.
    /// </remarks>
    public static readonly string DefaultAPIName = "VTubeStudioPublicAPI";
    /// <summary>
    /// API Version to use in <see cref="JsonPropertyNames.APIVersion"/> unless user overwrites it.
    /// </summary>
    /// <remarks>
    /// This is the only API Version defined in VTube Studio API: https://github.com/DenchiSoft/VTubeStudio
    /// You can manually overwrite it for each request individually.
    /// </remarks>
    public static readonly string DefaultAPIVersion = "1.0";
    /// <summary>
    /// Default port used for a discovery service.
    /// </summary>
    /// <remarks>
    /// This port is defined in VTube Studio API.
    /// See more: https://github.com/DenchiSoft/VTubeStudio#api-server-discovery-udp
    /// </remarks>
    public static readonly ushort DefaultDiscoveryPort = 47779;
    /// <summary>
    /// Json names for some top-level fields in Json data.
    /// </summary>
    /// <remarks>
    /// Referenced in zero/low allocation locations, such as when using high-performance <see cref="Utf8JsonReader"/>s.
    /// </remarks>
    public static class JsonPropertyNames
    {
        public const string APIName = "apiName";
        public const string APIVersion = "apiVersion";
        public const string RequestID = "requestID";
        public const string MessageType = "messageType";
        public const string Data = "data";
        public const string ErrorID = "errorID";
        public const string Message = "message";
    }
    /// <summary>
    /// VTube Studio uses UFT8 encoding.
    /// This property serves as a single source of truth for all APIs and plugins to use.
    /// </summary>
    public static Encoding Encoding
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Encoding.UTF8;
    }
    /// <summary>
    /// Options to use for JSON serialization in <see cref="JsonSerializer"/>.
    /// </summary>
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        TypeInfoResolver = VTSJsonTypeInfoResolver.Instance,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
        UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
    };
    /// <summary>
    /// Options to use for JSON serialization in <see cref="JsonSerializer"/> specifically for logging.
    /// </summary>
    public static readonly JsonSerializerOptions JsonLoggingOptions = new()
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        TypeInfoResolver = VTSJsonTypeInfoResolver.Instance,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
        UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
    };
    /// <summary>
    /// Options to use for Json serialization in <see cref="JsonDocument"/>.
    /// </summary>
    public static readonly JsonDocumentOptions DocumentOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
    };
    /// <summary>
    /// Options used with internal <see cref="Utf8JsonReader"/>s.
    /// </summary>
    public static readonly JsonReaderOptions ReaderOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
    };
}
