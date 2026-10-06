using System.Runtime.CompilerServices;

namespace VTSPlugin.Pooling;

/// <summary>
/// Special pool, designed specifically for <see cref="VTSPlugin"/> needs.
/// </summary>
public abstract class VTSPool
{
    /// <summary>
    /// Shared default implementation for <see cref="VTSPool"/> to be used globally.
    /// </summary>
    public static VTSPool Shared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GenericVTSPool.Instance;
    }
    /// <summary>
    /// Default capacity (per item type) for the pool to have.
    /// </summary>
    protected static readonly int DefaultCapacity = Environment.ProcessorCount * 2;
    /// <summary>
    /// Gets pool capacity for a specific type of item.
    /// </summary>
    public abstract int GetPoolCapacity<T>() where T : IVTSPoolable;
    /// <summary>
    /// Sets capacity for a specific type of item.
    /// </summary>
    public abstract void SetPoolCapacity<T>(int capacity) where T : IVTSPoolable;
    /// <summary>
    /// Rents an instance of <typeparamref name="T"/> from a pool, or constructs a new one using default .ctor.
    /// </summary>
    public abstract T Rent<T>() where T : IVTSPoolable, new();
    /// <summary>
    /// Rents an instance of <typeparamref name="T"/> from a pool, or constructs a new one using <paramref name="ctor"/> function.
    /// </summary>
    public abstract T Rent<T>(Func<T> ctor) where T : IVTSPoolable;
    /// <summary>
    /// Returns <typeparamref name="T"/> to the pool.
    /// Automatically resets instance using <see cref="IVTSPoolable.Reset"/>.
    /// </summary>
    public abstract void Return<T>(T packet) where T : IVTSPoolable;
}