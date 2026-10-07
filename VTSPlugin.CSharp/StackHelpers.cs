namespace VTSPlugin;

/// <summary>
/// Stack helpers for <see cref="VTSPlugin"/> specifically.
/// </summary>
/// Note: made to be internal, because the name of the class might conflict with user-defined names.
///  I don't want to force our utility class on a developer, using our library.
internal static class StackHelpers
{
    /// <summary>
    /// After how many bytes, <see langword="stackalloc"/> should be replaced with arrays,
    /// rented from <see cref="System.Buffers.ArrayPool{T}.Shared"/>
    /// </summary>
    public const int StackAllocByteThreshold = 256;
}
