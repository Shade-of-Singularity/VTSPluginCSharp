using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using VTSPlugin;

namespace VTSPlugin.WPF;

public sealed class VTSLogger : IVTSLogger<VTSLogger.InitializationOptions>
{
    /// <summary>
    /// Default directory where logs are saved.
    /// </summary>
    static readonly string DefaultLogsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
    /// <summary>
    /// Default <see cref="DateTime"/> format to use.
    /// </summary>
    /// <remarks>
    /// Evaluates to 2008-06-15 21-34-40-123:.
    /// </remarks>
    public const string DefaultDateTimeFormat = "yyyy-MM-dd HH-mm-ss-fff";
    /// <summary>
    /// Default file extension to use.
    /// </summary>
    public const string DefaultFileExtension = ".log";
    /// <summary>
    /// Options for initializing this <see cref="VTSLogger"/>.
    /// </summary>
    public readonly struct InitializationOptions : IVTSLoggerInitializationOptions
    {
        /// <summary>
        /// Default options that you can use.
        /// </summary>
        public static readonly InitializationOptions Default = new()
        {
            LogDirectory = DefaultLogsDirectory,
            DateTimeFormat = DefaultDateTimeFormat,
            FileExtension = DefaultFileExtension,
        };
        /// <summary>
        /// Whether to output all the logs to the console.
        /// </summary>
        public bool WriteConsole { get; init; }
        /// <summary>
        /// Whether to write logs to the disk.
        /// </summary>
        public bool WriteLogs { get; init; }
        /// <summary>
        /// Location for log files.
        /// </summary>
        /// <remarks>
        /// <see cref="VTSLogger"/> will make sure to check that directory actually exist before creating logs.
        /// </remarks>
        public string LogDirectory { get; init; }
        /// <summary>
        /// Format to use on <see cref="DateTime.Now"/>.
        /// </summary>
        public string DateTimeFormat { get; init; }
        /// <summary>
        /// File extension to use.
        /// </summary>
        /// <remarks>
        /// File name is <see cref="DateTime.Now"/> evaluated using <see cref="DateTimeFormat"/>, plus this extension.
        /// </remarks>
        public string FileExtension { get; init; }
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public VTSLoggingLevel LogLevel
    {
        get => (VTSLoggingLevel)Volatile.Read(ref Unsafe.As<VTSLoggingLevel, int>(ref field));
        set => Volatile.Write(ref Unsafe.As<VTSLoggingLevel, int>(ref field), (int)value);
    } = VTSLoggingLevel.Info;

    /// <inheritdoc/>
    public bool IsInitialized
    {
        get => field;
        private set
        {
            // Only accessed under a Semaphore.
            if (field != value)
            {
                field = value;
                try
                {
                    PropertyChanged?.Invoke(this, KnownEventArgs.IsInitializedChanged);
                }
                catch (Exception ex)
                {
                    var logger = value ? this : IVTSLogger.Default;
                    logger.TryLog(ex, $"{this} Exception firing 'IsInitialized: {value}' callback.\n");
                }
            }
        }
    }

    readonly ConcurrentStack<StringBuilder> Builders = new();
    readonly SemaphoreSlim Semaphore = new(0, 1);
    CancellationTokenSource? Source;
    Channel<string>? Channel;
    InitializationOptions Options;

    /// <inheritdoc/>
    public UniTask Initialize() => Initialize(default);

    /// <inheritdoc/>
    public async UniTask Initialize(InitializationOptions options)
    {
        if (options.WriteLogs)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(options.LogDirectory, nameof(options.LogDirectory));
            if (options.LogDirectory.ContainsAny(Path.GetInvalidPathChars()))
                throw new ArgumentException($"{nameof(InitializationOptions.LogDirectory)} contains invalid characters!");

            ArgumentException.ThrowIfNullOrWhiteSpace(options.DateTimeFormat, nameof(options.DateTimeFormat));
            ArgumentException.ThrowIfNullOrWhiteSpace(options.FileExtension, nameof(options.FileExtension));
            if (!options.FileExtension.StartsWith('.'))
                throw new ArgumentException($"{nameof(InitializationOptions.FileExtension)} should start from a dot (e.g.: '.log')!");
            if (options.FileExtension.ContainsAny(Path.GetInvalidFileNameChars()))
                throw new ArgumentException($"{nameof(InitializationOptions.FileExtension)} contains invalid characters!");
        }

        await Semaphore.WaitAsync();
        try
        {
            if (IsInitialized)
            {
                IVTSLogger.Default.TryLog("Already initialized!", this, VTSLoggingLevel.Warn);
                return;
            }
            Directory.CreateDirectory(options.LogDirectory);
            var time = DateTime.Now.ToString(options.DateTimeFormat);
            string path = Path.Combine(options.LogDirectory, $"{time}{options.FileExtension}");
            var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read);
            var source = new CancellationTokenSource();
            LogWorker(stream, source.Token).Forget();
            Source = source;

            Options = options;
            IsInitialized = true;
        }
        catch (Exception ex)
        {
            IVTSLogger.Default.TryLog(ex);
        }
        finally { Semaphore.Release(); }
    }

    async UniTask LogWorker(FileStream stream, ChannelReader<string> reader, CancellationToken token)
    {
        const int PreventiveFlushThreshold = 8192;
        StreamWriter writer = new(stream, VTSAPI.Encoding);
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (!await reader.WaitToReadAsync(token))
                    return;

                int totalChars = 0;
                while (reader.TryRead(out string input))
                {
                    int length = input.Length;
                    writer.
                }
                foreach (var input in enumerable)
                {
                    int chars = input.
                }

                await foreach (var input in reader.ReadAllAsync(token))
                {
                    reader.
                    writer.WriteLineAsync
                }
            }
        }
        catch (ChannelClosedException) { /* Ignored */ }
        catch (Exception ex)
        {
            var logger = IVTSLogger.Default
        }
    }

    /// <inheritdoc/>
    public async UniTask Terminate()
    {
        await Semaphore.WaitAsync();
        try
        {
            if (!IsInitialized)
            {
                IVTSLogger.Default.TryLog("Already Terminated!", this, VTSLoggingLevel.Warn);
                return;
            }
        }
        catch (Exception ex)
        {
            IVTSLogger.Default.TryLog(ex, this);
        }
        finally { Semaphore.Release(); }
        IsInitialized = false;
    }

    /// <inheritdoc/>
    public void Log(object? input, object? prefix, VTSLoggingLevel level)
    {
        var options = Options;
        if (IsInitialized)
        {

        }
        try
        {

        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    StringBuilder AcquireBuilder() => Builders.TryPop(out var builder) ? builder : new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ReturnBuilder(StringBuilder builder) => Builders.Push(builder);

    /// <inheritdoc/>
    public override string ToString() => $"[{nameof(VTSLogger)}]";

    static class KnownEventArgs
    {
        public static readonly PropertyChangedEventArgs IsInitializedChanged = new(nameof(IsInitialized));
    }
}