using System.ComponentModel;
using System.Net;
using System.Net.WebSockets;

namespace VTSPlugin;

public interface IVTSEndPoint : INotifyPropertyChanged
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
    int LastKeepAliveTick { get; }
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
