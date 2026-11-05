namespace VTSPlugin;

/// <summary>
/// Indicates that a specific <see cref="VTSRequest"/> can be precomputed.
/// As long as <see cref="VTSAPI.DefaultAPIName"/> and <see cref="VTSAPI.DefaultAPIVersion"/> match,
/// system will reuse JSON packets from previous session 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class VTSPrecomputableAttribute : Attribute;