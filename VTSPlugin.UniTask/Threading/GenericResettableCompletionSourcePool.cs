namespace VTSPlugin.Threading;

/// <summary>
/// Default implementation for <see cref="ResettableCompletionSourcePool.Shared"/> based on C#'s generics.
/// </summary>
/// <remarks>
/// Uses C#'s application memory mapping directly (a.k.a. generics).
/// Class should not be instantiated more than once.
/// </remarks>
internal sealed class GenericResettableCompletionSourcePool : ResettableCompletionSourcePool
{
    public static readonly GenericResettableCompletionSourcePool Instance = new();
    /// TODO: Might use implementation from: https://github.com/dotnet/dotnet/blob/main/src/aspnetcore/src/ObjectPool/src/DefaultObjectPool.cs
    /// Or: https://github.com/dotnet/dotnet/blob/b0f34d51fccc69fd334253924abd8d6853fad7aa/src/runtime/src/libraries/System.Collections.Concurrent/src/System/Collections/Concurrent/ConcurrentStack.cs
    static class Pool<T>
    {
        public static int Capacity
        {
            get
            {
                bool taken = false;
                try
                {
                    Lock.Enter(ref taken);
                    return field;
                }
                finally { if (taken) Lock.Exit(true); }
            }
            set
            {
                value = Math.Max(0, value);
                bool taken = false;
                try
                {
                    Lock.Enter(ref taken);
                    if (field != value)
                    {
                        // Truncates the stack.
                        int count = Stack.Count;
                        if (count > value)
                        {
                            do
                            {
                                Stack.Pop();
                                count--; // Avoids re-fetching via Count property.
                            }
                            while (count > value);
                            Stack.TrimExcess();
                        }
                        field = value;
                    }
                }
                finally { if (taken) Lock.Exit(true); }
            }
        } = DefaultCapacity;

        static readonly Stack<ResettableCompletionSource<T>> Stack = new(DefaultCapacity);
        static SpinLock Lock = new(); // Should not be readonly.
        public static ResettableCompletionSource<T> Rent()
        {
            bool taken = false;
            try
            {
                Lock.Enter(ref taken);
                if (Stack.TryPop(out var source))
                    return source;
            }
            finally { if (taken) Lock.Exit(true); }
            return new(); // Creation outside of a lock.
        }

        public static void Return(ResettableCompletionSource<T> source)
        {
            // Note: most memory performant solution will be to reset values after checking if stack has space for the item.
            //  This way, GC will be able to collect the reference with all the fields, without us needing to do extra work.
            //  But doing so requires us resetting the item in the lock scope, which will consume shared resources.
            //  No benchmarking was done on this method, so decision whether to leave it out of the score was made arbitrarily,
            source.Reset();
            bool taken = false;
            try
            {
                Lock.Enter(ref taken);
                if (Stack.Count < Capacity)
                    Stack.Push(source);
            }
            finally { if (taken) Lock.Exit(true); }
        }
    }

    /// <summary>
    /// .ctor is made private so you can never instantiate <see cref="GenericResettableCompletionSourcePool"/> twice.
    /// </summary>
    private GenericResettableCompletionSourcePool() { }

    public override int GetPoolCapacity<T>() => Pool<T>.Capacity;
    public override void SetPoolCapacity<T>(int capacity) => Pool<T>.Capacity = capacity;
    public override ResettableCompletionSource<T> Rent<T>() => Pool<T>.Rent();
    public override void Return<T>(ResettableCompletionSource<T> source) => Pool<T>.Return(source);
}