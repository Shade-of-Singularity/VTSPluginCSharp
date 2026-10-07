namespace VTSPlugin;

/// <summary>
/// Interface for associating <see cref="IVTSLogger"/> with specific <see cref="IVTSLoggerInitializationOptions"/>.
/// </summary>
/// <typeparam name="T"><see cref="IVTSLogger"/> initialization options.</typeparam>
public interface IVTSLogger<T> : IVTSLogger, IService<T> where T : IVTSLoggerInitializationOptions
{
    /// <inheritdoc cref="IVTSLogger.Options"/>
    public new T Options { get; }
}

/// <summary>
/// Interface for a logger, responsible for displaying things in console view and/or logging things to the disk.
/// </summary>
/// <remarks>
/// Calling log methods before it was initialized are completely alright.
/// But those records likely won't be stored in a log file.
/// </remarks>
/// Goals:
/// 1. Concurrency safety in WPF and Console apps. Optional concurrency in Unity and Godot (?).
/// 2. Abstract and sealed implementations, to allow JIT to optimize vTables.
/// 3. Reliability - all log messages written up until logger terminates should be written to the disk successfully.
/// 4. Settings for controlling the max log load.
/// 5. Optimization via custom UniTask/GDTask bounded channel.
/// 6. 
public interface IVTSLogger : IService
{
    /// <remarks>
    /// Default value is declared in its own strongly-typed field, so the JIT can "see"
    /// the underlying type of <see cref="Default"/> property and devirtualize all the calls to it.
    /// (Note: this has a weaker effect now, since you can overwrite default field).
    /// </remarks>
    static readonly ConsoleVTSLogger m_DefaultLogger = new()
    {
#if TRACE
        LogLevel = VTSLoggingLevel.Trace
#elif DEBUG
        LogLevel = VTSLoggingLevel.Debug
#else
        LogLevel = VTSLoggingLevel.Info
#endif
    };
    /// <summary>
    /// Backing field for <see cref="IVTSLogger"/>.
    /// </summary>
    static IVTSLogger m_DefaultBackingField = m_DefaultLogger;
    /// <summary>
    /// Default instance to use when none is provided.
    /// </summary>
    /// <remarks>
    /// Swapping <see cref="IVTSLogger"/> for another one in the middle of the application lifecycle
    /// can cause some exceptions to be logged to the old and new logger (outside of JIT optimizations).
    /// <para>
    /// 3
    /// It is advised that you only swap default logger outside of the app lifecycle.
    /// </para>
    /// <para>
    /// If provided with null - resets back to <see cref="ConsoleVTSLogger"/>.
    /// </para>
    /// </remarks>
    public static IVTSLogger Default
    {
        get => m_DefaultBackingField;
        set => m_DefaultBackingField = value ?? m_DefaultLogger;
    }
    /// <remarks>
    /// Implementation might use <see cref="Interlocked"/> members for reading and writing!
    /// </remarks>
    /// Note: I wonder if using interlocked here is even a good idea, actually.
    ///  LogLevel is accessed very frequently to be honest... <see cref="VTSLoggerExtensions"/>
    VTSLoggingLevel LogLevel { get; set; }
    /// <summary>
    /// Options, used when initializing the logger.
    /// </summary>
    IVTSLoggerInitializationOptions Options { get; }
    /// <summary>
    /// Logs as a given level.
    /// </summary>
    /// <remarks>
    /// It doesn't matter which <see cref="LogLevel"/> is it right now - it will use <paramref name="level"/> just as a reference.
    /// Called must ensure whether the log will actually be displayed or not using different extension methods.
    /// <seealso cref="VTSLoggerExtensions"/>
    /// </remarks>
    /// <param name="input">Input to output to the console.</param>
    /// <param name="prefix">Prefix to add before the message. See <see cref="ConsoleVTSLogger"/> for implementation example.</param>
    void Log(object? input, object? prefix, VTSLoggingLevel level = VTSLoggingLevel.Info);
}