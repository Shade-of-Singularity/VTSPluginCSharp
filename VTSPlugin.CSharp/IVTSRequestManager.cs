using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace VTSPlugin;

/// <summary>
/// Interface for an abstract, capable of sending requests somewhere else.
/// </summary>
/// TODO: Rename from Manager to something more sensible for an abstract, sending requests.
///  Carrier? Sender? But it also receives them and keeps track of RequestIDs and timeouts...
public interface IVTSRequestManager
{
    /// <summary>
    /// Sends request to a VTube Studio instance.
    /// <para>
    /// Returns <see cref="RequestResult{T}.FromFailed(ErrorID, ReadOnlyMemory{byte})"/>
    /// if VTube Studio is disconnected, or if any other issues occur.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of response to expect to receive.</typeparam>
    /// <param name="request"><see cref="VTSRequest"/> to send.</param>
    /// <param name="timeout">
    /// How long until the request should automatically timeout.
    /// Provide <see cref="Timeout.InfiniteTimeSpan"/> if request should never timeout
    /// (This is already implemented in most <see cref="VTSRequestManagerExtensions"/> methods).
    /// </param>
    /// <returns>
    /// Request result <typeparamref name="T"/> if response received successfully.
    /// <see cref="RequestResult{T}.FromFailed(ErrorID, ReadOnlyMemory{byte})"/> otherwise.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/></exception>
    UniTask<RequestResult<T>> Request<T>(VTSRequestTemplate request, TimeSpan timeout) where T : VTSResponseData;

    /// <remarks>
    /// Uses pre-computed request info instead of serializing a static instance.
    /// <para>
    /// When implementing this class properly, make sure to use <see cref="RuntimeHelpers.RunClassConstructor"/> on <typeparamref name="Y"/>.
    /// This will register <see cref="IPrecomputableVTSRequest"/> to a precompute list.
    /// </para>
    /// </remarks>
    /// <inheritdoc cref="Request{T}(VTSRequestTemplate, TimeSpan)"/>
    UniTask<RequestResult<T>> Request<T, Y>(TimeSpan timeout) where T : VTSResponseData where Y : VTSRequestTemplate, IPrecomputableVTSRequest;
}
