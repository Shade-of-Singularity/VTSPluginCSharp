using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Configuration for packet precomputation in <see cref="IVTSPacketSerializer"/>.
/// </summary>
public readonly struct VTSPrecomputeConfiguration() : IEquatable<VTSPrecomputeConfiguration>
{
    /// <summary>
    /// Configuration with default options from <see cref="VTSAPI"/>.
    /// </summary>
    public static readonly VTSPrecomputeConfiguration Default = new();
    /// <summary>
    /// API name to use in all precomputed packets.
    /// </summary>
    public readonly string APIName { get; init; } = VTSAPI.DefaultAPIName;
    /// <summary>
    /// API version to use in all precomputed packets.
    /// </summary>
    public readonly string APIVersion { get; init; } = VTSAPI.DefaultAPIVersion;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VTSPrecomputeConfiguration other && this == other;

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(VTSPrecomputeConfiguration other) => this == other;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(APIName, APIVersion);

    /// <inheritdoc/>
    public override string ToString() => $"({nameof(APIName)}: {APIName}, {nameof(APIVersion)}: {APIVersion})";

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(VTSPrecomputeConfiguration a, VTSPrecomputeConfiguration b)
    {
        return a.APIName == b.APIName
            && a.APIVersion == b.APIVersion;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(VTSPrecomputeConfiguration a, VTSPrecomputeConfiguration b)
    {
        return a.APIName != b.APIName
            || a.APIVersion != b.APIVersion;
    }
}