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
public interface IPrecomputableVTSRequest;
