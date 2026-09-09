using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VTSPlugin;

public static class VTSAPI
{
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

/// <summary>
/// Logging levels for <see cref="IVTSLogger"/>.
/// </summary>
/// <remarks>
/// Follows standards described by Sematext: https://sematext.com/blog/logging-levels/
/// </remarks>
/// TODO: Revisit and simplify level descriptions.
public enum VTSLoggingLevel : int // int, to allow usage in CLS-compliant Interlocked methods.
{
    /// <summary>
    /// The most fine-grained information only used in rare cases where you need the full visibility of what is happening in your application and inside the third-party libraries that you use. You can expect the TRACE logging level to be very verbose. You can use it for example to annotate each step in the algorithm or each individual query with parameters in your code.
    /// </summary>
    Trace,
    /// <summary>
    /// Less granular compared to the TRACE level, but it is more than you will need in everyday use. The DEBUG log level should be used for information that may be needed for diagnosing issues and troubleshooting or when running application in the test environment for the purpose of making sure everything is running correctly.
    /// </summary>
    Debug,
    /// <summary>
    /// The standard log level indicating that something happened, the application entered a certain state, etc. For example, a controller of your authorization API may include an INFO log level with information on which user requested authorization if the authorization was successful or not. The information logged using the INFO log level should be purely informative and not looking into them on a regular basis shouldn't result in missing any important information.
    /// </summary>
    /// <remarks>
    /// Default logging level.
    /// </remarks>
    Info,
    /// <summary>
    /// The log level that indicates that something unexpected happened in the application, a problem, or a situation that might disturb one of the processes. But that doesn't mean that the application failed. The WARN level should be used in situations that are unexpected, but the code can continue the work. For example,
    /// </summary>
    Warn,
    /// <summary>
    /// The log level that should be used when the application hits an issue preventing one or more functionalities from properly functioning. The ERROR log level can be used when one of the payment systems is not available, but there is still the option to check out the basket in the e-commerce application or when your social media logging option is not working for some reason.
    /// </summary>
    Error,
    /// <summary>
    /// The log level that tells that the application encountered an event or entered a state in which one of the crucial business functionality is no longer working. A FATAL log level may be used when the application is not able to connect to a crucial data store like a database or all the payment systems are not available and users can’t checkout their baskets in your e-commerce.
    /// </summary>
    Fatal,
}

public interface IVTSLogger
{
    /// <summary>
    /// Default instance to use when none is provided.
    /// </summary>
    public static IVTSLogger Default => field ??= new ConsoleVTSLogger()
    {
#if TRACE
        LogLevel = VTSLoggingLevel.Trace
#elif DEBUG
        LogLevel = VTSLoggingLevel.Debug
#else
        LogLevel = VTSLoggingLevel.Info
#endif
    };
    /// <remarks>
    /// Implementation should use <see cref="Interlocked"/> members for reading and writing!
    /// </remarks>
    public VTSLoggingLevel LogLevel { get; set; }
    /// <summary>
    /// Logs as a given level.
    /// </summary>
    /// <param name="input">Input to output to the console.</param>
    /// <param name="prefix">Prefix to add before the message. See <see cref="ConsoleVTSLogger"/> for implementation example.</param>
    void Log(object? input, string? prefix, VTSLoggingLevel level);
}

public sealed class ConsoleVTSLogger : IVTSLogger
{
    public VTSLoggingLevel LogLevel
    {
        get => (VTSLoggingLevel)InterlockedHelpers.Read(ref Unsafe.As<VTSLoggingLevel, int>(ref field));
        set => Interlocked.Exchange(ref Unsafe.As<VTSLoggingLevel, int>(ref field), (int)value);
    } = VTSLoggingLevel.Info;


    readonly ConcurrentStack<StringBuilder> Builders = new();
    public void Log(object? input, string? prefix, VTSLoggingLevel level)
    {
        StringBuilder builder = AcquireBuilder();
        try
        {
            //ConsoleColor last = Console.ForegroundColor;
            //Console.ForegroundColor = color;

            //builder.Clear();
            //if (prefix is not null)
            //{
            //    builder.Append(prefix);
            //    if (!prefix.EndsWith(' ') && !prefix.EndsWith('\t'))
            //        builder.Append(' ');
            //}
            //builder.Append((obj ?? "null").ToString());
            //string final = builder.ToString();
            //builder.Clear();

            //Console.WriteLine(final);
            //Console.ForegroundColor = last;
            //LoggerService.Instance.Log(final);
        }
        //catch (Exception ex) { Console.WriteLine(ex.ToDisplay()); }
        finally { ReturnBuilder(builder); }
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    StringBuilder AcquireBuilder() => Builders.TryPop(out var builder) ? builder : new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ReturnBuilder(StringBuilder builder) => Builders.Push(builder);
}

public static class IVTSLoggerExtensions
{
    extension(IVTSLogger logger)
    {
        /// <inheritdoc cref="IVTSLogger.Log(object?, string?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log(object? input, VTSLoggingLevel level) => logger.Log(input, null, level);

        /// <inheritdoc cref="IVTSLogger.Log(object?, string?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log(Exception? exception, VTSLoggingLevel level = VTSLoggingLevel.Error) => logger.Log(exception, null, level);

        /// <inheritdoc cref="IVTSLogger.Log(object?, string?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log(Exception? exception, string? prefix, VTSLoggingLevel level = VTSLoggingLevel.Error) => logger.Log(exception, prefix, level);

        /// <inheritdoc cref="IVTSLogger.Log(object?, string?, VTSLoggingLevel)"/>
        /// <remarks>
        /// If <see cref="IVTSLogger.LogLevel"/> is higher than <paramref name="level"/> - nothing is logged.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TryLog(object? exception, VTSLoggingLevel level = VTSLoggingLevel.Error)
        {
            if (logger.ShouldLog(level))
                logger.Log(exception, null, level);
        }

        /// <inheritdoc cref="IVTSLogger.Log(object?, string?, VTSLoggingLevel)"/>
        /// <inheritdoc cref="TryLog(IVTSLogger, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TryLog(Exception? exception, VTSLoggingLevel level = VTSLoggingLevel.Error)
        {
            if (logger.ShouldLog(level))
                logger.Log(exception, null, level);
        }

        /// <inheritdoc cref="IVTSLogger.Log(object?, string?, VTSLoggingLevel)"/>
        /// <inheritdoc cref="TryLog(IVTSLogger, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TryLog(Exception? exception, string? prefix, VTSLoggingLevel level = VTSLoggingLevel.Error)
        {
            if (logger.ShouldLog(level))
                logger.Log(exception, prefix, level);
        }

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> == <see cref="VTSLoggingLevel.Trace"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLog(VTSLoggingLevel level) => logger.LogLevel == level;

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> == <see cref="VTSLoggingLevel.Trace"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLogTrace() => logger.LogLevel == VTSLoggingLevel.Trace;

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> == <see cref="VTSLoggingLevel.Debug"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLogDebug() => logger.LogLevel == VTSLoggingLevel.Debug;

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> == <see cref="VTSLoggingLevel.Info"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLogInfo() => logger.LogLevel == VTSLoggingLevel.Info;

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> == <see cref="VTSLoggingLevel.Warn"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLogWarn() => logger.LogLevel == VTSLoggingLevel.Warn;

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> == <see cref="VTSLoggingLevel.Error"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLogError() => logger.LogLevel == VTSLoggingLevel.Error;

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> == <see cref="VTSLoggingLevel.Fatal"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLogFatal() => logger.LogLevel == VTSLoggingLevel.Fatal;
    }
}