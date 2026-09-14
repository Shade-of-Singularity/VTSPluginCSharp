using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;

namespace VTSPlugin;

public sealed class ConsoleVTSLogger : IVTSLogger
{
    public VTSLoggingLevel LogLevel
    {
        get => (VTSLoggingLevel)InterlockedHelpers.Read(ref Unsafe.As<VTSLoggingLevel, int>(ref field));
        set => Interlocked.Exchange(ref Unsafe.As<VTSLoggingLevel, int>(ref field), (int)value);
    } = VTSLoggingLevel.Info;


    readonly ConcurrentStack<StringBuilder> Builders = new();
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
}
