using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Useful extensions for when working with <see cref="IVTSLogger"/>s.
/// </summary>
public static class VTSLoggerExtensions
{
    /// <summary>
    /// <see cref="VTSLoggingLevel"/> to use by default with all <see cref="Exception"/>s.
    /// </summary>
    const VTSLoggingLevel DefaultExceptionLevel = VTSLoggingLevel.Error;

    extension<T>(T logger) where T : IVTSLogger
    {
        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log(object? input, VTSLoggingLevel level) => logger.Log(input, null, level);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogTrace(object? input) => logger.Log(input, null, VTSLoggingLevel.Trace);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogTrace(object? input, string? prefix) => logger.Log(input, prefix, VTSLoggingLevel.Trace);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogDebug(object? input) => logger.Log(input, null, VTSLoggingLevel.Debug);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogDebug(object? input, string? prefix) => logger.Log(input, prefix, VTSLoggingLevel.Debug);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogInfo(object? input) => logger.Log(input, null, VTSLoggingLevel.Info);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogInfo(object? input, string? prefix) => logger.Log(input, prefix, VTSLoggingLevel.Info);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogWarn(object? input) => logger.Log(input, null, VTSLoggingLevel.Warn);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogWarn(object? input, string? prefix) => logger.Log(input, prefix, VTSLoggingLevel.Warn);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogError(object? input) => logger.Log(input, null, VTSLoggingLevel.Error);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogError(object? input, string? prefix) => logger.Log(input, prefix, VTSLoggingLevel.Error);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogFatal(object? input) => logger.Log(input, null, VTSLoggingLevel.Fatal);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LogFatal(object? input, string? prefix) => logger.Log(input, prefix, VTSLoggingLevel.Fatal);



        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log(Exception? exception, VTSLoggingLevel level = DefaultExceptionLevel) => logger.Log(exception, null, level);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log(Exception? exception, string? prefix, VTSLoggingLevel level = DefaultExceptionLevel) => logger.Log(exception, prefix, level);



        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> >= <see cref="VTSLoggingLevel.Trace"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLog(VTSLoggingLevel level) => logger.LogLevel >= level;

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> >= <see cref="VTSLoggingLevel.Trace"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLogTrace() => logger.LogLevel >= VTSLoggingLevel.Trace;

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> >= <see cref="VTSLoggingLevel.Debug"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLogDebug() => logger.LogLevel >= VTSLoggingLevel.Debug;

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> >= <see cref="VTSLoggingLevel.Info"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLogInfo() => logger.LogLevel >= VTSLoggingLevel.Info;

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> >= <see cref="VTSLoggingLevel.Warn"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLogWarn() => logger.LogLevel >= VTSLoggingLevel.Warn;

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> >= <see cref="VTSLoggingLevel.Error"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLogException() => logger.LogLevel >= DefaultExceptionLevel;

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> >= <see cref="VTSLoggingLevel.Error"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLogError() => logger.LogLevel >= VTSLoggingLevel.Error;

        /// <summary>
        /// <see cref="IVTSLogger.LogLevel"/> >= <see cref="VTSLoggingLevel.Fatal"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ShouldLogFatal() => logger.LogLevel >= VTSLoggingLevel.Fatal;
    }
}