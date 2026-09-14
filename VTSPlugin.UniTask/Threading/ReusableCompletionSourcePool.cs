namespace VTSPlugin.Threading;

/// <summary>
/// Special pool, designed specifically for <see cref="ResettableCompletionSource{T}"/>s.
/// </summary>
/// Note: Standard .NET implementation uses <see cref="System.Collections.Concurrent.ConcurrentQueue{T}"/> under the hood.
///  This makes "cold" data structures be used first, which is bad for performance.
///  Thus we implement custom pool based on <see cref="System.Collections.Concurrent.ConcurrentStack{T}"/>.
///  In our future developments, we can consider making our own two abstracts, based on each of them, for different applications.
/// See also: https://github.com/dotnet/dotnet/blob/main/src/aspnetcore/src/ObjectPool/src/DefaultObjectPool.cs
public abstract class ResettableCompletionSourcePool
{
    public static readonly ResettableCompletionSourcePool Shared = GenericResettableCompletionSourcePool.Instance;
    protected static readonly int DefaultCapacity = Environment.ProcessorCount * 2;

    public abstract int GetPoolCapacity<T>();
    public abstract void SetPoolCapacity<T>(int capacity);
    public abstract ResettableCompletionSource<T> Rent<T>();
    public abstract void Return<T>(ResettableCompletionSource<T> source);
}
