using Cysharp.Threading.Tasks;
using System.ComponentModel;

namespace VTSPlugin;

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
