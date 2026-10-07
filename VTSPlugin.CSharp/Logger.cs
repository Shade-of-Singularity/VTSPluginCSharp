using Cysharp.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Test logger implementation.
/// </summary>
public sealed class Logger : IVTSLogger<DefaultLoggerOptions>
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
    public DefaultLoggerOptions Options
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
    } = DefaultLoggerOptions.Instance;


    readonly SemaphoreSlim Semaphore = new(0, 1);
    // Sync version.
    StreamWriter? LogStream;

    /// <inheritdoc/>
    public UniTask<bool> Initialize() => Initialize(DefaultLoggerOptions.Instance);

    /// <inheritdoc/>
    public async UniTask<bool> Initialize(DefaultLoggerOptions options)
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
