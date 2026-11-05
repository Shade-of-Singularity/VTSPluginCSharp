using System.ComponentModel;

namespace VTSPlugin;

/// <summary>
/// Allows to freeze/unfreeze values in a specific instance.
/// <see cref="Pooling.IVTSPoolable.Reset"/> usually automatically unfreezes the class as well.
/// </summary>
public interface IVTSFreezable
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
    void Unfreeze();
}
