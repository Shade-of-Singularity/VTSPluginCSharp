using System;

namespace VTSPlugin.Threading;

/// <summary>
/// Pool for <see cref="ReusableCompletionSource"/>s.
/// </summary>
/// Note: Standard .NET implementation uses <see cref="System.Collections.Concurrent.ConcurrentQueue{T}"/> under the hood.
///  This makes "cold" data structures be used first, which is bad for performance.
///  Thus we implement custom pool based on <see cref="System.Collections.Concurrent.ConcurrentStack{T}"/>.
///  In our future developments, we can consider making our own two abstracts, based on each of them, for different applications.
/// See also: https://github.com/dotnet/dotnet/blob/main/src/aspnetcore/src/ObjectPool/src/DefaultObjectPool.cs
public sealed class CompletionSourcePool : StackPool<>
{
}

public abstract class StackPool<T>
{
    /// <summary>
    /// Rents a new <typeparamref name="T"/> item from the pool, or creates a new one.
    /// </summary>
    /// <returns></returns>
    public abstract T Rent();
    /// <summary>
    /// Returns <paramref name="item"/> to the pool.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> - if item was returned successfully.
    /// <see langword="false"/> - if capacity reached, or item was no pooled by any other reason.
    /// </returns>
    public abstract bool Return(T item);
}

public abstract class StackPool
{
    public static StackPool<T> Create<T>()
    {
        throw new NotImplementedException();
    }
}