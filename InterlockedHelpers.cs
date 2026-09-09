using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

internal static class InterlockedHelpers
{
    /// <inheritdoc cref="Interlocked.CompareExchange(ref long, long, long)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Read(ref ulong field)
    {
#if NETSTANDARD
        return Unsafe.As<long, ulong>(
            ref Unsafe.AsRef(
                Interlocked.CompareExchange(
                    ref Unsafe.As<ulong, long>(ref field),
                    default,
                    default)));
#else
        return Interlocked.CompareExchange(ref field, default, default);
#endif
    }

    /// <inheritdoc cref="Interlocked.CompareExchange(ref int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Read(ref uint field)
    {
#if NETSTANDARD
        return Unsafe.As<int, uint>(
            ref Unsafe.AsRef(
                Interlocked.CompareExchange(
                    ref Unsafe.As<uint, int>(ref field),
                    default,
                    default)));
#else
        return Interlocked.CompareExchange(ref field, default, default);
#endif
    }

    /// <inheritdoc cref="Interlocked.CompareExchange(ref IntPtr, IntPtr, IntPtr)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntPtr Read(ref IntPtr field) => Interlocked.CompareExchange(ref field, default, default);

    /// <inheritdoc cref="Interlocked.CompareExchange(ref float, float, float)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Read(ref float field) => Interlocked.CompareExchange(ref field, default, default);

    /// <inheritdoc cref="Interlocked.CompareExchange(ref object?, object?, object?)"/>
    [return: NotNullIfNotNull(nameof(field))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object? Read(ref object? field) => Interlocked.CompareExchange(ref field, default, default);

    /// <inheritdoc cref="Interlocked.CompareExchange(ref int, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Read(ref int field) => Interlocked.CompareExchange(ref field, default, default);

    /// <inheritdoc cref="Interlocked.CompareExchange{T}(ref T, T, T)"/>
    [return: NotNullIfNotNull(nameof(field))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Read<T>(ref T field) where T : class => Interlocked.CompareExchange(ref field!, default, default)!;

    /// <inheritdoc cref="Interlocked.CompareExchange(ref double, double, double)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Read(ref double field) => Interlocked.CompareExchange(ref field, default, default);

    /// <inheritdoc cref="Interlocked.CompareExchange(ref long, long, long)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Read(ref long field) => Interlocked.CompareExchange(ref field, default, default);
}
