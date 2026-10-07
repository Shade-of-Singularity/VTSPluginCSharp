namespace VTSPlugin;

/// <summary>
/// Default equality comparer for <see cref="VTSPrecomputeConfiguration"/>s.
/// </summary>
public sealed class VTSPrecomputeConfigurationEqualityComparer : EqualityComparer<VTSPrecomputeConfiguration>
{
    /// <summary>
    /// Static instance to use everywhere.
    /// </summary>
    public static readonly VTSPrecomputeConfigurationEqualityComparer Instance = new();

    /// <summary>
    /// Hidden .ctor to prevent users from instantiating redundant instances.
    /// </summary>
    private VTSPrecomputeConfigurationEqualityComparer() { }

    /// <inheritdoc/>
    public override bool Equals(VTSPrecomputeConfiguration x, VTSPrecomputeConfiguration y) => x.Equals(y);

    /// <inheritdoc/>
    public override int GetHashCode(VTSPrecomputeConfiguration obj) => obj.GetHashCode();
}
