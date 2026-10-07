using Cysharp.Threading.Tasks;

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
    /// <remarks>
    /// <paramref name="request"/> is encoded immediately, before <see cref="UniTask"/> state machine returns.
    /// This makes it possible for a caller to immediately release back <paramref name="request"/>
    /// to <see cref="Pooling.VTSPool"/>, or any other pooling abstract.
    /// </remarks>
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
}
