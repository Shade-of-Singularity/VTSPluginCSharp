using System.Runtime.CompilerServices;

namespace VTSPlugin;

public static class VTSLoggerExtensions
{
    const VTSLoggingLevel ExceptionLevel = VTSLoggingLevel.Error;

    extension(IVTSLogger logger)
    {
        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log(object? input, VTSLoggingLevel level) => logger.Log(input, null, level);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log(Exception? exception, VTSLoggingLevel level = VTSLoggingLevel.Error) => logger.Log(exception, null, level);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Log(Exception? exception, string? prefix, VTSLoggingLevel level = VTSLoggingLevel.Error) => logger.Log(exception, prefix, level);

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        /// <remarks>
        /// If <see cref="IVTSLogger.LogLevel"/> is higher than <paramref name="level"/> - nothing is logged.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TryLog(object? input, VTSLoggingLevel level = VTSLoggingLevel.Error)
        {
            if (logger.ShouldLog(level))
                logger.Log(input, null, level);
        }

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        /// <remarks>
        /// If <see cref="IVTSLogger.LogLevel"/> is higher than <paramref name="level"/> - nothing is logged.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TryLog(object? input, object? prefix, VTSLoggingLevel level = VTSLoggingLevel.Error)
        {
            if (logger.ShouldLog(level))
                logger.Log(input, prefix, level);
        }

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        /// <inheritdoc cref="TryLog(IVTSLogger, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TryLog(Exception? exception, VTSLoggingLevel level = VTSLoggingLevel.Error)
        {
            if (logger.ShouldLog(level))
                logger.Log(exception, null, level);
        }

        /// <inheritdoc cref="IVTSLogger.Log(object?, object?, VTSLoggingLevel)"/>
        /// <inheritdoc cref="TryLog(IVTSLogger, object?, VTSLoggingLevel)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TryLog(Exception? exception, object? prefix, VTSLoggingLevel level = VTSLoggingLevel.Error)
        {
            if (logger.ShouldLog(level))
                logger.Log(exception, prefix, level);
        }

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
        public bool ShouldLogException() => logger.LogLevel >= ExceptionLevel;

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