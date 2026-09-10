using System;

namespace VTSPlugin;

/// <summary>
/// Special pool, designed specifically for <see cref="VTSPacket"/> and <see cref="VTSPacketData"/>.
/// </summary>
public abstract class VTSPacketPool
{
    protected const int DefaultCapacity = 16;

    public static readonly VTSPacketPool Shared = new GenericVTSPacketPool();

    public abstract int GetPoolCapacity<T>() where T : VTSPacketData;
    public abstract void SetPoolCapacity<T>(int capacity) where T : VTSPacketData;
    public abstract T Rent<T>() where T : VTSPacketData, new();
    public abstract T Rent<T>(Func<T> ctor) where T : VTSPacketData;
    public abstract void Return<T>(T packet) where T : VTSPacketData;
}
