namespace VTSPlugin;

/// <summary>
/// Initialization options for <see cref="IVTSService"/>.
/// </summary>
public interface IVTSServiceInitializationOptions : IVTSInitializationOptions
{
    /// <summary>
    /// Initial plugin info to use when communicating with remote VTube Studio app.
    /// </summary>
    IVTSPlugin Plugin { get; }
    /// <summary>
    /// Initial token storage to use for buffering and serializing authentication tokens.
    /// </summary>
    IVTSTokenStorage TokenStorage { get; }
    /// <summary>
    /// Initial discovery service to use for tracking new VTube Studio app instances.
    /// </summary>
    IVTSDiscoveryService DiscoveryService { get; }
    /// <summary>
    /// Preference for treating new VTube Studio instances.
    /// </summary>
    VTSConnectionPreference ConnectionPreference { get; }
    /// <summary>
    /// Whether to authenticate new connections, even if <see cref="ConnectionPreference"/> will otherwise ignore it.
    /// </summary>
    bool MaintainNewConnections { get; }
}

/// <summary>
/// Initializes communication with VTube Studio using <see cref="IVTSEndPoint"/>s.
/// </summary>
/// <remarks>
/// This service will automatically connect to the latest found <see cref="IVTSEndPoint"/>, and authenticate.
/// Also inherits <see cref="IVTSConnection"/> API, allowing you to send requests by using the service directly.
/// </remarks>
public interface IVTSService : IVTSSubSystem<IVTSServiceInitializationOptions>, IHasVTSLogger, IVTSRequestManager
{
    /// <summary>
    /// Plugin info to use when communicating with remote VTube Studio app.
    /// Initially provided by <see cref="IVTSServiceInitializationOptions.Plugin"/>.
    /// </summary>
    IVTSPlugin? Plugin { get; }
    /// <summary>
    /// Token storage to use for buffering and serializing authentication tokens.
    /// Initially provided by <see cref="IVTSServiceInitializationOptions.TokenStorage"/>.
    /// </summary>
    IVTSTokenStorage? TokenStorage { get; }
    /// <summary>
    /// Discovery service to use for tracking new VTube Studio app instances.
    /// Initially provided by <see cref="IVTSServiceInitializationOptions.DiscoveryService"/>.
    /// </summary>
    IVTSDiscoveryService? DiscoveryService { get; }
}
