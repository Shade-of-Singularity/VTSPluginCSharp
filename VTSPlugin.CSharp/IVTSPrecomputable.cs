namespace VTSPlugin;

/// <summary>
/// Declares <see cref="VTSRequest"/>s, which can be precomputed to save resources.
/// </summary>
/// <remarks>
/// Maybe in the future we will simply compare a commonly known static instance in each extension method.
/// If instance is the same - use precomputed value.
/// I think it will be more performant, and the intent will be a lot clearer.
/// And we can precompute data directly in the instance.
/// For example - in <see cref="Requests.VTSAPIStateRequest"/> based on <see cref="Requests.VTSAPIStateRequest.Instance"/>
/// </remarks>
public interface IVTSPrecomputable
{
    /// <summary>
    /// Checks whether this object matches provided precompute configuration.
    /// </summary>
    bool AllowPrecompute(VTSPrecomputeConfiguration configuration);
}

/// <summary>
/// Indicates that a specific <see cref="VTSResponse"/> can be precomputed.
/// </summary>
/// <remarks>
/// You should not modify built-in static instances, as it will not update precomputed values!
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class VTSPrecomputableAttribute : Attribute;