using CommunityToolkit.Mvvm.ComponentModel;
using Cysharp.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using VTSPlugin.Requests;

namespace VTSPlugin.WPF;

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

public interface IVTSEndPoint
{
    /// <summary>
    /// Port on which VTube Studio instance communicates on.
    /// </summary>
    ushort Port { get; }
    /// <summary>
    /// Cached <see cref="IPEndPoint"/> instance to use in your <see cref="ClientWebSocket"/> implementation.
    /// </summary>
    /// <remarks>
    /// Unless custom <see cref="IVTSDiscoveryService"/> is used, 
    /// </remarks>
    IPEndPoint IPEndPoint { get; }
    /// <summary>
    /// <see cref="CancellationTokenSource"/>, cancelled on <see cref="Kill"/>
    /// </summary>
    CancellationTokenSource KillTokenSource { get; }
    /// <summary>
    /// Token, cancelled on <see cref="Kill"/>.
    /// </summary>
    public CancellationToken KillToken => KillTokenSource.Token;
    /// <summary>
    /// Whether connection is alive or not.
    /// </summary>
    /// <remarks>
    /// In async programming, this parameter might return <see langword="true"/> microseconds before end-point is killed!
    /// You can use this property for debugging, but in other cases - prefer using <see cref="KillTokenSource"/>.
    /// </remarks>
    public bool Alive => !KillTokenSource.IsCancellationRequested;
    /// <summary>
    /// Last tick when <see cref="KeepAlive"/> was used.
    /// </summary>
    long LastKeepAliveTick { get; }
    /// <summary>
    /// Kills end-point and all associated 
    /// </summary>
    void Kill();
    /// <summary>
    /// Registers <see cref="Environment.TickCount64"/> to <see cref="LastKeepAliveTick"/>.
    /// </summary>
    /// <remarks>
    /// If end-point is killed - method won't throw, but will not restore killed end-point either!
    /// </remarks>
    void KeepAlive();
}

/// <summary>
/// Helpful extensions when working with <see cref="IVTSPlugin"/>s.
/// </summary>
public static class VTSPluginExtensions
{
    const long TicksTo100Nanoseconds = 1000;
    public static long TicksSinceLastKeepAlive(this IVTSEndPoint ep) => TicksSinceLastKeepAlive(ep, Environment.TickCount64);
    public static long TicksSinceLastKeepAlive(this IVTSEndPoint ep, long tickNow)
    {
        tickNow -= ep.LastKeepAliveTick;
        if (tickNow < 0) return 0;
        return tickNow;
    }

    public static TimeSpan TimeSinceLastKeepAlive(this IVTSEndPoint ep) => TimeSinceLastKeepAlive(ep, Environment.TickCount64);
    public static TimeSpan TimeSinceLastKeepAlive(this IVTSEndPoint ep, long tickNow)
    {
        tickNow -= ep.LastKeepAliveTick;
        if (tickNow < 0) return new TimeSpan(0);
        return new TimeSpan(tickNow * TicksTo100Nanoseconds);
    }
}

/// <summary>
/// Describes any object which has <see cref="IVTSLogger"/> attached to them.
/// </summary>
public interface IHasVTSLogger
{
    /// <summary>
    /// Logger to use for any logs and errors.
    /// </summary>
    /// <remarks>
    /// If not provided - returns <see cref="IVTSLogger.Default"/>.
    /// </remarks>
    public IVTSLogger Logger { get; }
}

/// <summary>
/// Connection status of an <see cref="IVTSConnection"/>.
/// </summary>
/// Note: inherits <see langword="int"/> for usage in CLS-compliant <see cref="Interlocked"/> methods.
public enum VTSConnectionStatus : int // Must inherit int
{
    Idle,
    Connecting,
    // Authenticating?
    // Authenticated?
    Connected,
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
    UniTask<bool> TryReconnect();
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
    UniTask Reconnect();
    /// <summary>
    /// Connects to a given <paramref name="endPoint"/>.
    /// <para>
    /// On exception will simply return <see langword="false"/>.
    /// On success, conversely - <see langword="true"/>.
    /// </para>
    /// </summary>
    /// <inheritdoc cref="Connect(IVTSEndPoint)"/>
    UniTask<bool> TryConnect(IVTSEndPoint endPoint);
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
    /// <returns><see cref="UniTask"/> from <see cref="Disconnect"/> and this method combined.</returns>
    UniTask Connect(IVTSEndPoint endPoint);
    /// <summary>
    /// Disconnects from current <see cref="EndPoint"/>, if it matches provided <paramref name="endPoint"/>.
    /// <para>
    /// Under concurrency:
    /// Will only disconnect <see cref="IVTSConnection"/> only if end-points still match after change.
    /// </para>
    /// </summary>
    UniTask Disconnect(IVTSEndPoint endPoint);
    /// <summary>
    /// Disconnects from current <see cref="EndPoint"/>.
    /// Does nothing if <see cref="IVTSConnection"/> is already disconnected.
    /// <para>
    /// Under concurrency:
    /// Will only attempt disconnection if this <see cref="IVTSConnection"/> wasn't connected to something else while this method was executing.
    /// </para>
    /// </summary>
    /// <returns></returns>
    UniTask Disconnect();
}

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
public static class VTSRequestHandlerExtensions
{
    /// <inheritdoc cref="IVTSRequestManager.Request{T}(VTSRequestTemplate, TimeSpan)"/>
    /// <remarks>
    /// Request will have an infinite timeout.
    /// </remarks>
    /// <seealso cref="Timeout.InfiniteTimeSpan"/>
    public static UniTask<JsonResult<T>> Request<T>(this IVTSRequestManager manager, VTSRequestTemplate request) where T : VTSResponseData
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