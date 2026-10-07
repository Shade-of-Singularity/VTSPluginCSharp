namespace VTSPlugin;

/// <summary>
/// Allows to freeze/unfreeze values in a specific instance.
/// <see cref="Pooling.IVTSPoolable.Reset"/> usually automatically unfreezes the class as well.
/// </summary>
/// <remarks>
/// Once frozen, values of a class/struct cannot be modified.
/// Used in <see cref="IInitializationOptions"/> to ensure that you cannot modify options after something started to use it.
/// </remarks>
public interface IFreezable
{
    /// <summary>
    /// Whether values of a class/struct is frozen.
    /// </summary>
    bool IsFrozen { get; }
    /// <summary>
    /// Freezes values of this class/struct, preventing any modifications.
    /// </summary>
    void Freeze();
    /// <summary>
    /// Unfreezes values of this class/struct, allowing modifications.
    /// </summary>
    /// <remarks>
    /// Unfreezing options from <see cref="IService{T}.Options"/> is invalid!
    /// This can lead to undefined behaviour!
    /// </remarks>
    void Unfreeze();
}
