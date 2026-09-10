using System;
using System.Collections.Generic;
using System.Threading;

namespace VTSPlugin;

internal sealed class GenericVTSPacketPool : VTSPacketPool
{
    static class Pool<T> where T : VTSPacketData
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

        static readonly Stack<T> Stack = new(DefaultCapacity);
        static SpinLock Lock = new(); // Should not be readonly.
        public static Y Rent<Y>() where Y : T, new()
        {
            T? result;
            bool taken = false;
            try
            {
                Lock.Enter(ref taken);
                if (Stack.TryPop(out result))
                    goto CastAndReturn; // Cast outside of a lock.
            }
            finally { if (taken) Lock.Exit(true); }
            return new();

            CastAndReturn:
            return (Y)result;
        }

        public static T Rent(Func<T> ctor)
        {
            T? result;
            bool taken = false;
            try
            {
                Lock.Enter(ref taken);
                if (Stack.TryPop(out result))
                    goto CastAndReturn; // Cast outside of a lock.
            }
            finally { if (taken) Lock.Exit(true); }
            return ctor();

            CastAndReturn:
            return result;
        }

        public static void Return(T instance)
        {
            // Note: most memory performant solution will be to reset values after checking if stack has space for the item.
            //  This way, GC will be able to collect the reference with all the fields, without us needing to do extra work.
            //  But doing so requires us resetting the item in the lock scope, which will consume shared resources.
            //  No benchmarking was done on this method, so decision whether to leave it out of the score was made arbitrarily,
            instance.Reset();
            bool taken = false;
            try
            {
                Lock.Enter(ref taken);
                if (Stack.Count < Capacity)
                    Stack.Push(instance);
            }
            finally { if (taken) Lock.Exit(true); }
        }
    }

    public override int GetPoolCapacity<T>() => Pool<T>.Capacity;
    public override void SetPoolCapacity<T>(int capacity) => Pool<T>.Capacity = capacity;
    public override T Rent<T>() => Pool<T>.Rent<T>();
    public override T Rent<T>(Func<T> ctor) => Pool<T>.Rent(ctor);
    public override void Return<T>(T packet) => Pool<T>.Return(packet);
}