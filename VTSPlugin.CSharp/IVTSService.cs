namespace VTSPlugin;

/// <summary>
/// Initializes communication with VTube Studio using <see cref="IVTSEndPoint"/>s.
/// </summary>
/// <remarks>
/// This service will automatically connect to the latest found <see cref="IVTSEndPoint"/>, and authenticate.
/// Also inherits <see cref="IVTSConnection"/> API, allowing you to send requests by using the service directly.
/// </remarks>
public interface IVTSService : IVTSRequestManager
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
