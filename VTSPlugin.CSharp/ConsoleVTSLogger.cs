using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace VTSPlugin;

/// <summary>
/// System console-based implementation for <see cref="IVTSLogger"/>.
/// Works always, but after initialization - also manages a log file.
/// </summary>
public sealed class ConsoleVTSLogger : IVTSLogger<IVTSLoggerInitializationOptions>
{
	public VTSLoggingLevel LogLevel
	{
		get => (VTSLoggingLevel)Volatile.Read(ref Unsafe.As<VTSLoggingLevel, int>(ref field));
		set => Volatile.Write(ref Unsafe.As<VTSLoggingLevel, int>(ref field), (int)value);
	} = VTSLoggingLevel.Info;

	public IVTSLoggerInitializationOptions Options { get; private set; }
	public bool IsInitialized { get; private set; }

#if NOTIFY_CHANGED
	public event PropertyChangedEventHandler? PropertyChanged;
#endif

	[ThreadStatic]
	static StringBuilder? Builder;
	SpinLock Lock = new();

	/// <inheritdoc/>
	public void Log(object? input, object? prefix, VTSLoggingLevel level)
	{
		StringBuilder? builder = Builder;
		if (builder is null)
		{
			builder = new();
			Builder = builder;
		}
		else builder.Clear();

		// Appends prefix as well.
		// Might be messy, but I don't mind.

		// Special logging prefix for exceptions.
		if (input is Exception ex)
		{
			Append(builder, ex);
			static void Append(StringBuilder builder, Exception ex)
			{
				builder.Append(ex.GetType());
				builder.Append(": ");
				builder.AppendLine(ex.Message);
				builder.Append(ex.StackTrace);
				var inner = ex.InnerException;
				if (inner is not null)
				{
					Append(builder, inner);
				}
			}
		}
		else
		{
			builder.Append(input);
		}

		// Tries to log to a file.
		try
		{

		}
		catch (Exception ex)
		{

		}


		//ConsoleColor last = Console.ForegroundColor;
		//Console.ForegroundColor = color;

		//builder.Clear();
		//if (prefix is not null)
		//{
		//    builder.Append(prefix);
		//    if (!prefix.EndsWith(' ') && !prefix.EndsWith('\t'))
		//        builder.Append(' ');
		//}
		//builder.Append((obj ?? "null").ToString());
		//string final = builder.ToString();
		//builder.Clear();


		//Console.WriteLine(final);
		//Console.ForegroundColor = last;
		//LoggerService.Instance.Log(final);
		throw new NotImplementedException();
	}

	public UniTask<bool> Initialize() => Initialize(IVTSLoggerInitializationOptions.Default);
	public UniTask<bool> Terminate()
	{
		IsInitialized = false;
		Options = null;
	}

	public UniTask<bool> Initialize(IVTSLoggerInitializationOptions options)
	{
		Options = options;
		IsInitialized = true;
	}
}
