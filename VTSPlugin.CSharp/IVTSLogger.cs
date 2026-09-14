namespace VTSPlugin;

public interface IVTSLogger
{
    /// <remarks>
    /// Default value is declared in its own strongly-typed field, so the JIT can "see"
    /// the underlying type of <see cref="Default"/> property and devirtualize all the calls to it.
    /// </remarks>
    static readonly ConsoleVTSLogger m_Default = new()
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
    /// Default instance to use when none is provided.
    /// </summary>
    public static IVTSLogger Default => m_Default;
    /// <remarks>
    /// Implementation should use <see cref="Interlocked"/> members for reading and writing!
    /// </remarks>
    public VTSLoggingLevel LogLevel { get; set; }
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
    void Log(object? input, object? prefix, VTSLoggingLevel level);
}
