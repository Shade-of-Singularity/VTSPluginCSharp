using System.ComponentModel;

namespace VTSPlugin;

/// <summary>
/// Options to use when initializing <see cref="IVTSLogger{T}"/>.
/// </summary>
/// Note: Implements <see cref="INotifyPropertyChanged"/> despite never being used, in if used in WPF.
/// See 5th entry here: https://michaelscodingspot.com/ways-to-cause-memory-leaks-in-dotnet
public interface IVTSLoggerInitializationOptions : IInitializationOptions, IEquatable<IVTSLoggerInitializationOptions>
{
    /// <summary>
    /// Default thread-safe (?) options to fallback on, if not provided.
    /// TODO: thread-safety needs to be double checked.
    /// </summary>
    public static IVTSLoggerInitializationOptions Default => DefaultLoggerOptions.Instance;
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
