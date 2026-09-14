using Cysharp.Threading.Tasks;

namespace VTSPlugin;

/// <summary>
/// Useful extensions for working with <see cref="IVTSConnection"/>.
/// </summary>
public static class VTSRequestManagerExtensions
{
    /// <inheritdoc cref="IVTSRequestManager.Request{T}(VTSRequestTemplate, TimeSpan)"/>
    /// <remarks>
    /// Request will have an infinite timeout.
    /// </remarks>
    /// <seealso cref="Timeout.InfiniteTimeSpan"/>
    public static UniTask<RequestResult<T>> Request<T>(this IVTSRequestManager manager, VTSRequestTemplate request)
        where T : VTSResponseData
    {
        return manager.Request<T>(request, Timeout.InfiniteTimeSpan);
    }
}
