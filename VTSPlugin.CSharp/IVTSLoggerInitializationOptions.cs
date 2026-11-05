using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Options to use when initializing <see cref="IVTSLogger{T}"/>.
/// </summary>
/// <remarks>
/// All parameters are frozen after anything starts to use those options.
/// </remarks>
/// Note: Implements <see cref="INotifyPropertyChanged"/> despite never being used, in if used in WPF.
/// See 5th entry here: https://michaelscodingspot.com/ways-to-cause-memory-leaks-in-dotnet
public interface IVTSLoggerInitializationOptions : IVTSInitializationOptions, IEquatable<IVTSLoggerInitializationOptions>
{
	/// <summary>
	/// Default thread-safe (?) options to fallback on, if not provided.
	/// TODO: thread-safety needs to be double checked.
	/// </summary>
	public static IVTSLoggerInitializationOptions Default
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get => VTSLoggerInitializationOptions.Default;
	}

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
	public static readonly string DefaultLogDirectory;
	static IVTSLoggerInitializationOptions()
	{
#if UNITY_2017_1_OR_NEWER
		// Logs should be stored at %UserProfile%/AppData/LocalLow/{CompanyName}/{Product}/Logs.
		// In other words: Application.perstistentDataPath, etc.
		throw new NotSupportedException($"UNITY is not added as a dependency yet.");
#else
		DefaultLogDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
#endif
	}

	/// <summary>
	/// Initial level for <see cref="IVTSLogger"/> to use.
	/// </summary>
	VTSLoggingLevel LogLevel { get; set; }
	/// <summary>
	/// Directory path to where all the logs should be stored.
	/// </summary>
	string LogDirectory { get; set; }
	/// <summary>
	/// Default file name to use.
	/// </summary>
	/// <remarks>
	/// File name is combine with <see cref="DateTime.Now"/> using format: {filename}{separator}{datetime}{extension}.
	/// If <see cref="LogFileName"/> is <see cref="string.IsNullOrEmpty(string)"/> - <see cref="LogFileNameSeparator"/> will be omitted.
	/// </remarks>
	string LogFileName { get; set; }
	/// <summary>
	/// Separator to use between <see cref="LogFileName"/> and <see cref="DateTime.Now"/>,
	/// formatted with <see cref="LogFileNameDateTimeFormat"/>
	/// </summary>
	/// <remarks>
	/// Omitted if <see cref="LogFileName"/> is <see cref="string.IsNullOrEmpty(string)"/>.
	/// </remarks>
	string LogFileNameSeparator { get; set; }
	/// <summary>
	/// Format to use with <see cref="DateTime.Now"/>.
	/// </summary>
	string LogFileNameDateTimeFormat { get; set; }
	/// <summary>
	/// Extension for log files.
	/// </summary>
	/// <remarks>
	/// MUST have a '.' at the beginning!
	/// </remarks>
	string LogFileExtension { get; }

	/// <summary>
	/// Known properties for rising <see cref="PropertyChangedEventHandler"/>s.
	/// </summary>
	public new static class KnownPropertyChangedArgs
	{
		public static readonly PropertyChangedEventArgs LogLevel = new(nameof(IVTSLoggerInitializationOptions.LogLevel));
		public static readonly PropertyChangedEventArgs LogDirectory = new(nameof(IVTSLoggerInitializationOptions.LogDirectory));
		public static readonly PropertyChangedEventArgs LogFileName = new(nameof(IVTSLoggerInitializationOptions.LogFileName));
		public static readonly PropertyChangedEventArgs LogFileNameSeparator = new(nameof(IVTSLoggerInitializationOptions.LogFileNameSeparator));
		public static readonly PropertyChangedEventArgs LogFileNameDateTimeFormat = new(nameof(IVTSLoggerInitializationOptions.LogFileNameDateTimeFormat));
		public static readonly PropertyChangedEventArgs LogFileExtension = new(nameof(IVTSLoggerInitializationOptions.LogFileExtension));
	}
}
