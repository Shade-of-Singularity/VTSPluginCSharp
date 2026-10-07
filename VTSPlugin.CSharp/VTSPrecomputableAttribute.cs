namespace VTSPlugin;

/// <summary>
/// Indicates that a specific <see cref="VTSRequest"/> can be precomputed.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class VTSPrecomputableAttribute : Attribute;