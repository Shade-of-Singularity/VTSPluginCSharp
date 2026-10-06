using Cysharp.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Options to use when initializing <see cref="IVTSLogger{T}"/>.
/// </summary>
/// Note: Implements <see cref="INotifyPropertyChanged"/> despite never being used, in if used in WPF.
/// See 5th entry here: https://michaelscodingspot.com/ways-to-cause-memory-leaks-in-dotnet
public interface IVTSLoggerInitializationOptions : IEquatable<IVTSLoggerInitializationOptions>, INotifyPropertyChanged
{
    /// <summary>
    /// Default value for <see cref="LogFileName"/>.
    /// </summary>
    public const string DefaultLogFileName = "";
    /// <summary>
    /// Default value for <see cref="LogFileNameSeparator"/>.
    /// </summary>
    public const string DefaultLogFileNameSeparator = "-";
    /// <summary>
    /// Default value for <see cref="LogFileNameDateTimeFormat"/>.
    /// </summary>
    public const string DefaultFileNameDateTimeFormat = "yyyy-MM-dd-HH-mm-ss-fff";
    /// <summary>
    /// Default value for <see cref="LogFileExtension"/>.
    /// </summary>
    public const string DefaultLogFileExtension = ".log";
    /// <summary>
    /// Default value for <see cref="LogDirectory"/>.
    /// </summary>
    public static readonly string DefaultLogDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Logs");
    /// <summary>
    /// Initial level for <see cref="IVTSLogger"/> to use.
    /// </summary>
    VTSLoggingLevel LogLevel { get; }
    /// <summary>
    /// Directory path to where all the logs should be stored.
    /// </summary>
    string LogDirectory { get; }
    /// <summary>
    /// Default file name to use.
    /// </summary>
    /// <remarks>
    /// File name is combine with <see cref="DateTime.Now"/> using format: {filename}{separator}{datetime}{extension}.
    /// If <see cref="LogFileName"/> is <see cref="string.IsNullOrEmpty(string)"/> - <see cref="LogFileNameSeparator"/> will be omitted.
    /// </remarks>
    string LogFileName { get; }
    /// <summary>
    /// Separator to use between <see cref="LogFileName"/> and <see cref="DateTime.Now"/>,
    /// formatted with <see cref="LogFileNameDateTimeFormat"/>
    /// </summary>
    /// <remarks>
    /// Omitted if <see cref="LogFileName"/> is <see cref="string.IsNullOrEmpty(string)"/>.
    /// </remarks>
    string LogFileNameSeparator { get; }
    /// <summary>
    /// Format to use with <see cref="DateTime.Now"/>.
    /// </summary>
    string LogFileNameDateTimeFormat { get; }
    /// <summary>
    /// Extension for log files.
    /// </summary>
    /// <remarks>
    /// MUST have a '.' at the beginning!
    /// </remarks>
    string LogFileExtension { get; }
}

/// <summary>
/// Contains useful extensions for working with <see cref="IVTSLoggerInitializationOptions"/>, used throughout the library.
/// </summary>
public static class VTSLoggerInitializationOptionsExtensions
{
    /// <summary>
    /// Validates the state of the provided <paramref name="options"/>
    /// </summary>
    /// <typeparam name="T">Tye of the options to validate. Must be a struct.</typeparam>
    /// <param name="options"></param>
    /// <returns>
    /// <see langword="true"/> - when <paramref name="options"/> passed API common validations.
    /// <see langword="false"/> - when <paramref name="options"/> contain issues.
    /// </returns>
    /// Note: implemented as generic, to avoid struct boxing on each call.
    public static bool Validate<T>(this T options) where T : IVTSLoggerInitializationOptions
    {
        return options.LogFileExtension.StartsWith('.');
    }

    /// <summary>
    /// Validates the state of the provided <paramref name="options"/> using assertions.
    /// </summary>
    /// <typeparam name="T">Type of the options to validate. Must be a struct.</typeparam>
    /// <param name="options">Options to validate.</param>
    /// <exception cref="InvalidExtensionException">
    /// When file extension in <paramref name="options"/> does not start from a '.' (dot).
    /// </exception>
    /// Note: implemented as generic, to avoid struct boxing on each call.
    public static void AssertValidate<T>(this T options) where T : IVTSLoggerInitializationOptions
    {
        if (!options.LogFileExtension.StartsWith('.'))
            InvalidExtensionException.ThrowMissingDot();
    }
}

[Serializable]
public sealed class InvalidExtensionException : VTSException
{
    /// <inheritdoc/>
    public InvalidExtensionException() : base() { }

    /// <inheritdoc/>
    public InvalidExtensionException(string message) : base(message) { }

    /// <inheritdoc/>
    public InvalidExtensionException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Throws <see cref="InvalidExtensionException"/>, specifying that the extension should start from a '.' (dot).
    /// </summary>
    /// <exception cref="InvalidExtensionException">Exception, specifying that the extension should start from a '.' (dot).</exception>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowMissingDot() => throw new InvalidExtensionException("Extension should start from a '.' (dot)!");
}

/// <summary>
/// Interface for associating <see cref="IVTSLogger"/> with specific <see cref="IVTSLoggerInitializationOptions"/>.
/// </summary>
/// <typeparam name="T"><see cref="IVTSLogger"/> initialization options.</typeparam>
public interface IVTSLogger<T> : IVTSLogger, IService<T> where T : IVTSLoggerInitializationOptions
{
    /// <inheritdoc cref="IVTSLogger.Options"/>
    public new T Options { get; }
}

public sealed class LoggerOptions() : IVTSLoggerInitializationOptions
{
    public static readonly LoggerOptions Default = new()
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

    public required VTSLoggingLevel LogLevel { get; init; }
    public required string LogDirectory { get; init; }
    public string LogFileName { get; init; } = IVTSLoggerInitializationOptions.DefaultLogFileName;
    public string LogFileNameDateTimeFormat { get; init; } = IVTSLoggerInitializationOptions.DefaultFileNameDateTimeFormat;
    public string LogFileExtension { get; init; } = IVTSLoggerInitializationOptions.DefaultLogFileExtension;
    public string LogFileNameSeparator { get; init; } = IVTSLoggerInitializationOptions.DefaultLogFileNameSeparator;

    /// <remarks>
    /// Never fired. Added to avoid memory leaks in WPF.
    /// </remarks>
    /// <inheritdoc/>
    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
        add { }
        remove { }
    }

    public bool Equals(IVTSLoggerInitializationOptions other)
    {
        return LogLevel == other.LogLevel
            && LogFileName == other.LogFileName
            && LogDirectory == other.LogDirectory
            && LogFileExtension == other.LogFileExtension
            && LogFileNameDateTimeFormat == other.LogFileNameDateTimeFormat;
    }
}

public sealed class Logger : IVTSLogger<LoggerOptions>
{
    internal const string LogPrefix = $"[{nameof(Logger)}]";

    public event PropertyChangedEventHandler? PropertyChanged;
    public VTSLoggingLevel LogLevel
    {
        get => (VTSLoggingLevel)InterlockedHelpers.Read(ref Unsafe.As<VTSLoggingLevel, int>(ref field));
        set
        {
            if (Interlocked.Exchange(ref Unsafe.As<VTSLoggingLevel, int>(ref field), (int)value) != (int)value)
                PropertyChanged?.Invoke(this, KnownPropertyChangedArgs.LogLevel);
        }
    } = VTSLoggingLevel.Info;

    public bool IsInitialized
    {
        get => field;
        private set
        {
            if (field != value)
            {
                field = value;
                PropertyChanged?.Invoke(this, KnownPropertyChangedArgs.IsInitialized);
            }
        }
    }

    IVTSLoggerInitializationOptions IVTSLogger.Options => Options;
    public LoggerOptions Options
    {
        get => field;
        private set
        {
            if (!field.Equals(value))
            {
                field = value;
                PropertyChanged?.Invoke(this, KnownPropertyChangedArgs.Options);
            }
        }
    } = LoggerOptions.Default;


    readonly SemaphoreSlim Semaphore = new(0, 1);
    // Sync version.
    StreamWriter? LogStream;

    /// <inheritdoc/>
    public UniTask<bool> Initialize() => Initialize(LoggerOptions.Default);

    /// <inheritdoc/>
    public async UniTask<bool> Initialize(LoggerOptions options)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        options.AssertValidate();
        await Semaphore.WaitAsync();
        if (IsInitialized)
        {
            // Does not reinitialize if the options are the same.
            if (Options.Equals(options))
                return true;

            await TerminateCore();
        }

        try
        {
            Directory.CreateDirectory(options.LogDirectory);

            // TODO: Add name clash resolution.
            string name = string.IsNullOrEmpty(options.LogFileName)
                ? $"{DateTime.Now.ToString(options.LogFileNameDateTimeFormat)}{options.LogFileExtension}"
                : $"{options.LogFileName}{options.LogFileNameSeparator}{DateTime.Now.ToString(options.LogFileNameDateTimeFormat)}{options.LogFileExtension}";
            string file = Path.Combine(options.LogDirectory, name);
            LogStream = new StreamWriter(File.Open(file, FileMode.Create));
            LogStream.WriteLine();
            await LogStream.FlushAsync();

            Options = options;
            IsInitialized = true;
            PropertyChanged?.Invoke(this, KnownPropertyChangedArgs.Options);
            return true;
        }
        catch (Exception ex)
        {
            if (LogStream is not null)
                await LogStream.DisposeAsync();
            LogStream = null;
            this.Log(ex, LogPrefix);
            IsInitialized = false;
            return false;
        }
        finally { Semaphore.Release(); }
    }

    /// <inheritdoc/>
    public void Log(object? input, object? prefix, VTSLoggingLevel level)
    {
        if (LogLevel < level) return;
    }

    /// <inheritdoc/>
    public async UniTask<bool> Terminate()
    {
        await Semaphore.WaitAsync();
        try
        {
            return await TerminateCore();
        }
        finally { Semaphore.Release(); }
    }

    UniTask<bool> TerminateCore()
    {
        return UniTask.FromResult(true);
    }

    internal static class KnownPropertyChangedArgs
    {
        public static readonly PropertyChangedEventArgs IsInitialized = new(nameof(Logger.IsInitialized));
        public static readonly PropertyChangedEventArgs Options = new(nameof(Logger.Options));
        public static readonly PropertyChangedEventArgs LogLevel = new(nameof(Logger.LogLevel));
    }
}

/// <summary>
/// Interface for a logger, responsible for displaying things in console view and/or logging things to the disk.
/// </summary>
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
    /// Implementation should use <see cref="Interlocked"/> members for reading and writing!
    /// </remarks>
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
    void Log(object? input, object? prefix, VTSLoggingLevel level);
}