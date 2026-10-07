using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Indicates that modifications on a specific member are frozen/blocked.
/// Appears when you try to modify members of a class/struct, implementing <see cref="IFreezable"/>.
/// </summary>
public sealed class MemberFrozenException : VTSException
{
    /// <inheritdoc/>
    public MemberFrozenException() : base() { }

    /// <inheritdoc/>
    public MemberFrozenException(string message) : base(message) { }

    /// <inheritdoc/>
    public MemberFrozenException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Throws <see cref="MemberFrozenException"/>.
    /// </summary>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Throw() => throw new MemberFrozenException("Type members are is frozen! Cannot modify.");

    /// <summary>
    /// Throws <see cref="MemberFrozenException"/>, specifying which member specifically was accessed.
    /// </summary>
    /// <param name="member">Encapsulated member that was attempted to be modified.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Throw(string member) => throw new MemberFrozenException($"Member ({member}) is frozen and cannot be modified!");

    /// <summary>
    /// Throws <see cref="MemberFrozenException"/>, specifying which class/struct, declaring accessed member, was modified.
    /// </summary>
    /// <param name="declarator">Class/struct, declaring a given <paramref name="member"/>.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Throw(Type declarator) => throw new MemberFrozenException($"({declarator.Name}) is frozen and cannot be modified!");

    /// <summary>
    /// Throws <see cref="MemberFrozenException"/>, specifying which member specifically was accessed.
    /// </summary>
    /// <param name="declarator">Class/struct, declaring a given <paramref name="member"/>.</param>
    /// <param name="member">Encapsulated member that was attempted to be modified.</param>
    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Throw(Type declarator, string member) => throw new MemberFrozenException($"Member ({declarator.Name}.{member}) is frozen and cannot be modified!");
}
