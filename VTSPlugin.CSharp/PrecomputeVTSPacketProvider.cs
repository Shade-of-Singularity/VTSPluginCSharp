using System.Buffers;
using System.Text.Json;

namespace VTSPlugin;

/// <summary>
/// Provider for precomputed VTSRequest packets.
/// Allows to save some time on reserialization of commonly used, but immutable packets. like <see cref="Requests.VTSAPIStateRequest.Instance"/>.
/// </summary>
public static class PrecomputeVTSPacketProvider
{
    /// <summary>
    /// Input struct with required data for <see cref="AcquireRequest{T}(PrecomputeRequestInput)"/>.
    /// </summary>
    public readonly ref struct PrecomputeRequestInput
    {
        /// <summary>
        /// API Name to use.
        /// </summary>
        /// <remarks>
        /// If <see cref="HasAPIName"/> is set to <see langword="false"/> - <see cref="VTSAPI.De"/>
        /// </remarks>
        public readonly ReadOnlySpan<byte> APIName { get; init; }
        /// <summary>
        /// Whether <see cref="APIName"/> should be redefined.
        /// Otherwise <see cref="VTSAPI.DefaultAPIName"/> is used.
        /// </summary>
        public readonly bool HasAPIName { get; init; }
        /// <summary>
        /// Version of the API that this request has used.
        /// </summary>
        public readonly ReadOnlySpan<byte> APIVersion { get; init; }
        /// <summary>
        /// Whether <see cref="APIVersion"/> should be redefined.
        /// Otherwise <see cref="VTSAPI.DefaultAPIVersion"/> is used.
        /// </summary>
        public readonly bool HasAPIVersion { get; init; }
        /// <summary>
        /// Request ID to associate with the request.
        /// </summary>
        public readonly required ReadOnlySpan<byte> RequestID { get; init; }
    }
    //public readonly ref struct PrecomputeResponseInput
    //{
    //    /// <summary>
    //    /// API Name to use.
    //    /// </summary>
    //    /// <remarks>
    //    /// If <see cref="HasAPIName"/> is set to <see langword="false"/> - <see cref="VTSAPI.De"/>
    //    /// </remarks>
    //    public readonly ReadOnlySpan<byte> APIName { get; init; }
    //    /// <summary>
    //    /// Whether <see cref="APIName"/> should be redefined.
    //    /// Otherwise <see cref="VTSAPI.DefaultAPIName"/> is used.
    //    /// </summary>
    //    public readonly bool HasAPIName { get; init; }
    //    /// <summary>
    //    /// Version of the API that this request has used.
    //    /// </summary>
    //    public readonly ReadOnlySpan<byte> APIVersion { get; init; }
    //    /// <summary>
    //    /// Whether <see cref="APIVersion"/> should be redefined.
    //    /// Otherwise <see cref="VTSAPI.DefaultAPIVersion"/> is used.
    //    /// </summary>
    //    public readonly bool HasAPIVersion { get; init; }
    //    /// <summary>
    //    /// UNIX timestamp to fill-in.
    //    /// </summary>
    //    public readonly required ReadOnlySpan<byte> Timestamp { get; init; }
    //    /// <summary>
    //    /// Request ID to associate with the request.
    //    /// </summary>
    //    public readonly required ReadOnlySpan<byte> RequestID { get; init; }
    //}
    public readonly struct PrecomputeResult : IDisposable
    {
        /// <summary>
        /// Memory region at which the entire precompute result resides.
        /// </summary>
        public readonly required IMemoryOwner<byte> Memory { get; init; }
        /// <inheritdoc/>
        public void Dispose() => Memory?.Dispose();
    }
    /// <summary>
    /// Precomputes (or Re-computes the Json packet using given <paramref name="instance"/>).
    /// </summary>
    public static void Precompute<T>(T instance, JsonSerializerOptions options) where T : VTSRequest, IPrecomputableVTSRequest
    {
        throw new NotImplementedException();
    }
    /// <remarks>
    /// Only Data-less requests are supported.
    /// </remarks>
    public static PrecomputeResult AcquireRequest<T>(PrecomputeRequestInput input) where T : VTSRequest, IPrecomputableVTSRequest
    {
        throw new NotImplementedException();
    }
    /// <remarks>
    /// Only Data-less request data is supported.
    /// </remarks>
    //public static PrecomputeResult AcquireRequestData<T>(PrecomputeRequestInput input) where T : VTSRequestData, IPrecomputableVTSRequest
    //{
    //    throw new NotImplementedException();
    //}
}