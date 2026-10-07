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
public sealed class ConsoleVTSLogger : IVTSLogger
{
    public VTSLoggingLevel LogLevel
    {
        get => (VTSLoggingLevel)Volatile.Read(ref Unsafe.As<VTSLoggingLevel, int>(ref field));
        set => Volatile.Write(ref Unsafe.As<VTSLoggingLevel, int>(ref field), (int)value);
    } = VTSLoggingLevel.Info;

    public IVTSLoggerInitializationOptions Options => throw new NotImplementedException();

    public bool IsInitialized => throw new NotImplementedException();

    readonly ConcurrentStack<StringBuilder> Builders = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Log(object? input, object? prefix, VTSLoggingLevel level)
    {
        StringBuilder builder = AcquireBuilder();
        try
        {
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
        }
        //catch (Exception ex) { Console.WriteLine(ex.ToDisplay()); }
        finally { ReturnBuilder(builder); }
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    StringBuilder AcquireBuilder() => Builders.TryPop(out var builder) ? builder : new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ReturnBuilder(StringBuilder builder) => Builders.Push(builder);

    public UniTask<bool> Initialize()
    {
        throw new NotImplementedException();
    }

    public UniTask<bool> Terminate()
    {
        throw new NotImplementedException();
    }
}
