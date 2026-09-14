using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using VTSPlugin;
using VTSPlugin.Legacy;
using VTSPlugin.Requests;

namespace VTSPlugin;

/// <summary>
/// Information about the plugin.
/// </summary>
public interface IVTSPlugin
{
    public const string DefaultName = "Unknown";
    public const string DefaultAuthor = "Unknown";
    public const string DefaultIcon = null;

    /// <summary>
    /// Plugin name.
    /// Will be displayed in VTube Studio's plugin authentication popup.
    /// </summary>
    /// <remarks>
    /// Do NOT access in hot paths!
    /// Default implementation uses <see langword="lock"/> when you access any <see cref="IVTSPlugin"/> parameters!
    /// </remarks>
    string? Name { get; }
    /// <summary>
    /// Plugin author.
    /// Will be displayed in VTube Studio's plugin authentication popup.
    /// </summary>
    /// <remarks>
    /// Do NOT access in hot paths!
    /// Default implementation uses <see langword="lock"/> when you access any <see cref="IVTSPlugin"/> parameters!
    /// </remarks>
    string? Author { get; }
    /// <summary>
    /// Plugin icon, encoded as Base64 (See also: <seealso cref="Convert.ToBase64String(byte[])"/>)
    /// Will be displayed in VTube Studio's plugin authentication popup.
    /// </summary>
    /// <remarks>
    /// Do NOT access in hot paths!
    /// Default implementation uses <see langword="lock"/> when you access any <see cref="IVTSPlugin"/> parameters!
    /// </remarks>
    string? Icon { get; }

    /// <summary>
    /// Sets <see cref="Name"/>, <see cref="Author"/> and <see cref="Icon"/> for a plugin.
    /// </summary>
    /// <remarks>
    /// Make sure to use a <see langword="lock"/> when modifying <see cref="Name"/>, <see cref="Author"/> and <see cref="Icon"/> parameters!
    /// </remarks>
    /// <param name="name">Name of the plugin. <see langword="null"/> can be replaced with <see cref="DefaultName"/>.</param>
    /// <param name="author">Plugin author. <see langword="null"/> can be replaced with <see cref="DefaultAuthor"/>.</param>
    /// <param name="icon">
    /// Plugin icon, converted to base 64 (See also: <seealso cref="Convert.ToBase64String(byte[])"/>).
    /// <see langword="null"/> can be replaced with <see cref="DefaultIcon"/>.
    /// </param>
    void Set(string? name = null, string? author = null, string? icon = null);
}

/// <summary>
/// Connection instance, actively communicating with <see cref="IVTSEndPoint"/>.
/// </summary>
/// Note #1: Try methods were moved from extensions to here, to allow us to optimize try-catch block usages.
///  Extension methods would have forced us to create 2+ try blocks per each method call.
/// Note #2: API is kept more or less standardized, so it can be used in Consoles as well.
///  We will see how far it goes, but for now, it means we will just avoid adding <see cref="INotifyPropertyChanging"/>
///  and <see cref="INotifyPropertyChanged"/>, and instead will implement them in the class you will have to reference.
public interface IVTSConnection : IVTSRequestManager, IHasVTSLogger
{
    /// <summary>
    /// Current, or last known <see cref="IVTSEndPoint"/> to which this <see cref="IVTSConnection"/> should connect to.
    /// </summary>
    IVTSEndPoint EndPoint { get; }
    /// <summary>
    /// Status of this <see cref="IVTSConnection"/>.
    /// </summary>
    VTSConnectionStatus Status { get; }
    /// <summary>
    /// Tries to reconnect to a last known <see cref="EndPoint"/>.
    /// <para>
    /// In multi-threading:
    /// Will return <see langword="false"/> if <see cref="EndPoint"/> was set to <see langword="null"/> while this method was executing.
    /// Otherwise will always return <see langword="true"/>.
    /// </para>
    /// </summary>
    /// <inheritdoc cref="Reconnect"/>
    AsyncBoolTask TryReconnect();
    /// <summary>
    /// Reconnects to last known <see cref="EndPoint"/>.
    /// <para>
    /// Under concurrency:
    /// will throw an <see cref="Exception"/> if <see cref="EndPoint"/> was set to <see langword="null"/> while this method was executing.
    /// </para>
    /// </summary>
    /// <returns>
    /// <see cref="UniTask"/>, for awaiting <see cref="Disconnect"/> and <see cref="Connect(IVTSEndPoint)"/> consecutively.
    /// </returns>
    AsyncTask Reconnect();
    /// <summary>
    /// Connects to a given <paramref name="endPoint"/>.
    /// <para>
    /// On exception will simply return <see langword="false"/>.
    /// On success, conversely - <see langword="true"/>.
    /// </para>
    /// </summary>
    /// <inheritdoc cref="Connect(IVTSEndPoint)"/>
    AsyncBoolTask TryConnect(IVTSEndPoint endPoint);
    /// <summary>
    /// Connects to a given <paramref name="endPoint"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If already active, and current <see cref="EndPoint"/> is different from provided <paramref name="endPoint"/>
    /// - will safely disconnect from a current <see cref="EndPoint"/> and then connect to a given one.
    /// </para>
    /// <para>
    /// Note: <see cref="EndPoint"/> will be replaced with a new one immediately when calling this method.
    /// </para>
    /// </remarks>
    /// <param name="endPoint"></param>
    /// <returns><see cref="AsyncTask"/> from <see cref="Disconnect"/> and this method combined.</returns>
    AsyncTask Connect(IVTSEndPoint endPoint);
    /// <summary>
    /// Disconnects from current <see cref="EndPoint"/>, if it matches provided <paramref name="endPoint"/>.
    /// <para>
    /// Under concurrency:
    /// Will only disconnect <see cref="IVTSConnection"/> only if end-points still match after change.
    /// </para>
    /// </summary>
    AsyncTask Disconnect(IVTSEndPoint endPoint);
    /// <summary>
    /// Disconnects from current <see cref="EndPoint"/>.
    /// Does nothing if <see cref="IVTSConnection"/> is already disconnected.
    /// <para>
    /// Under concurrency:
    /// Will only attempt disconnection if this <see cref="IVTSConnection"/> wasn't connected to something else while this method was executing.
    /// </para>
    /// </summary>
    /// <returns></returns>
    AsyncTask Disconnect();
}

/// <summary>
/// Experimental implementation testing RequestID processing with zero allocation.
/// </summary>
/// <remarks>
/// Marked as abstract to allow overrides. A default final implementation is provided as <see cref="VTSConnection"/>.
/// </remarks>
public abstract class AbstractVTSConnection : IVTSRequestManager
{
    public IVTSLogger Logger { get; private set; } = IVTSLogger.Default;

    // TODO: Fail all requests on disconnection/end-point kill.
    readonly ConcurrentDictionary<ReadOnlyMemory<byte>, RequestEntry> Requests = new();
    readonly struct RequestEntry(IMemoryOwner<byte> allocation, ResettableCompletionSource<IMemoryOwner<byte>> completion)
    {
        public readonly IMemoryOwner<byte> allocation = allocation;
        public readonly ResettableCompletionSource<IMemoryOwner<byte>> completion = completion;
    }

    public async UniTask<JsonResult<T>> Request<T>(VTSRequestTemplate request, TimeSpan timeout) where T : VTSResponseData
    {
        // Generates RequestID using 16 readable characters from A to P (4 bits).
        const int RequestIDLength = 32;
        Span<byte> values = stackalloc byte[RequestIDLength];
        Random.Shared.NextBytes(values);
        for (int i = 0; i < RequestIDLength; i += 4)
        {
            const int Shift = 'A';
            const int RangeMask = 0b1111; // Range: from A to P.
            values[i] = (byte)((Shift + values[i]) & RangeMask);
            values[i + 1] = (byte)((Shift + values[i + 1]) & RangeMask);
            values[i + 2] = (byte)((Shift + values[i + 2]) & RangeMask);
            values[i + 3] = (byte)((Shift + values[i + 3]) & RangeMask);
        }
        var owner = MemoryPool<byte>.Shared.Rent(RequestIDLength);
        var slice = owner.Memory[..RequestIDLength];
        values.CopyTo(slice.Span);

        // Tries to list the request entry.
        // If failed - returns failed result.
        var source = ResettableCompletionSourcePool.Shared.Rent<IMemoryOwner<byte>>();
        RequestEntry entry = new(owner, source);
        try
        {
            if (Requests.TryAdd(slice, entry))
            {
                // TODO: Send request using IVTSSocket here.

                // TODO: Add pool-able timeout handler here, if timeout is not infinite.

                // Awaits for request to fail or be satisfied.
                // TODO: Add better handling for: cancellation, deserialization failure, etc.
                await source.Task;

                return new JsonResult<T>()
                {
                    Successful = true,
                    ErrorID = ErrorID.None,
                    ErrorMessage = ReadOnlyMemory<byte>.Empty,
                    // TODO: Slice those from a received packet.
                    Timestamp = default,
                    APIName = ReadOnlyMemory<byte>.Empty,
                    RequestID = ReadOnlyMemory<byte>.Empty,
                    APIVersion = ReadOnlyMemory<byte>.Empty,
                    Data = ReadOnlyMemory<byte>.Empty,
                    // Temporary here, until API rework.
                    Logger = Logger,
                    Deserializer = (i) =>
                    {
                        throw new NotImplementedException();
                    }
                };

                static bool Deserializer(ReadOnlyMemory<byte> data, out T instance)
                {

                }
                static bool Disposer(T instance)
                {
                    VTSPacketPool.Shared.Return(instance);
                }
            }
            else
            {
                // TODO: Reattempt RequestID generation 1024 times instead, and release resources if failed.
                Logger.TryLog("Request with a same ID already registered!", this);
            }
        }
        catch (OperationCanceledException) // Cancelled or timed out.
        {
            return JsonResult<T>.FromFailed(ErrorID.None);
        }
        catch (Exception ex)
        {
            Logger.TryLog(ex, this);
            return JsonResult<T>.FromFailed(ErrorID.ClientSideException);
        }
        finally
        {
            ResettableCompletionSourcePool.Shared.Return(source);
            owner.Dispose();
        }
    }
}

/// <summary>
/// Experimental implementation testing RequestID processing with zero allocation.
/// </summary>
/// <remarks>
/// Finalizes implementation in <see cref="AbstractVTSConnection"/>.
/// </remarks>
public sealed class VTSConnection : AbstractVTSConnection;

/// <summary>
/// Interface for an abstract, capable of sending requests somewhere else.
/// </summary>
/// TODO: Rename from Manager to something more sensible for an abstract, sending requests.
///  Carrier? Sender? But it also receives them and keeps track of RequestIDs and timeouts...
public interface IVTSRequestManager
{
    UniTask<JsonResult<T>> Request<T>(VTSRequestTemplate request, TimeSpan timeout) where T : VTSResponseData;
}

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
    public static UniTask<JsonResult<T>> Request<TManager, T>(this TManager manager, VTSRequestTemplate request)
        where TManager : IVTSRequestManager
        where T : VTSResponseData
    {
        return manager.Request<T>(request, Timeout.InfiniteTimeSpan);
    }
}

public static class ParsingHelpers
{
    /// <summary>
    /// Converts input <paramref name="bytes"/> to <see cref="Encoding.UTF8"/> string using <see cref="UTF8Encoding"/>.
    /// </summary>
    /// <param name="bytes">Bytes to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsUTF8String(this ReadOnlyMemory<byte> bytes) => AsUTF8String(bytes.Span);

    /// <summary>
    /// Converts input <paramref name="bytes"/> to <see cref="Encoding.UTF8"/> string using <see cref="UTF8Encoding"/>.
    /// </summary>
    /// <param name="bytes">Bytes to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsUTF8String(this ReadOnlySpan<byte> bytes) => Encoding.UTF8.GetString(bytes);
}

/// <summary>
/// Examples for using <see cref="JsonResult{T}"/>.
/// </summary>
public static class JsonResultUsageExample
{
    static readonly IVTSConnection Connection = null!;
    public static async void HandlingVisUsingBlockVerbose()
    {
        using var request = (await Connection.Request<VTSAPIStateResponseData>(VTSAPIStateRequest.Instance)).TryUnpackScoped();
        if (request.Successful)
        {
            Console.WriteLine($"Request successful! Data: {request.Data}");
        }
        else
        {
            Console.WriteLine($"Request failed! ErrorID: {request.ErrorID}, Message: {request.ErrorMessage.AsUTF8String()}");
        }
    }

    public static async void HandlingViaJsonResult_NoPooling()
    {
        var result = await Connection.Request<VTSAPIStateResponseData>(VTSAPIStateRequest.Instance);
        if (result.TryUnpack(out var data))
        {
            Console.WriteLine($"Request successful! Data: {data}");
        }
        else
        {
            Console.WriteLine($"Request failed! ErrorID: {result.ErrorID}, Message: {result.ErrorMessage.AsUTF8String()}");
        }
    }

    public static async void MostUniform()
    {
        using var result = await Connection.Request<VTSAPIStateResponseData>(VTSAPIStateRequest.Instance);
        using var scope = result.TryUnpackScoped();
        if (scope.Successful)
        {
            Console.WriteLine($"Request successful! Data: {scope.Data}");
        }
        else
        {
            Console.WriteLine($"Request failed! ErrorID: {scope.ErrorID}, Message: {scope.ErrorMessage.AsUTF8String()}");
        }
    }
}

/// <summary>
/// Discovers new <see cref="IVTSEndPoint"/>s.
/// </summary>
public interface IVTSDiscoveryService
{
    /// <summary>
    /// Port on which this <see cref="IVTSDiscoveryService"/> listens on.
    /// </summary>
    ushort Port { get; }
    /// <summary>
    /// Time until a <see cref="IVTSEndPoint"/> should be marked as timed-out.
    /// </summary>
    TimeSpan EndPointTimeout { get; }
    /// <summary>
    /// All currently observed VTube Studio instances in the local network.
    /// </summary>
    ObservableCollection<IVTSEndPoint> EndPoints { get; }
}

/// <summary>
/// Settings for <see cref="IVTSService"/>.
/// </summary>
public readonly struct VTSServiceSettings()
{
    /// <summary>
    /// Preferences for which connections to expose and actively maintain.
    /// </summary>
    public enum ConnectionPreferences : byte
    {
        /// <summary>
        /// Recommended and default mode.
        /// Prefers to maintain connection with currently connected instance until it quits.
        /// </summary>
        /// <remarks>
        /// Normally, connection remains active until it times out, or if VTube Studio explicitly closes the connection.
        /// Changing to <see cref="Recent"/> can be beneficial, if your plugin will never (!) work on a device with multiple VTube Studios.
        /// </remarks>
        Active,
        /// <summary>
        /// Prefers to switch to the most recent VTube Studio instance, even if another active connection already exist.
        /// </summary>
        /// <remarks>
        /// Normally, connection remains active until it times out, or if VTube Studio explicitly closes the connection.
        /// Changing to <see cref="Recent"/> can be beneficial, if your plugin will never (!) work on a device with multiple VTube Studios.
        /// </remarks>
        Recent,
    }

    /// <summary>
    /// Preference for handing new <see cref="IVTSConnection"/>s.
    /// </summary>
    public ConnectionPreferences ConnectionPreference { get; init; } = ConnectionPreferences.Active;
    /// <summary>
    /// Whether <see cref="IVTSService"/> should automatically connect to all new <see cref="IVTSEndPoint"/>s and authenticate a plugin there.
    /// </summary>
    public bool MaintainNewConnections { get; init; } = true;
}

/// <summary>
/// Initializes communication with VTube Studio using <see cref="IVTSEndPoint"/>s.
/// </summary>
/// <remarks>
/// This service will automatically connect to the latest found <see cref="IVTSEndPoint"/>, and authenticate.
/// Also inherits <see cref="IVTSConnection"/> API, allowing you to send requests by using the service directly.
/// </remarks>
public interface IVTSService : IVTSConnection
{
    /// <summary>
    /// Plugin info to use for authentication.
    /// </summary>
    /// TODO: Maybe instead we can allow one service to control multiple plugins?
    ///  This is way better than asking devs to create an entire service for each plugin.
    /// For that we will likely need an abstract, or use <see cref="IVTSPlugin"/> itself as an abstract,
    ///  responsible for communicating plugin info via <see cref="IVTSConnection"/>.
    /// This, however, will mean that <see cref="IVTSConnection"/> won't be able to have a plugin field,
    ///  so authentication will have to be external... I don't like this.
    IVTSPlugin Plugin { get; }
    /// <summary>
    /// Discovery service this <see cref="IVTSStudio"/> tracks.
    /// </summary>
    IVTSDiscoveryService DiscoveryService { get; }
    /// <summary>
    /// Settings, used with this service. Read-only.
    /// </summary>
    VTSServiceSettings Settings { get; }
}

/// <inheritdoc cref="IVTSService"/>
public sealed partial class VTSService : ObservableObject, IVTSService
{
    public required IVTSPlugin Plugin { get; init; }
    public required VTSServiceSettings Settings { get; init; }
    [ObservableProperty] public partial IVTSConnection? Connection { get; private set; }
}

/// <summary>
/// An abstract maintaining connections with all newly found <see cref="IVTSEndPoint"/>s.
/// </summary>
public interface IVTSStudio
{
    /// <summary>
    /// Discovery service this <see cref="IVTSStudio"/> tracks.
    /// </summary>
    IVTSDiscoveryService DiscoveryService { get; }
    /// <summary>
    /// All active VTube Studio connections.
    /// </summary>
    ObservableCollection<IVTSConnection> Connections { get; }
}

/// <inheritdoc cref="IVTSStudio"/>
public sealed class VTSStudio : IVTSStudio
{
    public ObservableCollection<IVTSConnection> Connections => throw new NotImplementedException();
}