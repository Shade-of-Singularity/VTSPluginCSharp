using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Default options implementation for use with <see cref="IVTSLogger{T}"/>s.
/// </summary>
public sealed class DefaultLoggerOptions() : IVTSLoggerInitializationOptions, IEquatable<DefaultLoggerOptions>
{
    /// <summary>
    /// Default logger options to use.
    /// </summary>
    public static readonly DefaultLoggerOptions Instance = new()
    {
#if TRACE
        LogLevel = VTSLoggingLevel.Trace,
#elif DEBUG
        LogLevel = VTSLoggingLevel.Debug,
#else
        LogLevel = VTSLoggingLevel.Info,
#endif
        LogDirectory = IVTSLoggerInitializationOptions.DefaultLogDirectory,
    };

    // TODO: Double-check what happens to IsFrozen under concurrency.
    /// <remarks>
    /// Doesn't influence the <see cref="Equals(DefaultLoggerOptions)"/> check.
    /// </remarks>
    /// <inheritdoc/>
    public bool IsFrozen { get; private set; }

    /// <inheritdoc/>
    public required VTSLoggingLevel LogLevel
    {
        get => field;
        set
        {
            if (IsFrozen)
                MemberFrozenException.Throw(typeof(DefaultLoggerOptions), nameof(LogLevel));
            field = value;
        }
    }

    /// <inheritdoc/>
    public required string LogDirectory
    {
        get => field;
        set
        {
            if (IsFrozen)
                MemberFrozenException.Throw(typeof(DefaultLoggerOptions), nameof(LogLevel));
            field = value;
        }
    }

    /// <inheritdoc/>
    public string LogFileName
    {
        get => field;
        set
        {
            if (IsFrozen)
                MemberFrozenException.Throw(typeof(DefaultLoggerOptions), nameof(LogLevel));
            field = value;
        }
    } = IVTSLoggerInitializationOptions.DefaultLogFileName;

    /// <inheritdoc/>
    public string LogFileNameDateTimeFormat
    {
        get => field;
        set
        {
            if (IsFrozen)
                MemberFrozenException.Throw(typeof(DefaultLoggerOptions), nameof(LogLevel));
            field = value;
        }
    } = IVTSLoggerInitializationOptions.DefaultFileNameDateTimeFormat;

    /// <inheritdoc/>
    public string LogFileExtension
    {
        get => field;
        set
        {
            if (IsFrozen)
                MemberFrozenException.Throw(typeof(DefaultLoggerOptions), nameof(LogLevel));
            field = value;
        }
    } = IVTSLoggerInitializationOptions.DefaultLogFileExtension;

    /// <inheritdoc/>
    public string LogFileNameSeparator
    {
        get => field;
        set
        {
            if (IsFrozen)
                MemberFrozenException.Throw(typeof(DefaultLoggerOptions), nameof(LogLevel));
            field = value;
        }
    } = IVTSLoggerInitializationOptions.DefaultLogFileNameSeparator;

    /// <inheritdoc/>
    public void Freeze() => IsFrozen = true;

    /// <inheritdoc/>
    public void Unfreeze() => IsFrozen = false;

    /// <remarks>
    /// Never fires, and should never be used. Added to avoid memory leaks in WPF.
    /// https://stackoverflow.com/questions/18542940/can-bindings-create-memory-leaks-in-wpf
    /// </remarks>
    /// <inheritdoc/>
    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add { }
        remove { }
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is DefaultLoggerOptions other && Equals(other);

    /// <inheritdoc/>
    public bool Equals(DefaultLoggerOptions? other)
    {
        if (ReferenceEquals(this, other)) return true;
        return other is not null
            && LogLevel == other.LogLevel
            && LogFileName == other.LogFileName
            && LogDirectory == other.LogDirectory
            && LogFileExtension == other.LogFileExtension
            && LogFileNameDateTimeFormat == other.LogFileNameDateTimeFormat;
    }

    /// <inheritdoc/>
    public bool Equals(IVTSLoggerInitializationOptions? other)
    {
        if (ReferenceEquals(this, other)) return true;
        return other is not null
            && LogLevel == other.LogLevel
            && LogFileName == other.LogFileName
            && LogDirectory == other.LogDirectory
            && LogFileExtension == other.LogFileExtension
            && LogFileNameDateTimeFormat == other.LogFileNameDateTimeFormat;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(
        LogLevel, LogFileName, LogDirectory, LogFileExtension, LogFileNameDateTimeFormat);

    /// <inheritdoc/>
    public void Reset()
    {
        // Fields are init-only, so how do we even allow pooling?...
        // Maybe I can just make them public. Maybe make class freezable as well.
        LogLevel = VTSLoggingLevel.Info;
        LogFileName = IVTSLoggerInitializationOptions.DefaultLogFileName;
        LogDirectory = IVTSLoggerInitializationOptions.DefaultLogDirectory;
        LogFileExtension = IVTSLoggerInitializationOptions.DefaultLogFileExtension;
        LogFileNameDateTimeFormat = IVTSLoggerInitializationOptions.DefaultFileNameDateTimeFormat;
        IsFrozen = false;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(DefaultLoggerOptions? a, DefaultLoggerOptions? b) => a is null ? b is null : a.Equals(b);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(DefaultLoggerOptions? a, DefaultLoggerOptions? b) => a is null ? b is not null : !a.Equals(b);
}
