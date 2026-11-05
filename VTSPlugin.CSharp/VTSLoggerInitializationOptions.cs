using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Default options implementation for use with <see cref="IVTSLogger{T}"/>s.
/// </summary>
public sealed class VTSLoggerInitializationOptions() : IVTSLoggerInitializationOptions, IEquatable<VTSLoggerInitializationOptions>
{
	/// <summary>
	/// Default logger options to use.
	/// </summary>
	public static readonly VTSLoggerInitializationOptions Default = new()
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

	/// <summary>
	/// Automatically freezes <see cref="Default"/> instance, so no one changes it by a mistake.
	/// </summary>
	static VTSLoggerInitializationOptions() => Default.Freeze();

	// TODO: Double-check what happens to IsFrozen under concurrency.
	/// <remarks>
	/// Doesn't influence the <see cref="Equals(VTSLoggerInitializationOptions)"/> check.
	/// </remarks>
	/// <inheritdoc/>
	public bool IsFrozen
	{
		get => field;
		set
		{
#if WPF
			var callback = PropertyChanged;
			if (callback is not null)
			{
				if (field == value) return;

				field = value;
				PropertyChanged?.Invoke(this, IVTSLoggerInitializationOptions.KnownPropertyChangedArgs.LogLevel);
				return;
			}
#endif
			field = value;
		}
	}

	/// <inheritdoc/>
	public required VTSLoggingLevel LogLevel
	{
		get => field;
		set
		{
			if (IsFrozen)
				MemberFrozenException.Throw(typeof(VTSLoggerInitializationOptions), nameof(LogLevel));
#if WPF
			var callback = PropertyChanged;
			if (callback is not null)
			{
				if (field == value) return;

				field = value;
				PropertyChanged?.Invoke(this, IVTSLoggerInitializationOptions.KnownPropertyChangedArgs.LogLevel);
				return;
			}
#endif
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
				MemberFrozenException.Throw(typeof(VTSLoggerInitializationOptions), nameof(LogLevel));
#if WPF
			var callback = PropertyChanged;
			if (callback is not null)
			{
				if (field == value) return;

				field = value;
				PropertyChanged?.Invoke(this, IVTSLoggerInitializationOptions.KnownPropertyChangedArgs.LogDirectory);
				return;
			}
#endif
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
				MemberFrozenException.Throw(typeof(VTSLoggerInitializationOptions), nameof(LogLevel));
#if WPF
			var callback = PropertyChanged;
			if (callback is not null)
			{
				if (field == value) return;

				field = value;
				PropertyChanged?.Invoke(this, IVTSLoggerInitializationOptions.KnownPropertyChangedArgs.LogFileName);
				return;
			}
#endif
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
				MemberFrozenException.Throw(typeof(VTSLoggerInitializationOptions), nameof(LogLevel));
#if WPF
			var callback = PropertyChanged;
			if (callback is not null)
			{
				if (field == value) return;

				field = value;
				PropertyChanged?.Invoke(this, IVTSLoggerInitializationOptions.KnownPropertyChangedArgs.LogFileNameDateTimeFormat);
				return;
			}
#endif
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
				MemberFrozenException.Throw(typeof(VTSLoggerInitializationOptions), nameof(LogLevel));
#if WPF
			var callback = PropertyChanged;
			if (callback is not null)
			{
				if (field == value) return;

				field = value;
				PropertyChanged?.Invoke(this, IVTSLoggerInitializationOptions.KnownPropertyChangedArgs.LogFileExtension);
				return;
			}
#endif
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
				MemberFrozenException.Throw(typeof(VTSLoggerInitializationOptions), nameof(LogLevel));
#if WPF
			var callback = PropertyChanged;
			if (callback is not null)
			{
				if (field == value) return;

				field = value;
				callback(this, IVTSLoggerInitializationOptions.KnownPropertyChangedArgs.LogFileNameSeparator);
				return;
			}
#endif
			field = value;
		}
	} = IVTSLoggerInitializationOptions.DefaultLogFileNameSeparator;

	/// <inheritdoc/>
	public void Freeze() => IsFrozen = true;

	/// <inheritdoc/>
	public void Unfreeze() => IsFrozen = false;

#if WPF
	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;
#endif

	/// <inheritdoc/>
	public override bool Equals(object? obj) => obj is VTSLoggerInitializationOptions other && Equals(other);

	/// <inheritdoc/>
	public bool Equals(VTSLoggerInitializationOptions? other)
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
		PropertyChanged = null;
		LogLevel = VTSLoggingLevel.Info;
		LogFileName = IVTSLoggerInitializationOptions.DefaultLogFileName;
		LogDirectory = IVTSLoggerInitializationOptions.DefaultLogDirectory;
		LogFileExtension = IVTSLoggerInitializationOptions.DefaultLogFileExtension;
		LogFileNameDateTimeFormat = IVTSLoggerInitializationOptions.DefaultFileNameDateTimeFormat;
		IsFrozen = false;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(VTSLoggerInitializationOptions? a, VTSLoggerInitializationOptions? b) => a is null ? b is null : a.Equals(b);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(VTSLoggerInitializationOptions? a, VTSLoggerInitializationOptions? b) => a is null ? b is not null : !a.Equals(b);
}
