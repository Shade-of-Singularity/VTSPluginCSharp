using System.Buffers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace VTSPlugin;



/// <summary>
/// Interface for implementing JSON serializers for VTS packets.
/// </summary>
public interface IVTSPacketSerializer
{
    /// <summary>
    /// Default implementation for <see cref="IVTSPacketSerializer"/>.
    /// </summary>
    public static IVTSPacketSerializer Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => DefaultVTSPacketSerializer.Instance;
    }
    /// <summary>
    /// Configuration for packet precomputing (for those that support it).
    /// </summary>
    VTSPrecomputeConfiguration PrecomputeConfiguration { get; set; }
    /// <summary>
    /// Processor for precomputing the packets.
    /// Defaults to <see cref="VTSPrecomputeProcessor.Default"/>
    /// </summary>
    VTSPrecomputeProcessor PrecomputeProcessor { get; set; }
    /// <summary>
    /// Serializes packet into JSON without precomputing.
    /// </summary>
    /// <param name="instance">Instance of <typeparamref name="T"/> to serialize.</param>
    /// <param name="allowPrecompute">Whether to check if given <typeparamref name="T"/> can be precomputed.</param>
    /// <returns>Packet, serialized as JSON.</returns>
    string Serialize<T>(T instance, bool allowPrecompute = true) where T : VTSPacketTemplate;
    /// <summary>
    /// Deserializes packet data into a provided <paramref name="instance"/>.
    /// </summary>
    /// <param name="json">Raw JSON packet.</param>
    /// <param name="instance">Packet instance to populate.</param>
    void Deserialize<T>(ReadOnlyMemory<char> json, T instance) where T : VTSPacketTemplate;
}

/// <summary>
/// Seals implementation of <see cref="AbstractVTSPacketSerializer"/>.
/// Sealed for performance reasons.
/// Provides no functionality besides sealing <see cref="AbstractVTSPacketSerializer"/>.
/// </summary>
/// <remarks>
/// To modify its functionality, inherit <see cref="AbstractVTSPacketSerializer"/> instead.
/// </remarks>
public sealed class DefaultVTSPacketSerializer : AbstractVTSPacketSerializer
{
    /// <summary>
    /// Default instance of serializer to use.
    /// </summary>
    /// Note: Referencing this field directly allows JIT to devirtualize calls to <see cref="IVTSPacketSerializer.Default"/>.

    public static readonly DefaultVTSPacketSerializer Instance = new();
    /// <remarks>
    /// <see cref="Instance"/> should be used instead of creating a new one.
    /// </remarks>
    private DefaultVTSPacketSerializer() { }
}

/// <summary>
/// Default inheritable implementation for <see cref="IVTSPacketSerializer"/>.
/// </summary>
public abstract class AbstractVTSPacketSerializer : IVTSPacketSerializer, INotifyPropertyChanged
{

}

/// <summary>
/// Interface for custom precompute processors.
/// </summary>
public abstract class VTSPrecomputeProcessor : INotifyPropertyChanged
{
    /// <summary>
    /// Default implementation for <see cref="VTSPrecomputeProcessor"/>.
    /// </summary>
    public static VTSPrecomputeProcessor Default
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => DefaultVTSPrecomputeProcessor.Instance;
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Whether given instance of type <typeparamref name="T"/> should be precomputed.
    /// </summary>
    /// <remarks>
    /// Skipped if user explicitly blocked precomputing.
    /// </remarks>
    public abstract bool ShouldPrecompute<T>(T instance) where T : VTSPacketTemplate;
    /// <summary>
    /// Resets precompute information for all registered types.
    /// </summary>
    public abstract void Reset();
    /// <summary>
    /// Resets precompute information for a specific type <typeparamref name="T"/>.
    /// </summary>
    public abstract void Reset<T>() where T : VTSPacketTemplate;
    /// <summary>
    /// Serializes the packet using 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <returns></returns>
    public abstract string Precompute<T>(T instance) where T : VTSPacketTemplate;

    /// <summary>
    /// Invokes <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <inheritdoc cref="PropertyChangedEventHandler"/>.
    protected void InvokePropertyChanged(object sender, PropertyChangedEventArgs e) => PropertyChanged?.Invoke(sender, e);
}

/// <summary>
/// Seals implementation of <see cref="AbstractVTSPacketSerializer"/>.
/// Sealed for performance reasons.
/// Provides no functionality besides sealing <see cref="AbstractVTSPacketSerializer"/>.
/// </summary>
/// <remarks>
/// To modify processor behaviour, inherit from <see cref="AbstractVTSPrecomputeProcessor"/>.
/// </remarks>
public sealed class DefaultVTSPrecomputeProcessor : AbstractVTSPrecomputeProcessor
{
    /// <summary>
    /// Singleton instance of this processor.
    /// </summary>
    public static readonly DefaultVTSPrecomputeProcessor Instance = new();
    /// <remarks>
    /// <see cref="Instance"/> should be used instead of creating a new one.
    /// </remarks>
    private DefaultVTSPrecomputeProcessor() { }
}

/// <summary>
/// Default finalized implementation for <see cref="VTSPrecomputeProcessor"/> using C# generics.
/// </summary>
/// <remarks>
/// Can be inherited to modify its behaviour.
/// </remarks>
public abstract class AbstractVTSPrecomputeProcessor() : VTSPrecomputeProcessor
{

}

/// <summary>
/// Configuration for <see cref="VTSPrecomputePacketProvider"/>
/// </summary>
public readonly struct VTSPrecomputeConfiguration() : IEquatable<VTSPrecomputeConfiguration>
{
    /// <summary>
    /// Configuration with default options from <see cref="VTSAPI"/>.
    /// </summary>
    /// <remarks>
    /// Unless VTube Studio changes API version, <see cref="Default"/> should do just fine.
    /// </remarks>
    public static readonly VTSPrecomputeConfiguration Default = new();
    /// <summary>
    /// API name to use in all precomputed packets.
    /// </summary>
    public readonly string APIName { get; init; } = VTSAPI.DefaultAPIName;
    /// <summary>
    /// API version to use in all precomputed packets.
    /// </summary>
    public readonly string APIVersion { get; init; } = VTSAPI.DefaultAPIVersion;
    readonly int HashCode

    public override bool Equals(object obj) => obj is VTSPrecomputeConfiguration configuration && Equals(configuration);
    public bool Equals(VTSPrecomputeConfiguration other)
    {

    }

    public override int GetHashCode()
    {
        return
    }
}

public sealed class VTSPrecomputeConfigurationEqualityComparer : EqualityComparer<VTSPrecomputeConfiguration>
{
    public static readonly VTSPrecomputeConfigurationEqualityComparer Instance = new();
    private VTSPrecomputeConfigurationEqualityComparer() { }
    public override bool Equals(VTSPrecomputeConfiguration x, VTSPrecomputeConfiguration y) => x.Equals(y);
    public override int GetHashCode(VTSPrecomputeConfiguration obj)
    {
        return HashCode.Combine(obj.APIName, obj.APIVersion);
    }
}

/// <summary>
/// Implementation of <see cref="VTSPrecomputePacketProvider"/> implemented via C# generics.
/// </summary>
public sealed class GenericVTSPrecomputePacketProvider : VTSPrecomputePacketProvider
{
    /// <summary>
    /// Global instance to be used everywhere.
    /// </summary>
    public static readonly GenericVTSPrecomputePacketProvider Instance = new();
}

/// <summary>
/// Provider for precomputed VTSRequest packets.
/// Allows to save some time on reserialization of commonly used, but immutable packets. like <see cref="Requests.VTSAPIStateRequest.Instance"/>.
/// </summary>
public abstract class VTSPrecomputePacketProvider
{
    /// <summary>
    /// Instance to be used globally.
    /// </summary>
    public static VTSPrecomputePacketProvider Shared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GenericVTSPrecomputePacketProvider.Instance;
    }

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